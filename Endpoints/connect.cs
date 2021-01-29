using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Rcon.Function
{
    public static class connect
    {
        private static bool isAccessTokenValid(string token)
        {
            // poor validation for token
            Match match = Regex.Match(token, @"^[\d]+\-[\d]+");
            return match.Success;
        }

        /// <summary>
        /// get, upsert or remove discord channel config for/from usage with rcon
        /// </summary>
        /// <group>configmanagement</group>
        /// <verb>GET,POST,DELETE/verb>
        /// <url>https://rcon.azurewebsites.net/api/connect</url>
        /// <remarks>adds or removes connection</remarks>
        /// <response code="200">successful operation and response payload</response>
        /// <response code="400">Invalid request</response>
        /// <response code="401">Unauthorized</response>
        [FunctionName("connect")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", "delete", Route = null)] HttpRequest req, ILogger log)
        {
            var context = new CosmosDbContext();

            // GET
            if (req.Method.ToUpper() == "GET")
            {
                var simplePayload = await new RequestParser(req).GetRconPayload();
                if (req.ContentType.ToLower() != "application/json") return new BadRequestResult();
                // check payload
                if (simplePayload.IsValid == null) return new UnauthorizedResult();
                if (!simplePayload.IsValid.Value) return new BadRequestResult();
                // authorize
                var resultObject = await context.GetConnection(simplePayload.AccessToken);
                return new OkObjectResult(resultObject);
            }

           // POST
            if (req.Method.ToUpper() == "POST")
            {
                var connectionPayload = await new RequestParser(req).GetConnectionPayload();
                // check payload
                if (connectionPayload.IsValid == null) return new UnauthorizedResult();
                if (!connectionPayload.IsValid.Value) return new BadRequestResult();
                if (!isAccessTokenValid(connectionPayload.AccessToken)) return new BadRequestResult();
                // upsert connection
                await context.SetConnection(connectionPayload);
            }

            // DELETE
            else if (req.Method.ToUpper() == "DELETE")
            {
                var simplePayload = await new RequestParser(req).GetRconPayload();
                // authorize
                var connectionPayload = await context.GetConnection(simplePayload.AccessToken);
                if (connectionPayload == null) return new UnauthorizedResult();
                if (simplePayload.Parameter.Length > 0
                    && simplePayload.Parameter[0] == "all")
                {
                    await context.DeleteConnections(simplePayload.AccessToken);
                }
                else await context.DeleteConnection(connectionPayload);
            }
            
            return new OkObjectResult("00,OK,00,00");
        }
    }
}