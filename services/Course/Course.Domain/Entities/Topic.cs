namespace Course.Domain.Entities
{
    public class Topic
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string Status { get; set; } = "active"; // active | disabled
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<Semester> Semesters { get; set; } = new List<Semester>();
        public ICollection<TopicLecturer> TopicLecturers { get; set; } = new List<TopicLecturer>();
    }
}
