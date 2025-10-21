using Course.Application.Interfaces;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Course.Application.Implements
{
    public class FirebaseService : IFirebaseService
    {
        private readonly StorageClient _storageClient;
        private readonly ILogger<FirebaseService> _logger;
        private const string BucketName = "fibochatplatform.firebasestorage.app";

        // Replace the constructor (lines 15-26) with:
        public FirebaseService(IConfiguration configuration, ILogger<FirebaseService> logger)
        {
            var projectId = configuration["Firebase:ProjectId"];
            var privateKey = configuration["Firebase:PrivateKey"];
            var clientEmail = configuration["Firebase:ClientEmail"];

            if (string.IsNullOrWhiteSpace(projectId) || string.IsNullOrWhiteSpace(privateKey) || string.IsNullOrWhiteSpace(clientEmail))
            {
                throw new InvalidOperationException("Firebase configuration is missing. Please set Firebase:ProjectId, Firebase:PrivateKey, and Firebase:ClientEmail in your configuration.");
            }

            var credential = GoogleCredential.FromServiceAccountCredential(new ServiceAccountCredential(new ServiceAccountCredential.Initializer(clientEmail)
            {
                ProjectId = projectId
            }.FromPrivateKey(privateKey)));

            _storageClient = StorageClient.Create(credential);
            _logger = logger;
        }

        public async Task<string> UploadAvatarAsync(Stream imageStream, string fileName)
        {
            var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
            var contentType = GetContentType(fileName);

            try
            {
                var storageObject = await _storageClient.UploadObjectAsync(
                    BucketName,
                    $"avatars/FiboChatPlatform/{uniqueFileName}",
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


                return $"https://storage.googleapis.com/{BucketName}/avatars/FiboChatPlatform/{uniqueFileName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading avatar to Firebase Storage.");
                throw new InvalidOperationException("Error uploading avatar to Firebase Storage.", ex);
            }
        }

        public async Task<string> UploadDocumentAsync(Stream fileStream, string fileName)
        {
            var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
            var contentType = GetContentType(fileName);
            try
            {
                var storageObject = await _storageClient.UploadObjectAsync(
                    BucketName,
                    $"documents/FiboChatPlatform/{uniqueFileName}",
                    contentType,
                    fileStream
                );

                storageObject.Acl = new[] { new Google.Apis.Storage.v1.Data.ObjectAccessControl
                    {
                        Entity = "allUsers",
                        Role = "READER"
                    }
                };

                await _storageClient.UpdateObjectAsync(storageObject);

                return $"https://storage.googleapis.com/{BucketName}/documents/FiboChatPlatform/{uniqueFileName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading document to Firebase Storage.");
                throw new InvalidOperationException("Error uploading document to Firebase Storage.", ex);
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