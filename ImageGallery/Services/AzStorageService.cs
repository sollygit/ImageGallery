using CoreImageGallery.Data;
using CoreImageGallery.Extensions;
using CoreImageGallery.Interfaces;
using ImageGallery.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CoreImageGallery.Services
{
    public class AzStorageService : IStorageService
    {
        private readonly string _connectionString;
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
            var imageProps = UploadUtilities.GetImageProperties(originalName, userName);
            var imageBlob = _imagesContainer.GetBlockBlobReference(imageProps.FileName);
            
            await imageBlob.UploadFromStreamAsync(stream);
            await UploadUtilities.RecordImageUploadedAsync(_dbContext, imageProps.UploadId, imageProps.FileName, imageBlob.Uri.ToString(), imageProps.UserHash);
        }

        public async Task<IEnumerable<UploadedImage>> GetImagesAsync()
        {
            var token = new BlobContinuationToken();
            var blobList = await _watermarkedContainer.ListBlobsSegmentedAsync(UploadUtilities.ImagePrefix, true, BlobListingDetails.All, 100, token, null, null);
            var imageList = blobList.Results.Select(blob =>
            {
                return new UploadedImage
                {
                    ImagePath = blob.Uri.ToString()
                };
            });

            return imageList;
        }
    }
}
