using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Rcon.Function
{
    public static class connect
    {
        /// <summary>
        /// upsert or remove discord channel for/from usage with rcon
        /// </summary>
        /// <group>configmanagement</group>
        /// <verb>POST,DELETE/verb>
        /// <url>https://rcon.azurewebsites.net/api/connect</url>
        /// <remarks>adds or removes connection</remarks>
        /// <response code="200">successful operation and response payload</response>
        /// <response code="400">Invalid request</response>
        /// <response code="401">Unauthorized</response>
        [FunctionName("connect")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", "delete", Route = null)] HttpRequest req, ILogger log)
        {
            // ToDo: Upsert or remove onnection to DB
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
            var context = new CosmosDbContext();
            var connectionPayload = await context.GetConnection(connPayload.AccessToken);
            if (connectionPayload == null && req.Method.ToUpper() != "POST") 
            {
                return new UnauthorizedResult();
            }
            else if (req.Method.ToUpper() == "DELETE")
            {
                await context.DeleteConnection(connectionPayload);
                return new OkObjectResult("00,OK,00,00");
            }

            // poor validation for token
            Match match = Regex.Match(connPayload.AccessToken, @"^[\d]+\-[\d]+");
            if (!match.Success) return new UnauthorizedResult();

            // upsert connection
            await context.SetConnection(connPayload);
            return new OkObjectResult("00,OK,00,00");
        }
    }
}