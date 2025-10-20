namespace Course.Application.DTOs.MasterTopicDTOs
{
    public class MasterTopicCreateRequest
    {
        public Guid DomainId { get; set; }
        public Guid SemesterId { get; set; }
        public Guid? LecturerId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
    }
}