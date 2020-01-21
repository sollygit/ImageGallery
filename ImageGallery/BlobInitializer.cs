using CoreImageGallery.Data;
using ImageGallery.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Threading.Tasks;

namespace ImageGallery
{
    public class BlobInitializer
    {
        private static bool ResourcesInitialized { get; set; } = false;
        private string _connectionString;
        private CloudStorageAccount _account;
        private CloudBlobClient _client;
        private CloudBlobContainer _imagesContainer;
        private CloudBlobContainer _watermarkedContainer;

        public async Task InitAsync(IConfiguration config, ApplicationDbContext dbContext)
        {
            _connectionString = config["AzureStorageConnection"];
            _account = CloudStorageAccount.Parse(_connectionString);
            _client = _account.CreateCloudBlobClient();
            _imagesContainer = _client.GetContainerReference(Config.ImagesContainer);
            _watermarkedContainer = _client.GetContainerReference(Config.WatermarkedContainer);

            await InitResourcesAsync(dbContext);
        }

        public async Task InitResourcesAsync(ApplicationDbContext dbContext)
        {
            if (!ResourcesInitialized)
            {
                // Init DB
                dbContext.Database.Migrate();
                dbContext.Database.EnsureCreated();

                // Init Azure Storage resources
                await _watermarkedContainer.CreateIfNotExistsAsync();
                await _imagesContainer.CreateIfNotExistsAsync();

                var permissions = await _watermarkedContainer.GetPermissionsAsync();

                if (permissions.PublicAccess == BlobContainerPublicAccessType.Off ||
                    permissions.PublicAccess == BlobContainerPublicAccessType.Unknown)
                {
                    // If blob isn't public, we can't directly link to the pictures
                    await _watermarkedContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
                }

                ResourcesInitialized = true;
            }
        }
    }
}
