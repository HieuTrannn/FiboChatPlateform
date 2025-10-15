
namespace Authentication.Application.DTOs
{
    public class PasswordResetData
    {
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
    }

}
