namespace Course.Domain.Entities
{
    public class ClassEnrollment
    {
        public Guid Id { get; set; }
        public Guid ClassId { get; set; }
        public Guid UserId { get; set; }
        public string Status { get; set; } = "active";     // active | disabled
        public string RoleInClass { get; set; } = "student"; // student | lecturer | ta
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Class Class { get; set; } = null!;
    }
}
