using CoreImageGallery.Data;
using CoreImageGallery.Extensions;
using ImageGallery.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CoreImageGallery.Services
{
    public class AzStorageService : IStorageService
    {
        private readonly string _connectionString;
        private const string ImagePrefix = "img_";
        private readonly CloudStorageAccount _account;
        private readonly CloudBlobClient _client;
        private readonly CloudBlobContainer _imagesContainer;
        private readonly CloudBlobContainer _watermarkedContainer;
        private readonly ApplicationDbContext _dbContext;

        public AzStorageService(IConfiguration config, ApplicationDbContext dbContext)
        {
            _connectionString = config["AzureStorageConnection"];
            _account = CloudStorageAccount.Parse(_connectionString);
            _client = _account.CreateCloudBlobClient();
            _imagesContainer = _client.GetContainerReference(Config.ImagesContainer);
            _watermarkedContainer = _client.GetContainerReference(Config.WatermarkedContainer);
            _dbContext = dbContext;
        }

        public async Task AddImageAsync(Stream stream, string originalName, string userName)
        {
            UploadUtilities.GetImageProperties(originalName, userName, out string uploadId, out string fileName, out string userId);

            var imageBlob = _imagesContainer.GetBlockBlobReference(fileName);
            await imageBlob.UploadFromStreamAsync(stream);

            await UploadUtilities.RecordImageUploadedAsync(_dbContext, uploadId, fileName, imageBlob.Uri.ToString(), userId);
        }

        public async Task<IEnumerable<UploadedImage>> GetImagesAsync()
        {
            var imageList = new List<UploadedImage>();
            var token = new BlobContinuationToken();
            var blobList = await _watermarkedContainer.ListBlobsSegmentedAsync(ImagePrefix, true, BlobListingDetails.All, 100, token, null, null);

            foreach (var blob in blobList.Results)
            {
                var image = new UploadedImage
                {
                    ImagePath = blob.Uri.ToString()
                };

                imageList.Add(image);
            }

            return imageList;
        }
    }
}
