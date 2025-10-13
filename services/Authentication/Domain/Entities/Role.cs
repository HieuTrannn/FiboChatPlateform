using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.Domain.Entities
{
    public class Role : BaseEntity
    {
        public string RoleName { get; set; } = null!;

        public string? Description { get; set; }
        public virtual ICollection<Account> Users { get; set; } = new List<Account>();
    }
}
