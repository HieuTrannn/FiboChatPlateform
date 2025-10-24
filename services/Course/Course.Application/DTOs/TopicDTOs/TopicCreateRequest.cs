namespace Course.Application.DTOs.TopicDTOs
{
    public class TopicCreateRequest
    {
        public Guid MasterTopicId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }
}