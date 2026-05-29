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
            var data = request.Data;

            var trainingEvent = await _unitOfWork.TrainingEvents.GetEventWithDetailAsync(data.EventId);

            if (trainingEvent == null)
                throw new ArgumentException($"No se encontró el evento con ID {data.EventId}");

           if(!string.IsNullOrEmpty(data.InstructorSignature) && !data.InstructorSignature.StartsWith("http"))
           {
                var fileName = $"instructor-evento-{data.EventId}";
                trainingEvent.InstructorSignatureUrl = await _blobStorage.UploadSignatureAsync(data.InstructorSignature, fileName);
           }

            trainingEvent.GeneralComments = data.Comments!;
            trainingEvent.Status = "Completado";

            var orderedTopics = trainingEvent.Topics.OrderBy(t => t.TopicOrder).ToList();

            foreach (var record in data.EmployeeRecords)
            {
                var attendee = trainingEvent.Attendees.FirstOrDefault(a => a.EmployeeId == record.EmployeeId);

                if (attendee == null) continue;

                if (!string.IsNullOrEmpty(record.Signature) && !record.Signature.StartsWith("http"))
                {
                    var fileName = $"emp-{record.Signature}-evento-{data.EventId}";
                    attendee.ParticipantSignatureUrl = await _blobStorage.UploadSignatureAsync(record.Signature, fileName);
                }

                for(int i = 0; i < record.Evaluations.Count; i++)
                {
                    if(i >= orderedTopics.Count) break;

                    var topicId = orderedTopics[i].Id;
                    var evaluationCell = attendee.Evaluations.FirstOrDefault(e => e.TopicId == topicId);

                    if(evaluationCell != null)
                    {
                        evaluationCell.AttendanceStatus = record.Evaluations[i].Status;
                        evaluationCell.Grade = record.Evaluations[i].Grade;
                    }
                }
            }

            _unitOfWork.TrainingEvents.Update(trainingEvent);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}