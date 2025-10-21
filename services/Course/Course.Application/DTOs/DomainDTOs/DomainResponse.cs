using Contracts.Common;

namespace Course.Application.DTOs.DomainDTOs
{
    public class DomainResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public StaticEnum.StatusEnum Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}