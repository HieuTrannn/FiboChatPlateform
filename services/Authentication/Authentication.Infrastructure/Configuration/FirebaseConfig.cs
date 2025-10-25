using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

namespace Authentication.Infrastructure.Configuration
{
    public class FirebaseConfig
    {
        private static bool _isInitialized = false;

        public static void InitializeFirebase(string jsonPath)
        {
            if (!_isInitialized)
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(jsonPath),
                });

                _isInitialized = true;
            }
        }
    }
}