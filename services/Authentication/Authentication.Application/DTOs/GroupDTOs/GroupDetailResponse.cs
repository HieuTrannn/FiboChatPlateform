namespace Authentication.Application.DTOs.GroupDTOs
{
    public class GroupDetailResponse : GroupResponse
    {
        public string ClassCode { get; set; } = null!;
        public List<GroupMemberResponse> GroupMembers { get; set; } = new List<GroupMemberResponse>();
    }

    public class GroupMemberResponse
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string StudentId { get; set; } = null!;
        public string RoleInClass { get; set; } = null!;
        public string Status { get; set; } = null!;
    }
}