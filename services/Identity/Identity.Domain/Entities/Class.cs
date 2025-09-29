namespace Identity.Domain.Entities
{
    public class Class
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid SemesterId { get; set; }
        public string Code { get; set; } = default!;
        public string? Name { get; set; }
        public string Status { get; set; } = "active";
        public Guid? LecturerId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        private Class() { }
        public Class(Guid semesterId, string code, string? name, Guid? lecturerId)
        { SemesterId = semesterId; Code = code; Name = name; LecturerId = lecturerId; }
    }
}
