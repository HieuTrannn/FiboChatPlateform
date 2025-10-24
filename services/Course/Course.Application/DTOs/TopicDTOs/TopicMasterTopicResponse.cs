using Contracts.Common;

namespace Course.Application.DTOs.TopicDTOs
{
    public class TopicMasterTopicResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public StaticEnum.StatusEnum Status { get; set; } = StaticEnum.StatusEnum.Active; // active | disabled
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<TopicResponse> Topics { get; set; } = new List<TopicResponse>();
    }
}