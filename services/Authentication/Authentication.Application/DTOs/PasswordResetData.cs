
namespace Authentication.Application.DTOs
{
    public class PasswordResetData
    {
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
    public class ResetPasswordRequest
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }

}
