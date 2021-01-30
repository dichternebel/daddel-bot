using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

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
            string isEnabled = req.Query["isEnabled"];
            string role = req.Query["role"];

            accessToken = accessToken ?? req.Headers["accessToken"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            server = server ?? data?.server;
            port = port ?? data?.port;
            password = password ?? data?.password;
            // accessToken = accessToken ?? data?.accessToken;
            isEnabled = isEnabled ?? data?.isEnabled;
            role = role ?? data?.role;

            int tempVal;
            int? portNumber = Int32.TryParse(port, out tempVal) ? Int32.Parse(port) : (int?)null;

            var result = new ConnectionPayload {
                AccessToken = accessToken,
                Server = server,
                Port = portNumber,
                Password = password,
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
            accessToken = accessToken ?? req.Headers["accessToken"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            parameter = parameter ?? data?.param;
            // accessToken = accessToken ?? data?.accessToken;

            if (!string.IsNullOrWhiteSpace(parameter))
            {
                parameter = parameter.Trim();
                var myWriter = new StringWriter();
                HttpUtility.HtmlDecode(parameter, myWriter);
                parameter = myWriter.ToString();
            }
            else {
                parameter = "";
            }
            var parameterArray = parameter.Split(' ');

            var result = new RconPayload {
                AccessToken = accessToken,
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
            }

            return true;
        }
    }
}