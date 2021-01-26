using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;

namespace Rcon.Function
{
    public static class pistolonly
    {
        /// <summary>
        /// upsert or remove discord channel for/from usage with rcon
        /// </summary>
        /// <group>rcon</group>
        /// <verb>GET</verb>
        /// <url>https://rcon.azurewebsites.net/api/pistolonly</url>
        /// <remarks>changes mode of current match to (not) pistol only</remarks>
        /// <response code="200">successful operation and response payload</response>
        /// <response code="400">Invalid request</response>
        /// <response code="401">Unauthorized</response>
        [FunctionName("pistolonly")]
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
                return new OkObjectResult("Oops! You must provide a parameter to be executed e.g. `pistolonly 1`.");
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
                if (rconPayload.Parameter[0] == "0" || rconPayload.Parameter[0].ToLower() == "off" 
                    || rconPayload.Parameter[0].ToLower() == "false" || rconPayload.Parameter[0].ToLower() == "no")
                {
                    return new OkObjectResult(await rconClient.ExecuteCommandAsync("mp_maxmoney 16000;mp_startmoney 800;mp_afterroundmoney 0;mp_overtime_startmoney 10000; bot_pistols_only 0; mp_dm_bonus_length_max 30; sv_infinite_ammo 0; mp_weapons_allow_heavy -1; mp_weapons_allow_pistols -1; mp_weapons_allow_rifles -1; mp_weapons_allow_smgs -1;"));
                }

                return new OkObjectResult(await rconClient.ExecuteCommandAsync("mp_maxmoney 700; mp_startmoney 700; mp_afterroundmoney 700; mp_overtime_startmoney 700; bot_pistols_only 1; mp_dm_bonus_length_max 0; sv_infinite_ammo 2; mp_weapons_allow_heavy 0; mp_weapons_allow_pistols -1; mp_weapons_allow_rifles 0; mp_weapons_allow_smgs 0;"));
            }
            catch (System.Exception)
            {
                return new OkObjectResult("Oops! Server seems to be offline. :flushed:");
            }
        }
    }
}