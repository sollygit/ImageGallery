using CoreImageGallery.Extensions;
using CoreImageGallery.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CoreImageGallery.Controllers
{
    [Route("[controller]")]
    public class ImageController : Controller
    {
        private readonly IStorageService _storageService;

        public ImageController(IStorageService storageService)
        {
            _storageService = storageService;
        }

        [HttpGet("{id}/Download")]
        public async Task<IActionResult> Download([FromRoute] string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("Image Id is required");

            var host = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            var content = await _storageService.DownloadImageAsync(host, id);

            return File(content, "application/octet-stream", $"{UploadUtilities.ImagePrefix}{id}.png");
        }
    }
}
