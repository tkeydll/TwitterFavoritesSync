using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TwitterFavoritsSync
{
    public static class TargetAccounts{
        public static List<string> GetCsvList()
        {
            string csvPath = System.Environment.GetEnvironmentVariable("FOLLOW_LIST_PATH");
            using var sr = new StreamReader(csvPath);
            string csvStr = sr.ReadToEnd();
            string[] array = csvStr.Split(",");
            return array.ToList();
        }

        public static async Task<List<FollowUser>> GetJsonListAsync()
        {
            string jsonPath = Environment.GetEnvironmentVariable("FOLLOW_LIST_PATH");
            using var sr = new StreamReader(jsonPath);
            string jsonStr = await sr.ReadToEndAsync();
            return JsonConvert.DeserializeObject<List<FollowUser>>(jsonStr);
        }
    }
}