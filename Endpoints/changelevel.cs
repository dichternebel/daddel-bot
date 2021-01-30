using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using AzureFunctions.Extensions.Swashbuckle.Attribute;

namespace Rcon.Function
{
    internal static class changelevel
    {
        /// <summary>
        /// changes map of current match
        /// </summary>
        /// <response code="200">Map changed, not found or server offline</response>
        /// <response code="400">Map name missing</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Oops!</response>
        [FunctionName("changelevel")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req, String mapName, ILogger log)
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
            if (String.IsNullOrWhiteSpace(rconPayload.Parameter[0]))
            {
                return new OkObjectResult("Oops! You must provide a parameter to be executed e.g. `map de_dust2`.");
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
                var mapList = await RconHelper.GetMaps(rconClient);
                var map = mapList.FirstOrDefault(x => x.Contains(rconPayload.Parameter[0].ToLower()));
                
                if (string.IsNullOrEmpty(map))
                {
                    return new OkObjectResult($"Oops! Couldn't find map {rconPayload.Parameter[0]} on server. :flushed:");
                }

                var result = await rconClient.ExecuteCommandAsync("changelevel " + rconPayload.Parameter[0]);
                return new OkObjectResult(result);
            }
            catch (System.Exception)
            {
                return new OkObjectResult("Oops! Server seems to be offline. :flushed:");
            }
        }
    }
}