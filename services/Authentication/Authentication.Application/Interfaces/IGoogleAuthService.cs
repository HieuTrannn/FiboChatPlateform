
using static Authentication.Application.DTOs.AuthenDTO;

namespace Authentication.Application.Interfaces
{
    public interface IGoogleAuthService
    {
        Task<GoogleUserInfo?> VerifyGoogleTokenAsync(string idToken);
    }
}
