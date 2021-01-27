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
                if (req.ContentType.ToLower() != "application/json")
                {
                    return new BadRequestResult();
                }

                var simplePayload = await new RequestParser(req).GetRconPayload();

                // check payload
                if (simplePayload.IsValid == null)
                {
                    return new UnauthorizedResult();
                }
                if (!simplePayload.IsValid.Value)
                {
                    return new BadRequestResult();
                }
                // authorize
                var resultObject = await context.GetConnection(simplePayload.AccessToken);
                return new OkObjectResult(resultObject);
            }

            var connPayload = await new RequestParser(req).GetConnectionPayload();
            // check payload
            if (connPayload.IsValid == null)
            {
                return new UnauthorizedResult();
            }
            if (!connPayload.IsValid.Value)
            {
                return new BadRequestResult();
            }
            
            // authorize
            var connectionPayload = await context.GetConnection(connPayload.AccessToken);

            // POST
            if (req.Method.ToUpper() == "POST")
            {
                if (!isAccessTokenValid(connPayload.AccessToken)) return new BadRequestResult();
                // upsert connection
                await context.SetConnection(connPayload);
            }

            // DELETE
            else if (req.Method.ToUpper() == "DELETE")
            {
                if (connectionPayload == null) 
                {
                    return new UnauthorizedResult();
                }
                await context.DeleteConnection(connectionPayload);
            }
            
            return new OkObjectResult("00,OK,00,00");
        }
    }
}