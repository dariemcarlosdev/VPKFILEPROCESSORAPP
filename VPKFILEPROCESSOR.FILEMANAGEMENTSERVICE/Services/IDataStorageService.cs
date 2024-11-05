using VPKFILEPROCESSOR.SHARED.Services.ResponseService;

namespace VPKFILEPROCESSOR.FILEMANAGEMENTSERVICE.Services
{
    //Data Storage Abstraction (Adheres to Interface Segregation Principle and Dependecy Inversion Principles)
    public interface IDataStorageService
    {
        Task<bool> DeleteFileAsync(string fileName);
        Task<Stream> DownloadFileAsync(string fileName);
        Task<ResponseService<FileResponse>> UploadFileAsync(string fileName, Stream fileStream); 
        Task<bool> FileExistAsync(string fileName);
    }
}