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

            var newEvent = new TrainingEvent
            {
                CourseName = data.CourseName,
                InstructorName = data.InstructorName,
                RoomId = data.RoomId,
                DateFrom = data.DateFrom,
                DateTo = data.DateTo,
                Status = "Borrador" 
            };

            for (int i = 0; i < data.EvaluationTopics.Count; i++)
            {
                newEvent.Topics.Add(new EventTopic
                {
                    TopicName = data.EvaluationTopics[i],
                    TopicOrder = i
                });
            }
            
            await _unitOfWork.TrainingEvents.AddAsync(newEvent);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return newEvent.Id;
        }
    }
}