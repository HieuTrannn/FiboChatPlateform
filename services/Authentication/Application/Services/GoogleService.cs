using Authentication.Application.Interfaces;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using static Authentication.Application.DTOs.AuthenDTO;

namespace Authentication.Application.Services
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly ILogger<GoogleAuthService> _logger;
        private readonly IConfiguration _config;

        public GoogleAuthService(ILogger<GoogleAuthService> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;

            if (FirebaseApp.DefaultInstance == null)
            {
                var credentialPath = _config["Firebase:CredentialsPath"];
                if (string.IsNullOrEmpty(credentialPath))
                    throw new Exception("Firebase credentials path not found in configuration");

                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.FromFile(credentialPath)
                });
                _logger.LogInformation("FirebaseApp initialized successfully.");
            }
        }

        public async Task<GoogleUserInfo?> VerifyGoogleTokenAsync(string idToken)
        {
            try
            {
                FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
                var claims = decodedToken.Claims;

                var userInfo = new GoogleUserInfo
                {
                    Uid = decodedToken.Uid,
                    Email = claims.ContainsKey("email") ? claims["email"].ToString() : string.Empty,
                    Name = claims.ContainsKey("name") ? claims["name"].ToString() : string.Empty,
                    Picture = claims.ContainsKey("picture") ? claims["picture"].ToString() : string.Empty
                };

                return userInfo;
            }
            catch (FirebaseAuthException ex)
            {
                _logger.LogError("Invalid Google token: {Message}", ex.Message);
                return null;
            }
        }
    }
}