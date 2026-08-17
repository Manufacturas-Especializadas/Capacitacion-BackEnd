using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.TrainingReports.Commands
{
    public class UpdateWeldingUnionTypeDto
    {
        public int? Id { get; set; }

        public int ListNumber { get; set; }

        public string UnionName { get; set; } = string.Empty;
    }

    public class UpdateTrainingReportAttendeeTopicDto
    {
        public int TopicId { get; set; }

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
    }

    public class UpdateTrainingReportAttendeeDto
    {
        public int? Id { get; set; }

        public int EmployeeId { get; set; }

        public int LineId { get; set; }

        public bool DayMonday { get; set; }

        public bool DayTuesday { get; set; }

        public bool DayWednesday { get; set; }

        public bool DayThursday { get; set; }

        public bool DayFriday { get; set; }

        public bool DaySaturday { get; set; }

        public bool DaySunday { get; set; }

        public string? CustomerClient { get; set; }

        public string? UnionClassification { get; set; }

        public string? WeldingPercentage { get; set; }

        public string? Diameter { get; set; }

        public string? Shift { get; set; }

        public string? Machinery { get; set; }

        public string? Ast { get; set; }

        public List<int> TopicIds { get; set; } = new();

        public List<UpdateTrainingReportAttendeeTopicDto> Topics { get; set; } = new();

        public IFormFile? TraineeSignature { get; set; }

        public IFormFile? SupervisorSignature { get; set; }

        public bool RemoveTraineeSignature { get; set; }

        public bool RemoveSupervisorSignature { get; set; }
    }
}
