using Authentication.Application.Interfaces;
using Google.Cloud.Storage.V1;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Authentication.Application.Services
{
    public class FirebaseService : IFirebaseService
    {
        private readonly StorageClient _storageClient;
        private readonly ILogger<FirebaseService> _logger;
        private const string BucketName = "fibochatplatform.firebasestorage.app";

        public FirebaseService(IConfiguration configuration, ILogger<FirebaseService> logger)
        {
            var firebaseConfigPath = configuration["Firebase:CredentialsPath"];
            
            if (string.IsNullOrWhiteSpace(firebaseConfigPath))
            {
                throw new InvalidOperationException("Firebase:CredentialsPath configuration is missing. Please set the path to your Firebase service account JSON file.");
            }

            try
            {
                // Đọc trực tiếp từ file JSON
                var credential = GoogleCredential.FromFile(firebaseConfigPath);
                _storageClient = StorageClient.Create(credential);
                _logger = logger;
                _logger.LogInformation("Firebase Storage client initialized successfully from file: {FilePath}", firebaseConfigPath);
            }
            catch (FileNotFoundException)
            {
                _logger.LogError("Firebase service account file not found: {FilePath}", firebaseConfigPath);
                throw new InvalidOperationException($"Firebase service account file not found: {firebaseConfigPath}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize Firebase Storage client from file: {FilePath}", firebaseConfigPath);
                throw new InvalidOperationException($"Failed to initialize Firebase Storage client: {ex.Message}", ex);
            }
        }

        public async Task<string> UploadAvatarAsync(Stream imageStream, string fileName)
        {
            var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
            var contentType = GetContentType(fileName);

            try
            {
                var storageObject = await _storageClient.UploadObjectAsync(
                    BucketName,
                    $"avatars/Accounts/{uniqueFileName}",
                    contentType,
                    imageStream
                );

                storageObject.Acl = new[] { new Google.Apis.Storage.v1.Data.ObjectAccessControl
                    {
                        Entity = "allUsers",
                        Role = "READER"
                    }
                };

                await _storageClient.UpdateObjectAsync(storageObject);

                return $"https://storage.googleapis.com/{BucketName}/avatars/Accounts/{uniqueFileName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading avatar to Firebase Storage.");
                throw new InvalidOperationException("Error uploading avatar to Firebase Storage.", ex);
            }
        }

        private string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".jpg" => "image/jpg",
                ".jpeg" => "image/jpeg",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".txt" => "text/plain",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".tiff" => "image/tiff",
                ".pdf" => "application/pdf",
                _ => "application/octet-stream",
            };
        }
    }
}