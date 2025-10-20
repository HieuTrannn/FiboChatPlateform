using Contracts.Common;

namespace Course.Application.DTOs.KeywordDTOs
{
    public class KeywordResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public StaticEnum.StatusEnum Status { get; set; } = StaticEnum.StatusEnum.Active; // active | disabled
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}