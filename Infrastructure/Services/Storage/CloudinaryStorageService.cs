using Application.Interfaces.External;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services.Storage
{
    public class CloudinaryStorageService : IStorageService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryStorageService(IConfiguration configuration) 
        {
            var account = new Account(
                configuration["Cloudinary:CloudName"],
                configuration["Cloudinary:ApiKey"],
                configuration["Cloudinary:ApiSecret"]
            );

            _cloudinary = new Cloudinary(account);
        }

        public async Task DeleteAsync(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl))
                return;

            var publicId = GetPublicIdFromUrl(fileUrl);

            if (!string.IsNullOrEmpty(publicId))
                await _cloudinary.DestroyAsync(new DeletionParams(publicId));
        }

        public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType)
        {
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(fileName, fileStream),
                UseFilename = true,
                UniqueFilename = true,
                Overwrite = false,
                Folder = "naijarescue"
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
                throw new Exception($"Cloudinary upload failed: {uploadResult.Error?.Message}");

            return uploadResult.SecureUrl.AbsoluteUri;
        }

        private string GetPublicIdFromUrl(string url)
        {
            var uri = new Uri(url);
            var parts = uri.AbsolutePath.Split('/');
            var fileWithExtension = parts.Last();
            return Path.Combine(parts[parts.Length - 2], Path.GetFileNameWithoutExtension(fileWithExtension)).Replace("\\", "/");
        }
    }
}
