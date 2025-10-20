namespace Authentication.Application.DTOs.ClassDTOs
{
    public class ClassCreateRequest
    {
        public Guid SemesterId { get; set; }
        public string Code { get; set; } = null!;
        public Guid? LecturerId { get; set; }
    }
}