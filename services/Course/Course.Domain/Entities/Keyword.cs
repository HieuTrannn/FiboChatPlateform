using Contracts.Common;

namespace Course.Domain.Entities
{
    public class Keyword
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public StaticEnum.StatusEnum Status { get; set; } = StaticEnum.StatusEnum.Active; // active | disabled
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<MasterTopicKeyword> MasterTopicKeywords { get; set; } = new List<MasterTopicKeyword>();
    }
}