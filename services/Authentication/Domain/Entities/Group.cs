namespace Authentication.Domain.Entities
{
    public class Group
    {
        public Guid Id { get; set; }
        public Guid ClassId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Class Class { get; set; } = null!;
        public ICollection<ClassEnrollment> Enrollments { get; set; } = new List<ClassEnrollment>();
    }
}