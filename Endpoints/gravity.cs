using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;

namespace Rcon.Function
{
    public static class gravity
    {
        /// <summary>
        /// upsert or remove discord channel for/from usage with rcon
        /// </summary>
        /// <group>rcon</group>
        /// <verb>GET</verb>
        /// <url>https://rcon.azurewebsites.net/api/gravity</url>
        /// <remarks>changes gravity of current match to given value</remarks>
        /// <response code="200">successful operation and response payload</response>
        /// <response code="400">Invalid request</response>
        /// <response code="401">Unauthorized</response>
        [FunctionName("gravity")]
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
                return new OkObjectResult("Oops! You must provide a parameter to be executed e.g. `gravity pluto`.");
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
                var rconClient = await new RconService(connectionPayload).GetClient();
                var result = "Oops! Allowed values for `gravity` are: " + string.Join(", ", Enum.GetNames(typeof(GravityTypes))) + ".";
                GravityTypes gravityType;
                if (Enum.TryParse(rconPayload.Parameter[0].ToLower(), true, out gravityType))
                {
                    result = await rconClient.ExecuteCommandAsync("sv_gravity " + (int)gravityType);
                }
                return new OkObjectResult(result);
            }
            catch (System.Exception)
            {
                return new OkObjectResult("Oops! Server seems to be offline. :flushed:");
            }
        }
    }
}