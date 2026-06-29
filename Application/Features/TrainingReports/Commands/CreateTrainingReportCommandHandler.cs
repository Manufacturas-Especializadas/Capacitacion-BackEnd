using Microsoft.AspNetCore.Http;
using Domain.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.TrainingReports.Commands
{
    public class CreateWeldingUnionTypeDto
    {
        public int ListNumber { get; set; }
        public required string UnionName { get; set; }
    }

    public class CreateTrainingReportAttendeeDto
    {
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

        public required List<int> TopicIds { get; set; } = new();

        public IFormFile? TraineeSignature { get; set; }

        public IFormFile? SupervisorSignature { get; set; }
    }

    public class CreateTrainingReportCommand : IRequest<int>
    {
        public required string TrainingType { get; set; }

        public required string LeaderName { get; set; }

        public required string LeaderPayroll { get; set; }

        public int? WeekNumber { get; set; }

        public string? Observations { get; set; }

        public IFormFile? InstructorSignature { get; set; }

        public IFormFile? CoordinatorSignature { get; set; }

        public IFormFile? SecuritySignature { get; set; }

        public List<CreateWeldingUnionTypeDto>? UnionTypes { get; set; } = new();

        public required List<CreateTrainingReportAttendeeDto> Attendees { get; set; } = new();
    }

    public class CreateTrainingReportCommandHandler(IUnitOfWork unitOfWork, IBlobStorageService blobStorage) : IRequestHandler<CreateTrainingReportCommand, int>
    {
        public async Task<int> Handle(CreateTrainingReportCommand request, CancellationToken cancellationToken)
        {
            TimeZoneInfo mexicoTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time (Mexico)");

            DateTime nowInMexico = TimeZoneInfo.ConvertTime(DateTime.UtcNow, mexicoTimeZone);

            var report = new TrainingReport
            {
                TrainingType = request.TrainingType,
                LeaderName = request.LeaderName,
                LeaderPayroll = request.LeaderPayroll,
                WeekNumber = request.WeekNumber,
                Observations = request.Observations,
                CreatedAt = nowInMexico,

                InstructorSignatureUrl = request.InstructorSignature != null
                    ? await blobStorage.UploadFileTrainingReportAsync(request.InstructorSignature, "instructor_sig.png")
                    : null,
                CoordinatorSignatureUrl = request.CoordinatorSignature != null
                    ? await blobStorage.UploadFileTrainingReportAsync(request.CoordinatorSignature, "coordinator_sig.png")
                    : null,
                SecuritySignatureUrl = request.SecuritySignature != null
                    ? await blobStorage.UploadFileTrainingReportAsync(request.SecuritySignature, "security_sig.png")
                    : null
            };

            if (request.UnionTypes != null && request.UnionTypes.Any())
            {
                foreach (var union in request.UnionTypes)
                {
                    report.WeldingUnionTypes.Add(new WeldingReportUnionType
                    {
                        ListNumber = union.ListNumber,
                        UnionName = union.UnionName
                    });
                }
            }

            foreach (var attDto in request.Attendees)
            {
                var attendee = new TrainingReportAttendee
                {
                    EmployeeId = attDto.EmployeeId,
                    LineId = attDto.LineId,
                    DayMonday = attDto.DayMonday,
                    DayTuesday = attDto.DayTuesday,
                    DayWednesday = attDto.DayWednesday,
                    DayThursday = attDto.DayThursday,
                    DayFriday = attDto.DayFriday,
                    DaySaturday = attDto.DaySaturday,
                    DaySunday = attDto.DaySunday,
                    CustomerClient = attDto.CustomerClient,
                    UnionClassification = attDto.UnionClassification,
                    WeldingPercentage = attDto.WeldingPercentage,
                    Diameter = attDto.Diameter,
                    Shift = attDto.Shift,
                    Machinery = attDto.Machinery,
                    Ast = attDto.Ast,

                    TraineeSignatureUrl = attDto.TraineeSignature != null
                        ? await blobStorage.UploadFileTrainingReportAsync(attDto.TraineeSignature, $"trainee_{attDto.EmployeeId}_sig.png")
                        : null,
                    SupervisorSignatureUrl = attDto.SupervisorSignature != null
                        ? await blobStorage.UploadFileTrainingReportAsync(attDto.SupervisorSignature, $"supervisor_{attDto.EmployeeId}_sig.png")
                        : null
                };

                if (attDto.TopicIds != null && attDto.TopicIds.Any())
                {
                    foreach (var topicId in attDto.TopicIds)
                    {
                        var topic = await unitOfWork.TrainingTopics.GetByIdAsync(topicId);
                        if (topic != null)
                        {
                            attendee.Topics.Add(topic);
                        }
                    }
                }

                report.Attendees.Add(attendee);
            }

            unitOfWork.TrainingReports.AddAsync(report);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return report.Id;
        }
    }
}