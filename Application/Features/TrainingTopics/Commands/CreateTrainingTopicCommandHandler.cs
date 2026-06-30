using Domain.Interfaces;
using Application.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.Features.TrainingTopics.Commands
{
    public record CreateTrainingTopicCommand(CreateTrainingTopicDto Data) : IRequest<TrainingTopicDto>;

    public class CreateTrainingTopicCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateTrainingTopicCommand, TrainingTopicDto>
    {
        public async Task<TrainingTopicDto> Handle(CreateTrainingTopicCommand request, CancellationToken cancellationToken)
        {
            var topic = new TrainingTopic
            {
                TrainingType = request.Data.TrainingType.ToUpper(),
                TopicCode = request.Data.TopicCode,
                TopicName = request.Data.TopicName
            };

            await unitOfWork.TrainingTopics.AddAsync(topic);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return new TrainingTopicDto
            {
                Id = topic.Id,
                TrainingType = topic.TrainingType,
                TopicCode = topic.TopicCode,
                TopicName = topic.TopicName
            };
        }
    }
}