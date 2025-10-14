using Authentication.Application.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;
namespace Authentication.Infrastructure.Implementation
{
    public class RsaService : IRsaService
    {
        private readonly RSA _rsa;
        private readonly string _privateKeyPath;
        private readonly string _publicKeyPath;

        public RsaService(string privateKeyPath = "Key/private_key.rsa", string publicKeyPath = "Key/public_key.rsa")
        {
            _privateKeyPath = privateKeyPath;
            _publicKeyPath = publicKeyPath;
            _rsa = RSA.Create();
            LoadExistingKey();
        }

        private void LoadExistingKey()
        {
            if (!File.Exists(_privateKeyPath))
                throw new FileNotFoundException("Private key file not found.", _privateKeyPath);

            try
            {
                var pemPrivate = File.ReadAllText(_privateKeyPath);
                _rsa.ImportFromPem(pemPrivate.ToCharArray());

                if (File.Exists(_publicKeyPath))
                {
                    var pemPublic = File.ReadAllText(_publicKeyPath);
                    _rsa.ImportFromPem(pemPublic.ToCharArray());
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to load RSA PEM key.", ex);
            }
        }


        public string Encrypt(string data)
        {
            if (string.IsNullOrEmpty(data))
                throw new ArgumentNullException(nameof(data));

            try
            {
                byte[] dataBytes = Encoding.UTF8.GetBytes(data);
                byte[] encryptedBytes = _rsa.Encrypt(dataBytes, RSAEncryptionPadding.OaepSHA256);
                return Convert.ToBase64String(encryptedBytes);
            }
            catch (CryptographicException ex)
            {
                throw new InvalidOperationException("Encryption failed.", ex);
            }
        }

        public string Decrypt(string encryptedData)
        {
            if (string.IsNullOrEmpty(encryptedData))
                throw new ArgumentNullException(nameof(encryptedData));

            try
            {
                byte[] encryptedBytes = Convert.FromBase64String(encryptedData);
                byte[] decryptedBytes = _rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.OaepSHA256);
                return Encoding.UTF8.GetString(decryptedBytes);
            }
            catch (FormatException ex)
            {
                throw new ArgumentException("Invalid Base64 string.", nameof(encryptedData), ex);
            }
            catch (CryptographicException ex)
            {
                throw new InvalidOperationException("Decryption failed.", ex);
            }
        }

        // public SecurityKey GetRsaSecurityKey()
        // {
        //     return new RsaSecurityKey(_rsa);
        // }
        public SecurityKey GetRsaSecurityKey()
        {
            // Create a new RSA instance with the same parameters
            var rsaParams = _rsa.ExportParameters(true);
            var newRsa = RSA.Create();
            newRsa.ImportParameters(rsaParams);

            // Return a new security key with the cloned RSA instance
            return new RsaSecurityKey(newRsa);
        }

        public SigningCredentials GetSigningCredentials()
        {
            var securityKey = GetRsaSecurityKey();
            return new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256);
        }

        public void Dispose()
        {
            _rsa?.Dispose();
        }
    }
}
