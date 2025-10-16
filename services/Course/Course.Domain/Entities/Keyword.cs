namespace Course.Domain.Entities
{
    public class Keyword
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Status { get; set; } = "active"; // active | disabled

        // Navigation
        public ICollection<MasterTopicKeyword> MasterTopicKeywords { get; set; } = new List<MasterTopicKeyword>();
    }
}