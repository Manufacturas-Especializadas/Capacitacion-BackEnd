using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.TrainingReports
{
    public class TrainingReportSummaryDto
    {
        public int Id { get; set; }

        public string TrainingType { get; set; } = string.Empty;

        public string LeaderName { get; set; } = string.Empty;

        public int? WeekNumber { get; set; }

        public int AttendeesCount { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
