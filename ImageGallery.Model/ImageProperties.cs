namespace ImageGallery.Model
{
    public class ImageProperties
    {
        public string UploadId { get; set; }
        public string FileName { get; set; }
        public string UserHash { get; set; }

        public ImageProperties(string uploadId, string fileName, string userHash)
        {
            UploadId = uploadId;
            FileName = fileName;
            UserHash = userHash;
        }
    }
}
