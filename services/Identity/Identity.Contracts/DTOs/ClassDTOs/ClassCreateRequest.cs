namespace Identity.Contracts.DTOs.ClassDTOs
{
    public class ClassCreateRequest
    {
        public Guid SemesterId { get; set; }
        public string Code { get; set; }
        public string? Name { get; set; }
        public Guid? LecturerId { get; set; }
    }
}