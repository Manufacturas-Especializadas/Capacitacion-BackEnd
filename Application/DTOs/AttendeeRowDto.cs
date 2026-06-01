using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs
{
    public class AttendeeRowDto
    {
        public required string EmployeeNumber { get; set; }

        public required string Name { get; set; }

        public required string LineName { get; set; }

        public List<bool> Enrollments { get; set; } = new();
    }
}