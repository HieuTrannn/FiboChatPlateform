using Authentication.Domain.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Authentication.Domain.Entities
{
    public class Account : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Firstname { get; set; }
        [Required]
        [MaxLength(100)]
        public string Lastname { get; set; }

        [Column(TypeName = "text")]
        public string Password { get; set; }

        public DateOnly? DateOfBirth { get; set; }
        public Gender? Sex { get; set; }
        [Required]
        [MaxLength(255)]
        public string Email { get; set; }

        [MaxLength(10)]
        public string? PhoneNumber { get; set; }
        public string? StudentID { get; set; }
        public Guid? RoleId { get; set; }
        public string? Address { get; set; }
        [Column(TypeName = "text")]
        public bool IsVerified { get; set; }
        public virtual Role? Role { get; set; }

        public Guid? ClassId { get; set; }

        [ForeignKey("ClassId")]
        public virtual Class? Class { get; set; }
        public virtual Lecturer? Lecture { get; set; }

        //public bool IsEmailVerified { get; set; }

        //public AccountStatus AccountStatus { get; set; }\
    }
}
