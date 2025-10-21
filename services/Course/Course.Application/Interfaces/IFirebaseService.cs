namespace Course.Application.Interfaces
{
    public interface IFirebaseService
    {
        public Task<string> UploadAvatarAsync(Stream imageStream, string fileName);
        public Task<string> UploadDocumentAsync(Stream fileStream, string fileName);
    }
}