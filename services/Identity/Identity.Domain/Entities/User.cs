namespace Identity.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Email { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Role { get; set; } = "user"; // user|lecturer|admin
        public string Status { get; set; } = "active"; // active|disabled
        public string? Cohort { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        private User() { }
        public User(string email, string name, string role = "user")
        { Email = email; Name = name; Role = role; }
    }
}
