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
    public class SyncFavoritesTimerTrigger
    {
        // Source Account
        private string sourceApiKey = Environment.GetEnvironmentVariable("SOURCE_API_KEY");
        private string sourceApiSecret = Environment.GetEnvironmentVariable("SOURCE_API_SECRET");
        private string sourceAccessToken = Environment.GetEnvironmentVariable("SOURCE_ACCESS_TOKEN");
        private string sourceAccessSecret = Environment.GetEnvironmentVariable("SOURCE_ACCESS_SECRET");

        // Target Account
        private string targetApiKey = Environment.GetEnvironmentVariable("TARGET_API_KEY");
        private string targetApiSecret = Environment.GetEnvironmentVariable("TARGET_API_SECRET");
        private string targetAccessToken = Environment.GetEnvironmentVariable("TARGET_ACCESS_TOKEN");
        private string targetAccessSecret = Environment.GetEnvironmentVariable("TARGET_ACCESS_SECRET");

        // Target List
        string connectionString = Environment.GetEnvironmentVariable("BLOB_CONNECTION_STRING");
        string containerName = Environment.GetEnvironmentVariable("BLOB_CONTAINER_NAME");
        string fileName = Environment.GetEnvironmentVariable("BLOB_FILENAME");


        [FunctionName("SyncFavoritesTimerTrigger")]
        public async Task Run([TimerTrigger("%ScheduleAppSetting%")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");


            // Get target follows.
            var jsonStr = await GetListAsync();
            var accountList = JsonConvert.DeserializeObject<List<FollowUser>>(jsonStr);
            log.LogInformation($"Target count: {accountList.Count}");

            // Get source likes.
            var sourceClient = new TwitterClient(sourceApiKey, sourceApiSecret, sourceAccessToken, sourceAccessSecret);
            var favorites = await sourceClient.GetFavoritesAsync();

            // Get target follows tweet.
            var targetTweet = accountList.SelectMany(a => favorites.Where(f => f.User.Id == a.Id));
            log.LogInformation($"Hit: {targetTweet.Count()}");

            if (targetTweet.Count() == 0)
            {
                return;
            }


            // Target user
            var targetClient = new TwitterClient(targetApiKey, targetApiSecret, targetAccessToken, targetAccessSecret);

            foreach (var t in targetTweet)
            {   
                try
                {
                    await targetClient.AddFavoritsAsync(t.Id);
                    log.LogInformation($"Add favorits in target account: {t.User.ScreenName}");
                }
                catch (Exception ex)
                {
                    log.LogWarning(ex.ToString());
                }

                await sourceClient.RemoveFavoritsAsync(t.Id);
                log.LogInformation($"Remove favorits in source account: {t.User.ScreenName}");
            }

            log.LogInformation($"Execute finished at: {DateTime.Now}");

        }

        /// <summary>
        /// Get list.
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetListAsync()
        {
            var blob = new AzureBlobClient(connectionString, containerName);
            return await blob.GetFollowListAsync(fileName);
        }
    }
}
