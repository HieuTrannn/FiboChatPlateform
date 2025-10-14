using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.Domain.Entities
{
    public class Class : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string ClassName { get; set; }

        [MaxLength(255)]
        public string? Description { get; set; }

        public Guid SemesterId { get; set; }

        public Semester Semester { get; set; }

        public Guid? HomeroomTeacherId { get; set; }
        
        public Account? HomeroomTeacher { get; set; }

        public ICollection<Account> Students { get; set; } = new List<Account>();
    }

}
