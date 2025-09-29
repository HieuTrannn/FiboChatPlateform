using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Entities
{
    public class ClassEnrollment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ClassId { get; set; }
        public Guid UserId { get; set; }
        public string Status { get; set; } = "active";
        public string RoleInClass { get; set; } = "student"; // student|lecturer|ta
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        private ClassEnrollment() { }
        public ClassEnrollment(Guid classId, Guid userId, string roleInClass = "student")
        { ClassId = classId; UserId = userId; RoleInClass = roleInClass; }
    }
}
