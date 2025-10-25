namespace Authentication.Application.Interfaces
{
    public interface IFirebaseService
    {
        public Task<string> UploadAvatarAsync(Stream imageStream, string fileName);
    }
}