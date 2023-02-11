using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace TwitterFavoritsSync
{

    public class TargetAccountGetTimerTrigger
    {
        private string targetApiKey = Environment.GetEnvironmentVariable("TARGET_API_KEY");
        private string targetApiSecret = Environment.GetEnvironmentVariable("TARGET_API_SECRET");
        private string targetAccessToken = Environment.GetEnvironmentVariable("TARGET_ACCESS_TOKEN");
        private string targetAccessSecret = Environment.GetEnvironmentVariable("TARGET_ACCESS_SECRET");

        [FunctionName("TargetAccountGetTimerTrigger")]
        public async Task Run([TimerTrigger("0 0 22 * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var client = new TwitterClient(targetApiKey, targetApiSecret, targetAccessToken, targetAccessSecret);
            var friends = await client.GetAllFollowsAsync();

            log.LogInformation($"Get follow users: {friends.Count}");

            // Convert minimum info.
            var users = new List<FollowUser>();
            friends.ForEach(x => users.Add(new FollowUser(x.Id, x.ScreenName)));

            // Get string
            var value = JsonConvert.SerializeObject(users);

            // Save
            await TargetAccounts.SaveTargetListAsync(value);

            log.LogInformation($"Saved.");
        }
    }
}