using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Entities
{
    public class ClassEnrollment
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid ClassId { get; private set; }
        public Guid UserId { get; private set; }
        public string Status { get; private set; } = "active";
        public string RoleInClass { get; private set; } = "student"; // student|lecturer|ta
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        private ClassEnrollment() { }
        public ClassEnrollment(Guid classId, Guid userId, string roleInClass = "student")
        { ClassId = classId; UserId = userId; RoleInClass = roleInClass; }
    }
}
