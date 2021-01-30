using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;

namespace Rcon.Function
{
    public static class headshotonly
    {
        /// <summary>
        /// changes mode of current match to (not) headshot only
        /// </summary>
        /// <response code="200">Successful operation</response>
        /// <response code="400">Invalid request</response>
        /// <response code="401">Unauthorized</response>
        [FunctionName("headshotonly")]
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
            if (String.IsNullOrWhiteSpace(rconPayload.Parameter[0]))
            {
                return new OkObjectResult("Oops! You must provide a parameter to be executed e.g. `headshotonly 1`.");
            }

            // authorize
            var context = new CosmosDbContext();
            var connectionPayload = await context.GetConnection(rconPayload.AccessToken);
            if (connectionPayload == null)
            {
                return new UnauthorizedResult();
            }

            // execute
            try
            {
                // instantiate client and execute command
                var rconClient = await new RconService(connectionPayload, context).GetClient();
                if (rconPayload.Parameter[0] == "0" || rconPayload.Parameter[0].ToLower() == "off" 
                    || rconPayload.Parameter[0].ToLower() == "false" || rconPayload.Parameter[0].ToLower() == "no")
                {
                    return new OkObjectResult(await rconClient.ExecuteCommandAsync("mp_damage_headshot_only 0"));
                }

                return new OkObjectResult(await rconClient.ExecuteCommandAsync("mp_damage_headshot_only 0"));
            }
            catch (System.Exception)
            {
                return new OkObjectResult("Oops! Server seems to be offline. :flushed:");
            }
        }
    }
}