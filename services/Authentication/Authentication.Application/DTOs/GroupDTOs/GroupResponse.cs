namespace Authentication.Application.DTOs.GroupDTOs
{
    public class GroupResponse
    {
        public Guid Id { get; set; }
        public Guid ClassId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}