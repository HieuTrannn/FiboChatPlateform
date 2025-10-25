namespace Course.Application.Interfaces
{
    public interface IFirebaseService
    {
        public Task<string> UploadDocumentAsync(Stream fileStream, string fileName);
    }
}