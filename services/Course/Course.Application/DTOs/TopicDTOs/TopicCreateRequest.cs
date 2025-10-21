namespace Course.Application.DTOs.TopicDTOs
{
    public class TopicCreateRequest
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }
}