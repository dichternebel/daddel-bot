using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Rcon.Function
{
    public static class maps
    {
        /// <summary>
        /// returns CS:GO server maps
        /// </summary>
        /// <response code="200">Successful operation and response payload</response>
        /// <response code="400">Invalid request</response>
        /// <response code="401">Unauthorized</response>
        [FunctionName("maps")]
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
                var result = await rconClient.ExecuteCommandAsync("maps *", true);
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