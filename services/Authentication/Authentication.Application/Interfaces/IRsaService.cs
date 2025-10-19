using Microsoft.IdentityModel.Tokens;
namespace Authentication.Application.Interfaces
{
    public interface IRsaService
    {
        string Encrypt(string data);
        string Decrypt(string encryptedData);
        SecurityKey GetRsaSecurityKey();
        SigningCredentials GetSigningCredentials();
    }
}
