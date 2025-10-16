namespace Authentication.Application.DTOs.GroupDTOs
{
    public class GroupCreateRequest
    {
        public Guid ClassId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
    }
}