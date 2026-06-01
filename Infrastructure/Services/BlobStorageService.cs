using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services
{
    public class BlobStorageService(IConfiguration configuration) : IBlobStorageService
    {
        private readonly string _connectionString = configuration.GetConnectionString("AzureBlobStorage")
                                                    ?? throw new InvalidOperationException("Azure Blob Storage connection string is not configured.");

        private readonly string _containerName = "trainingevents";

        public async Task<string> UploadSignatureAsync(string base64Image, string fileName)
        {
            if (string.IsNullOrWhiteSpace(base64Image))
            {
                return string.Empty;
            }

            var base64Data = base64Image.Contains(',')
                    ? base64Image.Split(',')[1]
                    : base64Image;

            byte[] imageBytes = Convert.FromBase64String(base64Data);

            var blobServiceClient = new BlobServiceClient(_connectionString);
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(_containerName);

            await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var uniqueFileName = $"{Guid.NewGuid()} - {fileName}.png";
            var blobClient = blobContainerClient.GetBlobClient(uniqueFileName);

            using var stream = new MemoryStream(imageBytes);
            var blobHttpHeaders = new BlobHttpHeaders { ContentType = "image/png" };

            await blobClient.UploadAsync(stream, new BlobUploadOptions { HttpHeaders = blobHttpHeaders });

            return blobClient.Uri.ToString();
        }

        public async Task<string> UploadFileAsync(IFormFile file, string fileName)
        {
            if (file == null || file.Length == 0)
            {
                return string.Empty;
            }

            var blobServiceClient = new BlobServiceClient(_connectionString);
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(_containerName);

            await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var blobClient = blobContainerClient.GetBlobClient(fileName);

            using var stream = file.OpenReadStream();
            var blobHttpHeaders = new BlobHttpHeaders { ContentType = file.ContentType ?? "image/png" };

            await blobClient.UploadAsync(stream, new BlobUploadOptions { HttpHeaders = blobHttpHeaders });

            return blobClient.Uri.ToString();
        }
    }
}