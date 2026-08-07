using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.TrainingReports.Commands
{
    public class UpdateTrainingReportCommand: IRequest<UpdateTrainingReportResult>
    {
        public int Id { get; set; }

        public string TrainingType { get; set; } = string.Empty;

        public string LeaderName { get; set; } = string.Empty;

        public string LeaderPayroll { get; set; } = string.Empty;

        public int? WeekNumber { get; set; }

        public string? Observations { get; set; }

        public IFormFile? InstructorSignature { get; set; }

        public IFormFile? CoordinatorSignature { get; set; }

        public IFormFile? SecuritySignature { get; set; }

        public bool RemoveInstructorSignature { get; set; }

        public bool RemoveCoordinatorSignature { get; set; }

        public bool RemoveSecuritySignature { get; set; }

        public List<UpdateWeldingUnionTypeDto> UnionTypes
        {
            get;
            set;
        } = new();

        public List<UpdateTrainingReportAttendeeDto> Attendees
        {
            get;
            set;
        } = new();
    }
}
