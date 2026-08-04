using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.TrainingReports
{
    public class TrainingReportTopicDetailsDto
    {
        public int Id { get; set; }

        public string TrainingType { get; set; } = string.Empty;

        public string TopicCode { get; set; } = string.Empty;

        public string TopicName { get; set; } = string.Empty;
    }
}
