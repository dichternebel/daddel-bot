using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Generic;

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
                var response = await rconClient.ExecuteCommandAsync("maps *");
                // Get random map that suits the given typen
                var randomMap = getRandomMapByGameType(response, gameType);

                if (gameType == GameTypes.casual)
                {
                    var resp = await rconClient.ExecuteCommandAsync($"game_type 0; game_mode 0; exec gamemode_casual; map {randomMap}; mapgroup mg_{randomMap};");
                    return new OkObjectResult($"Switched server to mode 'Competitive'.\n{resp}");
                }
                if (gameType == GameTypes.competitive)
                {
                    var resp = await rconClient.ExecuteCommandAsync($"game_type 0; game_mode 1; exec gamemode_competitive; map {randomMap}; mapgroup mg_{randomMap};");
                    return new OkObjectResult($"Switched server to mode 'Competitive'.\n{resp}");
                }
                if (gameType == GameTypes.wingman)
                {
                    var resp = await rconClient.ExecuteCommandAsync($"game_type 0; game_mode 2; exec gamemode_competitive2v2; map {randomMap}; mapgroup mg_{randomMap};");
                    return new OkObjectResult($"Switched server to mode 'Wingman'.\n{resp}");
                }
                if (gameType == GameTypes.dangerzone)
                {
                    var resp = await rconClient.ExecuteCommandAsync($"game_type 6; game_mode 0; exec gamemode_survival; map {randomMap}; mapgroup mg_{randomMap};");
                    return new OkObjectResult($"Switched server to mode 'Dangerzone'.\n{resp}");
                }
                if (gameType == GameTypes.armsrace)
                {
                    var resp = await rconClient.ExecuteCommandAsync($"game_type 1; game_mode 0; exec gamemode_armsrace; map {randomMap}; mapgroup mg_{randomMap};");
                    return new OkObjectResult($"Switched server to mode 'Armsrace'.\n{resp}");
                }
                if (gameType == GameTypes.deathmatch)
                {
                    var resp = await rconClient.ExecuteCommandAsync($"game_type 1; game_mode 2; exec gamemode_deathmatch; map {randomMap};mapgroup mg_{randomMap}; sv_infinite_ammo 2; mp_teammates_are_enemies 1;");
                    return new OkObjectResult($"Switched server to mode 'Deathmatch'.\n{resp}");
                }
                if (gameType == GameTypes.teamdeathmatch)
                {
                    var resp = await rconClient.ExecuteCommandAsync($"game_type 1; game_mode 2; exec gamemode_deathmatch; map {randomMap}; mapgroup mg_{randomMap}; sv_infinite_ammo 2;");
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

        private static string getRandomMapByGameType(string mapResponse, GameTypes gameType)
        {
            // create an array from response
            var responseArray = mapResponse.Split("\n"); // split by lines
            responseArray = responseArray.Skip(1).ToArray(); //shift
            responseArray = responseArray.Reverse().Skip(1).Reverse().ToArray(); // pop

            // get those map names only
            var mapList = new List<string>();
            foreach (var item in responseArray)
            {
                var elements = item.Split(new [] {' '}, StringSplitOptions.RemoveEmptyEntries);
                if (elements.Length > 2) mapList.Add(elements[2].Remove(elements[2].Length - 4)); // 'Path' not working here because of workshop maps
            }
            var filteredList = new List<string>();

            switch (gameType)
            {
                case GameTypes.armsrace:
                case GameTypes.wingman:
                    filteredList = mapList.Where(x => x.Contains("ar_")).ToList();
                break;
                
                case GameTypes.casual:
                    filteredList = mapList.Where(x => x.Contains("de_")).ToList();
                break;

                case GameTypes.competitive:
                case GameTypes.deathmatch:
                case GameTypes.teamdeathmatch:
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