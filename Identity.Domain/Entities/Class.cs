using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Entities
{
    public class Class
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid SemesterId { get; private set; }
        public string Code { get; private set; } = default!;
        public string? Name { get; private set; }
        public string Status { get; private set; } = "active";
        public Guid? LecturerId { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        private Class() { }
        public Class(Guid semesterId, string code, string? name, Guid? lecturerId)
        { SemesterId = semesterId; Code = code; Name = name; LecturerId = lecturerId; }
    }
}
