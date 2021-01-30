using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Rcon.Function
{
    internal static class bot_add
    {
        /// <summary>
        /// sets bot mode back to normal and adds bots
        /// </summary>
        /// <response code="200">Bot added</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Oops!</response>
        [FunctionName("bot_add")]
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
                var result = await rconClient.ExecuteCommandAsync("bot_quota_mode normal; bot_add");
                return new OkObjectResult(result);
            }
            catch (System.Exception)
            {
                return new OkObjectResult("Oops! Server seems to be offline. :flushed:");
            }
        }
    }
}