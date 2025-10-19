
namespace Authentication.Application.DTOs
{
    public class UserDTO
    {
        public class UserInfo
        {
            public string Firstname { get; set; }
            public string Lastname { get; set; }
            public string Email { get; set; }
            public string? PhoneNumber { get; set; }
            public string? StudentID { get; set; }
        }
    }
}
