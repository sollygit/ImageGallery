using ImageGallery.Model;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CoreImageGallery.Interfaces
{
    public interface IStorageService
    {
        Task<IEnumerable<UploadedImage>> GetImagesAsync();
        Task AddImageAsync(Stream stream, string fileName, string userName);
    }
}