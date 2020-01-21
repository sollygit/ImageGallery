using CoreImageGallery.Interfaces;
using ImageGallery.Model;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreImageGallery.Pages
{
    public class IndexModel : PageModel
    {
        public IEnumerable<UploadedImage> Images;

        private readonly IStorageService _storageService;

        public IndexModel(IStorageService storageService)
        {
            _storageService = storageService;
        }

        public async Task OnGetAsync()
        {
            this.Images = await _storageService.GetImagesAsync();
        }

    }
}
