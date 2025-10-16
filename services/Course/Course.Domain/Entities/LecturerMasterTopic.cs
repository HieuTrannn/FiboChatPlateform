namespace Course.Domain.Entities
{
    public class LecturerMasterTopic
    {
        public Guid Id { get; set; }
        public Guid LecturerId { get; set; }
        public Guid MasterTopicId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // navigation
        public MasterTopic MasterTopic { get; set; } = null!;
    }
}