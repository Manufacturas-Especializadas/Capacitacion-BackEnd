using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.TrainingReports
{
    public class TrainingReportDetailsDto
    {
        public int Id { get; set; }

        public string TrainingType { get; set; } = string.Empty;

        public string LeaderName { get; set; } = string.Empty;

        public string LeaderPayroll { get; set; } = string.Empty;

        public int? WeekNumber { get; set; }

        public string? Observations { get; set; }

        public string? InstructorSignatureUrl { get; set; }

        public string? CoordinatorSignatureUrl { get; set; }

        public string? SecuritySignatureUrl { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<WeldingReportUnionTypeDetailsDto> WeldingUnionTypes { get; set; } = new();

        public List<TrainingReportAttendeeDetailsDto> Attendees { get; set; } = new();

    }
}
