using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace TwitterFavoritsSync
{
    public class TargetAccountGetHttpTrigger
    {
        // Target Account
        private string targetApiKey = Environment.GetEnvironmentVariable("TARGET_API_KEY");
        private string targetApiSecret = Environment.GetEnvironmentVariable("TARGET_API_SECRET");
        private string targetAccessToken = Environment.GetEnvironmentVariable("TARGET_ACCESS_TOKEN");
        private string targetAccessSecret = Environment.GetEnvironmentVariable("TARGET_ACCESS_SECRET");


        [FunctionName("TargetAccountGetHttpTrigger")]
        public async Task<OkObjectResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var client = new TwitterClient(targetApiKey, targetApiSecret, targetAccessToken, targetAccessSecret);
            var friends = await client.GetAllFollowsAsync();

            log.LogInformation($"Get follow users: {friends.Count}");

            // Convert minimum info.
            var users = new List<FollowUser>();
            friends.ForEach(x => users.Add(new FollowUser(x.Id, x.ScreenName)));

            // Get string
            var value = JsonConvert.SerializeObject(users);

            return new OkObjectResult(value);
        }

    }
}