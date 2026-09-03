using Application.DTOs;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.TrainingEvents.Queries
{
    public record GetTrainingEventQuery(int EventId) : IRequest<TrainingEventDetailDto?>;

    public class GetTrainingEventQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetTrainingEventQuery, TrainingEventDetailDto?>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<TrainingEventDetailDto?> Handle(GetTrainingEventQuery request, CancellationToken cancellationToken)
        {
            var entity = await _unitOfWork.TrainingEvents.GetEventWithDetailAsync(request.EventId);

            if (entity == null) return null;

            var orderedTopics = entity.Topics.OrderBy(t => t.TopicOrder).ToList();

            var eventData = new EventDataDto
            {
                Id = entity.Id,
                CourseName = entity.CourseName,
                Instructor = entity.InstructorName,
                InstructorSignatureUrl = entity.InstructorSignatureUrl,
                RoomId = entity.RoomId,
                Area = entity.Room?.RoomName ?? "Sin sala",
                DateFrom = entity.DateFrom.ToString("dd-MM-yyyy"),
                DateTo = entity.DateTo.ToString("dd-MM-yyyy"),
                EvaluationTopics = orderedTopics.Select(t => new TopicDetailDto
                {
                    Id = t.Id,
                    Name = t.TopicName,
                    Date = t.TopicDate.HasValue ? t.TopicDate.Value.ToString("dd-MM-yyyy") : string.Empty,
                    StartTime = t.StartTime.HasValue ? t.StartTime.Value.ToString(@"hh\:mm") : string.Empty,
                    EndTime = t.EndTime.HasValue ? t.EndTime.Value.ToString(@"hh\:mm") : string.Empty
                }).ToList()
            };

            var employeesList = new List<EmployeeDto>();
            var attendanceList = new List<AttendanceRecordDto>();

            foreach (var attendee in entity.Attendees)
            {
                var empIdString = attendee.EmployeeId.ToString();

                employeesList.Add(new EmployeeDto
                {
                    Id = empIdString,
                    EmployeeNumber = attendee.Employee!.EmployeeNumber,
                    Name = attendee.Employee.Name,
                    Line = attendee.Employee.ProductionLine?.LineName ?? "Sin línea"
                });

                var evaluationsDto = new List<TopicEvaluationDto>();

                foreach (var topic in orderedTopics)
                {
                    var cell = attendee.Evaluations.FirstOrDefault(e => e.TopicId == topic.Id);

                    evaluationsDto.Add(new TopicEvaluationDto
                    {
                        IsEnrolled = cell?.IsEnrolled ?? false,
                        Status = cell?.AttendanceStatus ?? "EMPTY",
                        Grade = cell?.Grade
                    });
                }

                attendanceList.Add(new AttendanceRecordDto
                {
                    EmployeeId = empIdString,
                    Signature = attendee.ParticipantSignatureUrl,
                    Evaluations = evaluationsDto
                });
            }

            return new TrainingEventDetailDto
            {
                EventData = eventData,
                Employees = employeesList,
                InitialAttendance = attendanceList
            };
        }
    }
}