using CoreImageGallery.Data;
using CoreImageGallery.Extensions;
using CoreImageGallery.Interfaces;
using ImageGallery.Model;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CoreImageGallery.Services
{
    public class FileStorageService : IStorageService
    {
        private const string ImageFolderUri = "userImages";
        private readonly string ImageFolder = $"wwwroot\\{ImageFolderUri}";
        private readonly ApplicationDbContext _dbContext;

        public FileStorageService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddImageAsync(Stream stream, string originalName, string userName)
        {
            var imageProps = UploadUtilities.GetImageProperties(originalName, userName);
            var localPath = Path.Combine(ImageFolder, imageProps.FileName);
            var imageUri = $"{ImageFolderUri}/{imageProps.FileName}";

            using (var fileStream = File.Create(localPath))
            {
                stream.Seek(0, SeekOrigin.Begin);
                await stream.CopyToAsync(fileStream);
            }

            await UploadUtilities.RecordImageUploadedAsync(_dbContext, imageProps.UploadId, imageProps.FileName, imageUri, imageProps.UserHash);
        }

        public async Task<IEnumerable<UploadedImage>> GetImagesAsync()
        {
            var files = await Task.Run(() => Directory.EnumerateFiles(ImageFolder));
            var imageList = files.Select(file =>
            {
                return new UploadedImage
                {
                    ImagePath = $"{ImageFolderUri}/{Path.GetFileName(file)}"
                };
            }).ToList();

            return imageList;
        }

        public async Task<byte[]> DownloadImageAsync(string host, string id)
        {
            var files = await Task.Run(() => Directory.EnumerateFiles(ImageFolder));
            var filename = files.FirstOrDefault(o => o == $"{ImageFolder}\\{UploadUtilities.ImagePrefix}{id}.png");
            var ImagePath = $"{host}/{ImageFolderUri}/{Path.GetFileName(filename)}";
            using var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, ImagePath);
            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new ServiceException(response.StatusCode, $"Image Id {id} - download error.");
            }

            return await response.Content.ReadAsByteArrayAsync();
        }
    }
}
