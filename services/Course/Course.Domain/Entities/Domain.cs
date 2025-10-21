namespace Course.Domain.Entities
{
    public class Domain
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Status { get; set; } = "active"; // active | disabled
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}