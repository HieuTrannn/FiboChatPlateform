namespace Authentication.Application.DTOs
{
    public class ChangePasswordRequest
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }

    public class ChangePasswordFirstTimeRequest
    {
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }
}
