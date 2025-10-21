namespace Course.Application.DTOs.TopicDTOs
{
    public class TopicUpdateRequest
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }
}