using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

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
                return new OkObjectResult("Oops! You must provide a parameter to be executed e.g. `mode wingman`.");
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
                var result = "Oops! Allowed game types are: " + string.Join(", ", Enum.GetNames(typeof(GameTypes))) + ".";

                GameTypes gameType;
                if (!Enum.TryParse(rconPayload.Parameter[0].ToLower(), true, out gameType))
                {
                    return new OkObjectResult(result);
                }

                // Get map pool from server
                var mapList = await RconHelper.GetMaps(rconClient);

                // Set random map that suits the given type or use parameter value(s)
                var currentMap = rconPayload.Parameter.Length > 1 ?
                    mapList.FirstOrDefault(x => x.Contains(rconPayload.Parameter[1].ToLower())) :
                    getRandomMapByGameType(mapList, gameType);

                if (string.IsNullOrEmpty(currentMap))
                {
                    return new OkObjectResult($"Oops! Couldn't find map on server... :flushed:");
                }

                // Set mapgroup by parameter or match currentmap
                var currentMapGroup = rconPayload.Parameter.Length > 2 ?
                    rconPayload.Parameter[2].ToLower() :
                    "mg_" + (currentMap.Contains("workshop") ? currentMap.Split('/')[2] : currentMap);

                // switch game to chosen type and map
                if (gameType == GameTypes.casual)
                {
                    var resp = await rconClient.ExecuteCommandAsync($"game_type 0; game_mode 0; exec gamemode_casual; map {currentMap}; mapgroup {currentMapGroup};");
                    return new OkObjectResult($"Switched server to mode 'Competitive'.\n{resp}");
                }
                if (gameType == GameTypes.competitive)
                {
                    var resp = await rconClient.ExecuteCommandAsync($"game_type 0; game_mode 1; exec gamemode_competitive; map {currentMap}; mapgroup {currentMapGroup};");
                    return new OkObjectResult($"Switched server to mode 'Competitive'.\n{resp}");
                }
                if (gameType == GameTypes.wingman)
                {
                    var resp = await rconClient.ExecuteCommandAsync($"game_type 0; game_mode 2; exec gamemode_competitive2v2; map {currentMap}; mapgroup {currentMapGroup};");
                    return new OkObjectResult($"Switched server to mode 'Wingman'.\n{resp}");
                }
                if (gameType == GameTypes.dangerzone)
                {
                    var resp = await rconClient.ExecuteCommandAsync($"game_type 6; game_mode 0; exec gamemode_survival; map {currentMap}; mapgroup {currentMapGroup};");
                    return new OkObjectResult($"Switched server to mode 'Dangerzone'.\n{resp}");
                }
                if (gameType == GameTypes.armsrace)
                {
                    var resp = await rconClient.ExecuteCommandAsync($"game_type 1; game_mode 0; exec gamemode_armsrace; map {currentMap}; mapgroup {currentMapGroup};");
                    return new OkObjectResult($"Switched server to mode 'Armsrace'.\n{resp}");
                }
                if (gameType == GameTypes.deathmatch)
                {
                    var resp = await rconClient.ExecuteCommandAsync($"game_type 1; game_mode 2; exec gamemode_deathmatch; map {currentMap};mapgroup {currentMapGroup}; sv_infinite_ammo 2; mp_teammates_are_enemies 1;");
                    return new OkObjectResult($"Switched server to mode 'Deathmatch'.\n{resp}");
                }
                if (gameType == GameTypes.teamdeathmatch)
                {
                    var resp = await rconClient.ExecuteCommandAsync($"game_type 1; game_mode 2; exec gamemode_deathmatch; map {currentMap}; mapgroup {currentMapGroup}; sv_infinite_ammo 2;");
                    return new OkObjectResult($"Switched server to mode 'Team Deathmatch'.\n{resp}");
                }
                
                return new OkObjectResult("Oops! That didn't work... :flushed:");
            }
            catch (System.Exception exception)
            {
                log.LogError(exception.Message);
                return new OkObjectResult("Oops! Server seems to be offline. :flushed:");
            }
        }

        private static string getRandomMapByGameType(List<string> mapList, GameTypes gameType)
        {
            var filteredList = new List<string>();

            switch (gameType)
            {
                case GameTypes.armsrace:
                case GameTypes.deathmatch:
                case GameTypes.teamdeathmatch:
                    filteredList = mapList.Where(x => x.Contains("ar_")).ToList();
                break;
                
                case GameTypes.casual:
                case GameTypes.wingman:
                    filteredList = mapList.Where(x => x.Contains("de_")).ToList();
                break;

                case GameTypes.competitive:
                    filteredList = mapList.Where(x => x.Contains("de_") || x.Contains("cs_")).ToList();
                break;

                case GameTypes.dangerzone:
                    filteredList = mapList.Where(x => x.Contains("dz_")).ToList();
                break;

                default:
                    filteredList = mapList;
                break;
            }

            return filteredList.ElementAt(new Random(DateTime.Now.Millisecond).Next(filteredList.Count()));
        }
    }
}