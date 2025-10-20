using System.ComponentModel.DataAnnotations;
namespace Authentication.Application.DTOs.GroupDTOs
{
    public class GroupMemberRequest
    {
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public string RoleInClass { get; set; } = null!;
    }
}