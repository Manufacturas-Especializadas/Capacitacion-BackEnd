using Microsoft.AspNetCore.Http;

namespace Domain.Interfaces
{
    public interface IBlobStorageService
    {
        Task<string> UploadSignatureAsync(string base64Image, string fileName);

        Task<string> UploadFileAsync(IFormFile file, string fileName);

        Task<string> UploadFileWeldersAsync(IFormFile file, string fileName);

        Task<string> UploadFileTrainingReportAsync(IFormFile file, string fileName);

        Task DeleteFileTrainingReportAsync(string fileUrl);

        Task DeleteFileWeldersAsync(string fileUrl);
    }
}