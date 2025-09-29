using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.Contracts.DTOs.ClassDTOs
{
    public class ClassResponse
    {
        public Guid Id { get; set; }
        public Guid SemesterId { get; set; }
        public string Code { get; set; }
        public string? Name { get; set; }
        public string Status { get; set; }
        public Guid? LecturerId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}