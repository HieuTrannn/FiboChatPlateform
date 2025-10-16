using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Authentication.Domain.Entities
{
    public class Group
    {
        public Guid Id { get; set; }
        public Guid ClassId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;
        [Required]
        [MaxLength(255)]
        public string Description { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        [ForeignKey("ClassId")]
        public Class Class { get; set; } = null!;
        public ICollection<ClassEnrollment> Enrollments { get; set; } = new List<ClassEnrollment>();
    }
}