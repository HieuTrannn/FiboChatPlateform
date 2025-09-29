using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Entities
{
    public class Semester
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Code { get; private set; } = default!;
        public string Term { get; private set; } = default!; // SP|SU|FA
        public int Year { get; private set; }
        public string? Name { get; private set; }
        public string Status { get; private set; } = "active";
        public DateTime? StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        private Semester() { }
        public Semester(string code, string term, int year, string? name = null)
        { Code = code; Term = term; Year = year; Name = name; }
    }
}
