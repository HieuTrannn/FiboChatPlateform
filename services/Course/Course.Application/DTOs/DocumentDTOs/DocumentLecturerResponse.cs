namespace Course.Application.DTOs.DocumentDTOs
{
    public class DocumentLecturerResponse
    {
        public Guid LecturerId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public string? Gender { get; set; }
        public List<DocumentResponse> Documents { get; set; } = new List<DocumentResponse>();
    }
}