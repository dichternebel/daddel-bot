using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Rcon.Function
{
    internal static class bot_kick
    {
        /// <summary>
        /// kicks all bots from current match
        /// </summary>
        /// <response code="200">Bots kicked</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Oops!</response>
        [FunctionName("bot_kick")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req, ILogger log)
        {
            var rconPayload = await new RequestParser(req).GetRconPayload();
            // check payload
            if (rconPayload.IsValid == null)
            {
                return new UnauthorizedResult();
            }
            if (!rconPayload.IsValid.Value)
            {
                return new BadRequestResult();
            }

            // authorize
            var context = new CosmosDbContext();
            var connectionPayload = await context.GetConnection(rconPayload);
            if (connectionPayload == null)
            {
                return new UnauthorizedResult();
            }

            // execute
            try
            {
                // instantiate client and execute command
                var rconClient = await new RconService(connectionPayload, context).GetClient();
                var result = await rconClient.ExecuteCommandAsync("bot_quota 0; bot_kick");
                return new OkObjectResult(result);
            }
            catch (System.Exception ex)
            {
                log.LogError(ex.Message);
                return new OkObjectResult("Oops! Server did not respond. :flushed:");
            }
        }
    }
}