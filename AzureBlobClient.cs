using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace TwitterFavoritsSync
{

    public class AzureBlobClient
    {
        BlobContainerClient _client = null;

        public AzureBlobClient(string connectionString, string containerName)
        {
            _client = new BlobContainerClient(connectionString, containerName);
        }

        /// <summary>
        /// Upload string to Blob file.
        /// </summary>
        /// <param name="json">Json formatted string.</param>
        /// <param name="fileName">Filename</param>
        /// <returns></returns>        
        public async Task UploadStreamAsync(string json, string fileName)
        {
            BlobClient blobClient = _client.GetBlobClient(fileName);
            await blobClient.UploadAsync(BinaryData.FromString(json), overwrite: true);
        }

        /// <summary>
        /// Get json string from blob file.
        /// </summary>
        /// <param name="fileName">Filename</param>
        /// <returns></returns>
        public async Task<string> GetFollowListAsync(string fileName)
        {
            BlobClient blobClient = _client.GetBlobClient(fileName);
            BlobDownloadResult result = await blobClient.DownloadContentAsync();
            return result.Content.ToString();
        }

    }
}