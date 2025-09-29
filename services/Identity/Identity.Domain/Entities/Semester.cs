using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Entities
{
    public class Semester
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Code { get; set; } = default!;
        public string Term { get; set; } = default!; // SP|SU|FA
        public int Year { get; set; }
        public string? Name { get; set; }
        public string Status { get; set; } = "active";
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        private Semester() { }
        public Semester(string code, string term, int year, string? name = null)
        { Code = code; Term = term; Year = year; Name = name; }
    }
}
