namespace Authentication.Application.DTOs.ClassDTOs
{
    public class ClassLectrurerResponse
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public SemesterResponse Semester { get; set; }
    }
}