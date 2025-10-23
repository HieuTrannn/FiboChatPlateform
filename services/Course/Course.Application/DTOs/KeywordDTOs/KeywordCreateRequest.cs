namespace Course.Application.DTOs.KeywordDTOs
{
    public class KeywordCreateRequest
    {
        public string Name { get; set; } = null!;
        public List<Guid> MasterTopicIds { get; set; } = new List<Guid>();
    }
}