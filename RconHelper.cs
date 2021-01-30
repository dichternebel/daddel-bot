using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RconSharp;

namespace Rcon.Function
{
    internal static class RconHelper
    {
        public static async Task<List<string>> GetMaps(RconClient rconClient)
        {
            var mapList = new List<string>();
            var mapResponse = await rconClient.ExecuteCommandAsync("maps *");
            // create an array from response
            var responseArray = mapResponse.Split("\n"); // split by lines
            responseArray = responseArray.Skip(1).ToArray(); //shift
            responseArray = responseArray.Reverse().Skip(1).Reverse().ToArray(); // pop

            // get those map names only
            foreach (var item in responseArray)
            {
                var elements = item.Split(new [] {' '}, StringSplitOptions.RemoveEmptyEntries);
                if (elements.Length > 2) mapList.Add(elements[2].Remove(elements[2].Length - 4)); // 'Path' not working here because of workshop maps
            }

            mapList.Sort();
            return mapList;
        }
    }
}