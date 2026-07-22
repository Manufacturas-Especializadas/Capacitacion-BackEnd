using Domain.Interfaces;
using Application.DTOs;
using MediatR;

namespace Application.Features.TrainingTopics.Commands
{
    public record UpdateTrainingTopicCommand(int Id, CreateTrainingTopicDto Data) : IRequest<bool>;

    public class UpdateTrainingTopicCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateTrainingTopicCommand, bool>
    {
        public async Task<bool> Handle(UpdateTrainingTopicCommand request, CancellationToken cancellationToken)
        {
            var topic = await unitOfWork.TrainingTopics.GetByIdAsync(request.Id);
            if (topic == null) return false;

            topic.TrainingType = request.Data.TrainingType.ToUpper();
            topic.TopicCode = request.Data.TopicCode;
            topic.TopicName = request.Data.TopicName;

            unitOfWork.TrainingTopics.Update(topic);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}