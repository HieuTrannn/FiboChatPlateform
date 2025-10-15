namespace Course.Domain.Entities
{
    public class TopicLecturer
    {
        public Guid TopicId { get; set; }
        public Guid LecturerId { get; set; }
        public Topic Topic { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}