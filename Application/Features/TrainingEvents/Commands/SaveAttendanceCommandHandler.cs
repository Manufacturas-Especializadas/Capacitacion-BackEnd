using Application.DTOs;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.TrainingEvents.Commands
{
    public record SaveAttendanceCommand(SaveAttendanceDto Data) : IRequest<bool>;

    public class SaveAttendanceCommandHandler(IUnitOfWork unitOfWork, IBlobStorageService blobStorage) : IRequestHandler<SaveAttendanceCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IBlobStorageService _blobStorage = blobStorage;

        public async Task<bool> Handle(SaveAttendanceCommand request, CancellationToken cancellationToken)
        {
            var trainingEvent = await _unitOfWork.TrainingEvents.GetEventWithAttendeesAsync(request.Data.EventId);
            if (trainingEvent == null) return false;

            trainingEvent.GeneralComments = request.Data.Comments;

            if (request.Data.InstructorSignature != null && request.Data.InstructorSignature.Length > 0)
            {
                var instructorFileName = $"signatures/instructor-{request.Data.EventId}-{Guid.NewGuid()}.png";

                trainingEvent.InstructorSignatureUrl = await _blobStorage.UploadFileAsync(request.Data.InstructorSignature, instructorFileName);
            }

            var orderedTopics = trainingEvent.Topics?.OrderBy(t => t.Id).ToList() ?? new();

            foreach (var record in request.Data.EmployeeRecords)
            {
                var attendee = trainingEvent.Attendees.FirstOrDefault(a => a.EmployeeId == record.EmployeeId);

                if (attendee == null) continue;

                if (record.Signature != null && record.Signature.Length > 0)
                {
                    var participantFileName = $"signatures/emp-{record.EmployeeId}-event-{request.Data.EventId}-{Guid.NewGuid()}.png";

                    attendee.ParticipantSignatureUrl = await _blobStorage.UploadFileAsync(record.Signature, participantFileName);
                }

                for (int i = 0; i < record.Evaluations.Count; i++)
                {
                    if (i >= orderedTopics.Count) break;

                    var topicId = orderedTopics[i].Id;
                    var evaluationCell = attendee.Evaluations.FirstOrDefault(e => e.TopicId == topicId);

                    if (evaluationCell != null)
                    {
                        evaluationCell.AttendanceStatus = record.Evaluations[i].Status;
                        evaluationCell.Grade = record.Evaluations[i].Grade;
                    }
                }

                var totalEvaluations = attendee.Evaluations.Count;

                if(totalEvaluations > 0)
                {
                    var presentCount = attendee.Evaluations.Count(e => e.AttendanceStatus == "PRESENT");
                    attendee.AttendancePercentage = Math.Round((decimal)presentCount / totalEvaluations * 100, 2);

                    var grededEvaluations = attendee.Evaluations
                            .Where(e => e.Grade.HasValue)
                            .Select(e => e.Grade!.Value)
                            .ToList();

                    if (grededEvaluations.Any())
                    {
                        attendee.FinalGradeAverage = Math.Round((decimal)grededEvaluations.Average(), 2);
                    }
                    else
                    {
                        attendee.FinalGradeAverage = null;
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}