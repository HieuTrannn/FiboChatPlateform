using Authentication.Domain.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Authentication.Domain.Entities
{
    public class Lecture
    {

        [Key]
        [ForeignKey("Account")]
        public Guid LecturerId { get; set; }
        public string? FullName { get; set; }
        public Gender Gender { get; set; }
        public string Status { get; set; }
        public virtual Account Account { get; set; } = null!;
        public virtual ICollection<Class> Classes { get; set; } = new List<Class>();
    }
}
