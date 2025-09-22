using Application.Interfaces.External;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services.Storage
{
    public class GoogleDriveService : IStorageService
    {
        private readonly DriveService _driveService;
        private readonly string _folderId;

        public GoogleDriveService(IConfiguration configuration)
        {
            var credential = GoogleCredential.FromFile(
                Path.Combine(AppContext.BaseDirectory, "Configs", "naijarescue-5b972-d34266fbdbca.json")
            ).CreateScoped(DriveService.ScopeConstants.Drive);

            _driveService = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "NaijaRescue"
            });

            _folderId = configuration["GoogleDrive:FolderId"]
                        ?? throw new Exception("Google Drive FolderId is not configured");
        }

        public async Task DeleteAsync(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl))
                throw new ArgumentException("File URL cannot be null or empty", nameof(fileUrl));

            string fileId;

            if (fileUrl.Contains("id="))
            {
                fileId = fileUrl.Split("id=")[1];
            }
            else if (fileUrl.Contains("/d/"))
            {
                var parts = fileUrl.Split("/d/");
                fileId = parts[1].Split('/')[0];
            }
            else
            {
                throw new InvalidOperationException("Could not extract fileId from URL");
            }

            await _driveService.Files.Delete(fileId).ExecuteAsync();
        }

        public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType)
        {
            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = fileName,
                Parents = new List<string> { _folderId }
            };

            var request = _driveService.Files.Create(fileMetadata, fileStream, contentType);
            request.Fields = "id";
            var result = await request.UploadAsync();

            if (result.Status != UploadStatus.Completed)
                throw new Exception($"File upload failed: {result.Exception?.Message}");

            var file = request.ResponseBody;

            await _driveService.Permissions.Create(
                new Google.Apis.Drive.v3.Data.Permission()
                {
                    Type = "anyone",
                    Role = "reader"
                },
                file.Id
            ).ExecuteAsync();

            return $"https://drive.google.com/uc?id={file.Id}";
        }
    }
}
