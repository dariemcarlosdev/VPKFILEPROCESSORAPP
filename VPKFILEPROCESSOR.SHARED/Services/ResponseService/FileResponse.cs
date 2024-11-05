namespace VPKFILEPROCESSOR.SHARED.Services.ResponseService
{
    /// <summary>
    /// File Response class is created to return the file name and file URL after processing the file.
    /// </summary>
    public class FileResponse
    {
        public string BlobFileName { get; set; }
        public string BlobFileUrl { get; set; }

    }
}