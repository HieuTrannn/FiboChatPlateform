namespace Course.Application.DTOs.MasterTopicDTOs
{
    public class MasterTopicCreateRequest
    {
        public Guid DomainId { get; set; }
        public Guid SemesterId { get; set; }
        public List<Guid>? LecturerIds { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
    }
}