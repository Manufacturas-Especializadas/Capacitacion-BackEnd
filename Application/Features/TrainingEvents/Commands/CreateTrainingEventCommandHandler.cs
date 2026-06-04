using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.TrainingEvents.Commands
{
    public record CreateTrainingEventCommand(CreateTrainingEventDto EventData) : IRequest<int>;

    public class CreateTrainingEventCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateTrainingEventCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<int> Handle(CreateTrainingEventCommand request, CancellationToken cancellationToken)
        {
            var data = request.EventData;

            TimeZoneInfo mexicoTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time (Mexico)");

            DateTime nowInMexico = TimeZoneInfo.ConvertTime(DateTime.UtcNow, mexicoTimeZone);

            var newEvent = new TrainingEvent
            {
                CourseName = data.CourseName,
                InstructorName = data.InstructorName,
                RoomId = data.RoomId,
                DateFrom = data.DateFrom,
                DateTo = data.DateTo,
                Status = "PROGRAMADO",
                CreatedAt = nowInMexico
            };

            for (int i = 0; i < data.EvaluationTopics.Count; i++)
            {
                var topic = data.EvaluationTopics[i];
                newEvent.Topics.Add(new EventTopic
                {
                    TopicName = topic.Name,
                    TopicOrder = i,
                    TopicDate = topic.Date,
                    StartTime = topic.StartTime,
                    EndTime = topic.EndTime
                });
            }

            await _unitOfWork.TrainingEvents.AddAsync(newEvent);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return newEvent.Id;
        }
    }
}