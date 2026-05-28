namespace Domain.Interfaces
{
    public interface IBlobStorageService
    {
        Task<string> UploadSignatureAsync(string base64Image, string fileName);
    }
}