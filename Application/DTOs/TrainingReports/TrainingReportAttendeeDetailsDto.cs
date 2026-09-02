using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.TrainingReports
{
    public class TrainingReportAttendeeDetailsDto
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public string EmployeeNumber { get; set; } = string.Empty;

        public string EmployeeName { get; set; } = string.Empty;

        public int LineId { get; set; }

        public string LineName { get; set; } = string.Empty;

        public bool DayMonday { get; set; }

        public bool DayTuesday { get; set; }

        public bool DayWednesday { get; set; }

        public bool DayThursday { get; set; }

        public bool DayFriday { get; set; }

        public bool DaySaturday { get; set; }

        public bool DaySunday { get; set; }

        public decimal? HoursMonday { get; set; }

        public decimal? HoursTuesday { get; set; }

        public decimal? HoursWednesday { get; set; }

        public decimal? HoursThursday { get; set; }

        public decimal? HoursFriday { get; set; }

        public decimal? HoursSaturday { get; set; }

        public decimal? HoursSunday { get; set; }

        public decimal? TotalHours { get; set; }

        public string? CustomerClient { get; set; }

        public string? UnionClassification { get; set; }

        public string? WeldingPercentage { get; set; }

        public string? Diameter { get; set; }

        public string? Shift { get; set; }

        public string? Machinery { get; set; }

        public string? Ast { get; set; }

        public string? TraineeSignatureUrl { get; set; }

        public string? SupervisorSignatureUrl { get; set; }

        public List<TrainingReportTopicDetailsDto> Topics { get; set; }
            = new();
    }
}
