using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Rcon.Function
{
    internal static class connect
    {
        private static bool isAccessTokenValid(string token)
        {
            // poor validation for token
            Match match = Regex.Match(token, @"^[\d]+\-[\d]+");
            return match.Success;
        }

        /// <summary>
        /// get discord channel config for usage with rcon
        /// </summary>
        /// <response code="200">Authenticated</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Oops!</response>
        /// <param name="req">Request</param>    
        [ProducesResponseType(typeof(ConnectionPayload), 200)]
        [FunctionName("connect-get")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "connect")] HttpRequest req, ILogger log)
        {
            var context = new CosmosDbContext();
            var simplePayload = await new RequestParser(req).GetRconPayload();
            if (req.ContentType.ToLower() != "application/json") return new BadRequestResult();
            // check payload
            if (simplePayload.IsValid == null) return new UnauthorizedResult();
            if (!simplePayload.IsValid.Value) return new BadRequestResult();
            // authorize
            var resultObject = await context.GetConnection(simplePayload.AccessToken);
            return new OkObjectResult(resultObject);
        }

        /// <summary>
        /// upsert discord channel config for usage with rcon
        /// </summary>
        /// <response code="200">Authenticated</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Oops!</response>
        [FunctionName("connect-post")]
        public static async Task<IActionResult> Run2([HttpTrigger(AuthorizationLevel.Function, "post", Route = "connect")] HttpRequest req, ILogger log)
        {
            var context = new CosmosDbContext();
            var connectionPayload = await new RequestParser(req).GetConnectionPayload();
            // check payload
            if (connectionPayload.IsValid == null) return new UnauthorizedResult();
            if (!connectionPayload.IsValid.Value) return new BadRequestResult();
            if (!isAccessTokenValid(connectionPayload.AccessToken)) return new BadRequestResult();
            // upsert connection
            await context.SetConnection(connectionPayload);
            return new OkObjectResult("00,OK,00,00");
        }

        /// <summary>
        /// remove discord channel config from usage with rcon
        /// </summary>
        /// <response code="200">Authenticated</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Oops!</response>
        /// <param name="req"></param> // -- ToDo: NOT WORKING
        [HttpDelete("{req}")] // -- ToDo: NOT WORKING
        [ProducesResponseType(typeof(ConnectionPayload), 200)]
        [FunctionName("connect-delete")]
        public static async Task<IActionResult> Run3([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "connect")] HttpRequest req, ILogger log)
        {
            var context = new CosmosDbContext();
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
            return new OkObjectResult("00,OK,00,00");
        }
    }
}