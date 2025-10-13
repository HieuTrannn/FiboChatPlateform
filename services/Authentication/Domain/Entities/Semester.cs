using System.ComponentModel.DataAnnotations;

namespace Authentication.Domain.Entities
{
    public class Semester : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }  

        [Required]
        public int Year { get; set; }

        public bool IsActive { get; set; } 

        public ICollection<Class> Classes { get; set; } = new List<Class>();
    }

}
