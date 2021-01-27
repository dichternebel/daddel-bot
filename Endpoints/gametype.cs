using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;

namespace Rcon.Function
{
    public static class gametype
    {
        /// <summary>
        /// upsert or remove discord channel for/from usage with rcon
        /// </summary>
        /// <group>rcon</group>
        /// <verb>GET</verb>
        /// <url>https://rcon.azurewebsites.net/api/gametype</url>
        /// <remarks>changes game type and game mode of the server</remarks>
        /// <response code="200">successful operation and response payload</response>
        /// <response code="400">Invalid request</response>
        /// <response code="401">Unauthorized</response>
        [FunctionName("gametype")]
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
                return new OkObjectResult("Oops! You must provide a parameter to be executed e.g. `gametype wingman`.");
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
                var result = "Oops! Allowed values for `gametype` are: " + string.Join(", ", Enum.GetNames(typeof(GameTypes))) + ".";

                GameTypes gameType;
                if (Enum.TryParse(rconPayload.Parameter[0].ToLower(), true, out gameType))
                {
                    if (gameType == GameTypes.competitive)
                    {
                        //var resp = await rconClient.ExecuteCommandAsync("game_type 0; game_mode 1; exec gamemode_competitive; map de_anubis; mapgroup mg_cl_daddel; mp_restartgame 1;");
                        var resp = await rconClient.ExecuteCommandAsync("game_type 0; game_mode 1; exec gamemode_competitive; map de_mirage; mapgroup mg_active;");
                        result = $"Switched server to mode 'Competitive'.\n{resp}";
                    }
                    if (gameType == GameTypes.wingman)
                    {
                        //var resp = await rconClient.ExecuteCommandAsync("game_type 0; game_mode 2; exec gamemode_competitive2v2; map de_lake; mapgroup mg_wm_daddel; mp_restartgame 1;");
                        var resp = await rconClient.ExecuteCommandAsync("game_type 0; game_mode 2; exec gamemode_competitive2v2; map de_lake; mapgroup mg_wm_daddel;");
                        result = $"Switched server to mode 'Wingman'.\n{resp}";
                    }
                    if (gameType == GameTypes.dangerzone)
                    {
                        //var resp = await rconClient.ExecuteCommandAsync("game_type 6; game_mode 0; exec gamemode_survival; map workshop\\1871501511\\dz_junglety; mapgroup mg_dz_daddel; mp_restartgame 1;");
                        var resp = await rconClient.ExecuteCommandAsync("game_type 6; game_mode 0; exec gamemode_survival; map dz_blacksite; mapgroup mg_dz_daddel;");
                        result = $"Switched server to mode 'Dangerzone'.\n{resp}";
                    }
                    if (gameType == GameTypes.armsrace)
                    {
                        var resp = await rconClient.ExecuteCommandAsync("game_type 1; game_mode 0; exec gamemode_armsrace; map ar_shoots; mapgroup mg_armsrace;");
                        result = $"Switched server to mode 'Armsrace'.\n{resp}";
                    }
                    if (gameType == GameTypes.deathmatch)
                    {
                        var resp = await rconClient.ExecuteCommandAsync("game_type 1; game_mode 2; exec gamemode_deathmatch; mapgroup mg_demolition; sv_infinite_ammo 2; mp_teammates_are_enemies 1;");
                        result = $"Switched server to mode 'Deathmatch'.\n{resp}";
                    }
                    if (gameType == GameTypes.teamdeathmatch)
                    {
                        var resp = await rconClient.ExecuteCommandAsync("game_type 1; game_mode 2; exec gamemode_deathmatch; mapgroup mg_demolition; sv_infinite_ammo 2;");
                        result = $"Switched server to mode 'Team Deathmatch'.\n{resp}";
                    }
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