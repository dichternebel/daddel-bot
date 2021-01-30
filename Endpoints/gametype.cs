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
        /// changes game type and game mode of the server
        /// </summary>
        /// <response code="200">Successful operation and response payload</response>
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
                var result = "Oops! Allowed game types are: " + string.Join(", ", Enum.GetNames(typeof(GameTypes))) + ".";

                GameTypes gameType;
                if (!Enum.TryParse(rconPayload.Parameter[0].ToLower(), true, out gameType))
                {
                    return new OkObjectResult(result);
                }

                // Get map pool from server
                var mapList = await RconHelper.GetMaps(rconClient);

                // Set random map that suits the given type or use parameter value(s)
                var currentMap = String.Empty;
                if (rconPayload.Parameter.Length < 2)
                {
                    currentMap = getRandomMapByGameType(mapList, gameType);
                }
                else if (rconPayload.Parameter.Length > 1)
                {
                    // exact match
                    currentMap = mapList.FirstOrDefault(x => x == rconPayload.Parameter[1].ToLower());
                    // starts with
                    if (string.IsNullOrEmpty(currentMap))
                    {
                        currentMap = mapList.FirstOrDefault(x => x.StartsWith(rconPayload.Parameter[1].ToLower()));
                    }
                    // ends with
                    if (string.IsNullOrEmpty(currentMap))
                    {
                        currentMap = mapList.FirstOrDefault(x => x.EndsWith(rconPayload.Parameter[1].ToLower()));
                    }
                    // contains -> Warning: could also load de_dust2_se if de_dust2 is parameter
                    if (string.IsNullOrEmpty(currentMap))
                    {
                        currentMap = mapList.FirstOrDefault(x => x.Contains(rconPayload.Parameter[1].ToLower()));
                    }
                    // Todo: Fuzzy search? ... No, just kidding. ;-)
                }

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
                    return new OkObjectResult($"Switched server to mode 'Casual'.\n{resp}");
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
                    // picked from https://github.com/timche/csgo-server-configs/blob/master/cfg/deathmatch.cfg
                    var resp = await rconClient.ExecuteCommandAsync($@"
                    game_type 1; game_mode 2; exec gamemode_deathmatch; map {currentMap};mapgroup {currentMapGroup};
                    sv_infinite_ammo 2;
                    mp_teammates_are_enemies 1;
                    mp_timelimit 0;
                    mp_tkpunish 0;
                    mp_warmup_pausetimer 1;
                    mp_warmuptime 1;
                    bot_quota 0;
                    mp_friendlyfire 1;
                    mp_autokick 0;
                    mp_autoteambalance 0;
                    mp_buy_allow_grenades 0;
                    mp_buy_allow_guns 0;
                    mp_ct_default_grenades "";
                    mp_ct_default_melee weapon_knife;
                    mp_ct_default_primary weapon_ak47;
                    mp_ct_default_secondary weapon_deagle;
                    mp_t_default_grenades "";
                    mp_t_default_melee weapon_knife;
                    mp_t_default_primary weapon_ak47;
                    mp_t_default_secondary weapon_deagle;
                    mp_free_armor 2;
                    mp_death_drop_gun 0;
                    mp_defuser_allocation 0;
                    mp_give_player_c4 0;
                    mp_limitteams 0;
                    mp_logdetail 0;
                    mp_match_can_clinch 1;
                    mp_match_end_restart 1;
                    mp_randomspawn 1;
                    mp_respawn_on_death_ct 1;
                    mp_respawn_on_death_t 1;
                    ");
                    return new OkObjectResult($"Switched server to mode 'Deathmatch (FFA)'.\n{resp}");
                }
                if (gameType == GameTypes.teamdeathmatch)
                {
                    var resp = await rconClient.ExecuteCommandAsync($@"
                    game_type 1; game_mode 2; exec gamemode_deathmatch; map {currentMap}; mapgroup {currentMapGroup};
                    sv_infinite_ammo 2;
                    ");
                    return new OkObjectResult($"Switched server to mode 'Team Deathmatch'.\n{resp}");
                }
                
                return new OkObjectResult("Oops! That didn't work... :flushed:");
            }
            catch (System.Exception ex)
            {
                log.LogError(ex.Message);
                return new OkObjectResult("Oops! Server did not respond. :flushed:");
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