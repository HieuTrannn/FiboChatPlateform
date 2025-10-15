using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.Domain.Entities
{
    public class Class : BaseEntity
    {
        public string ClassName { get; set; } = null!;
        // Foreign keys
        public Guid LectureId { get; set; }
        public Guid SemesterId { get; set; }

        // Navigation properties
        [ForeignKey("LectureId")]
        public virtual Lecture Lecture { get; set; } = null!;

        [ForeignKey("SemesterId")]
        public virtual Semester Semester { get; set; } = null!;
        public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    }

}
