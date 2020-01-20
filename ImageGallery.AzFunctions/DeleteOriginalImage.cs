using System;
using System.IO;
using System.Threading.Tasks;
using ImageGallery.Model;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;

namespace ImageGallery.AzFunctions
{
    public static class DeleteOriginalImage
    {
        [FunctionName("DeleteOriginalImage")]
        public static async Task Run([BlobTrigger(Config.WatermarkedContainer + "/{name}", 
            Connection = "AzureWebJobsStorage")]Stream myBlob, 
            string name, 
            ILogger log)
        {
            if (!myBlob.CanRead)
            {
                log.LogError($"{Config.ImagesContainer} Blob has no Read access");
                return;
            }

            var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage", EnvironmentVariableTarget.Process);
            var account = CloudStorageAccount.Parse(connectionString);
            var client = account.CreateCloudBlobClient();

            var container = client.GetContainerReference(Config.ImagesContainer);
            var blockBlob = container.GetBlockBlobReference(name);

            await blockBlob.DeleteIfExistsAsync();

            log.LogInformation("Deleted original: {0}", name);
        }
    }
}
