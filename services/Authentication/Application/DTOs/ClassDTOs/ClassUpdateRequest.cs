namespace Authentication.Application.DTOs.ClassDTOs
{
    public class ClassUpdateRequest
    {
        public Guid? SemesterId { get; set; }
        public string? Code { get; set; } = null;
        public Guid? LecturerId { get; set; }
    }
}