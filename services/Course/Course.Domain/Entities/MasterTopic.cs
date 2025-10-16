using System.ComponentModel.DataAnnotations.Schema;
using Authentication.Domain.Entities;

namespace Course.Domain.Entities
{
    public class MasterTopic
    {
        public Guid Id { get; set; }
        public Guid DomainId { get; set; }
        public Guid SemesterId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Status { get; set; } = "active"; // active | disabled
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        [ForeignKey("SemesterId")]
        public Semester Semester { get; set; } = null!;
        public ICollection<Topic> Topics { get; set; } = new List<Topic>();
        public ICollection<MasterTopicKeyword> MasterTopicKeywords { get; set; } = new List<MasterTopicKeyword>();
        [ForeignKey("DomainId")]
        public Domain Domain { get; set; } = null!;
        public ICollection<LecturerMasterTopic> LecturerMasterTopics { get; set; } = new List<LecturerMasterTopic>();
    }
}