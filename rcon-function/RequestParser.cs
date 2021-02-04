using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Rcon.Function
{
    internal class RequestParser
    {
        private HttpRequest req { get; set; }

        public RequestParser(HttpRequest req)
        {
            this.req = req;
        }

        public async Task<ConnectionPayload> GetConnectionPayload()
        {
            string server = req.Query["server"];
            string port = req.Query["port"];
            string password = req.Query["password"];
            string accessToken = req.Query["accessToken"];
            string salt = req.Query["salt"];
            string isEnabled = req.Query["isEnabled"];
            string role = req.Query["role"];

            accessToken = accessToken ?? req.Headers["accessToken"];
            salt = salt ?? req.Headers["salt"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<JObject>(requestBody);

            if (data != null)
            {
                server = server ?? (data.ContainsKey("server") ? data.GetValue("server").ToString() : null);
                port = port ?? (data.ContainsKey("port") ? data.GetValue("port").ToString() : null);
                password = password ?? (data.ContainsKey("password") ? data.GetValue("password").ToString() : null);
                isEnabled = isEnabled ?? (data.ContainsKey("isEnabled") ? data.GetValue("isEnabled").ToString() : null);
                role = role ?? (data.ContainsKey("role") ? data.GetValue("role").ToString() : null);
            }

            int tempInt;
            int? portNumber = Int32.TryParse(port, out tempInt) ? Int32.Parse(port) : (int?)null;
            long tempLong;
            long? saltNumber = Int64.TryParse(salt, out tempLong) ? Int64.Parse(salt) : (long?)null;

            var result = new ConnectionPayload {
                AccessToken = accessToken,
                Server = server,
                Port = portNumber,
                Password = password,
                Salt = saltNumber,
                IsEnabled = Convert.ToBoolean(isEnabled),
                Role = role
            };

            result.IsValid = this.isValid(result);
            return result;
        }

        public async Task<RconPayload> GetRconPayload()
        {
            string parameter = req.Query["param"];
            string accessToken = req.Query["accessToken"];
            string salt = req.Query["salt"];
            accessToken = accessToken ?? req.Headers["accessToken"];
            salt = salt ?? req.Headers["salt"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<JObject>(requestBody);
            if (data != null)
            {
                parameter = parameter ?? (data.ContainsKey("param") ? data.GetValue("param").ToString() : null);
            }

            long tempVal;
            long? saltNumber = Int64.TryParse(salt, out tempVal) ? Int64.Parse(salt) : (long?)null;

            if (!string.IsNullOrWhiteSpace(parameter))
            {
                parameter = parameter.Trim();
                var myWriter = new StringWriter();
                HttpUtility.HtmlDecode(parameter, myWriter);
                parameter = myWriter.ToString();
            }
            else
            {
                parameter = "";
            }
            var parameterArray = parameter.Split(' ');

            var result = new RconPayload {
                AccessToken = accessToken,
                Salt = saltNumber,
                Parameter = parameterArray
            };

            result.IsValid = this.isValid(result);
            return result;
        }

        private bool? isValid (object entity)
        {
            // WAT?
            if (this.req == null && (entity == null)) return false;

            if (entity is ConnectionPayload)
            {
                var connectionPayload = (ConnectionPayload)entity;

                // Unauthorized
                if (String.IsNullOrWhiteSpace(connectionPayload.AccessToken)) return null;
                if (!connectionPayload.Salt.HasValue) return null;

                // Bad Request
                if (connectionPayload.IsEnabled)
                {
                    if (!connectionPayload.Port.HasValue) return false;
                    if (connectionPayload.Port.Value < 80 || connectionPayload.Port.Value > 65535) return false;
                    if (String.IsNullOrWhiteSpace(connectionPayload.Server)) return false;
                 }
            }
            else
            {
                var rconPayload = entity as RconPayload;
                if (rconPayload == null) return false;

                // Unauthorized
                if (String.IsNullOrWhiteSpace(rconPayload.AccessToken)) return null;
                if (!rconPayload.Salt.HasValue) return null;
            }

            return true;
        }
    }
}