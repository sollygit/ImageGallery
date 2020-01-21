using CoreImageGallery.Data;
using ImageGallery.Model;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CoreImageGallery.Extensions
{
    public class UploadUtilities
    {
        public static string ImagePrefix = "img_";

        public static ImageProperties GetImageProperties(string originalName, string userName)
        {
            var uploadId = Guid.NewGuid().ToString();
            var fileName = $"{ImagePrefix}{uploadId}{Path.GetExtension(originalName)}";
            var userHash = userName?.GetHashCode().ToString();
            
            return new ImageProperties(uploadId, fileName, userHash);
        }

        public static async Task RecordImageUploadedAsync(ApplicationDbContext dbContext, string uploadId, string fileName, string imageUri, string userHash = null)
        {
            var img = new UploadedImage
            {
                Id = uploadId,
                FileName = fileName,
                ImagePath = imageUri,
                UploadTime = DateTime.Now,
                UserHash = userHash
            };

            await dbContext.Images.AddAsync(img);
            await dbContext.SaveChangesAsync();
        }
    }
}
