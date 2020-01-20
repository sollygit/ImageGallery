using System;
using System.IO;
using ImageGallery.Common;
using ImageGallery.Model;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace ImageGallery.AzFunctions
{
    public static class ImageUploaded
    {
        [FunctionName("ImageUploaded")]
        public static void Run([BlobTrigger("images/{name}")]Stream inputBlob,
                               [Blob("watermarked/{name}", FileAccess.Write)] Stream outputBlob,
                               string name,
                               ILogger log)
        {
            if (!outputBlob.CanWrite)
            {
                log.LogError($"{Config.WatermarkedContainer} Blob has no Write access");
                return;
            }
            try
            {
                WaterMarker.Generate(inputBlob, outputBlob);
                log.LogInformation($"Image:{name}, {inputBlob.Length} bytes");
            }
            catch (Exception e)
            {
                log.LogError($"Watermaking failed {e.Message}");
            }
        }
    }

}
