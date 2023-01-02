using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace TwitterFavoritsSync
{
    public static class TargetAccounts{
        public static List<string> GetList()
        {
            string csvPath = System.Environment.GetEnvironmentVariable("FOLLOW_LIST_PATH");
            using var sr = new StreamReader(csvPath);
            string csvStr = sr.ReadToEnd();
            string[] array = csvStr.Split(",");
            return array.ToList();
        }
    }
}