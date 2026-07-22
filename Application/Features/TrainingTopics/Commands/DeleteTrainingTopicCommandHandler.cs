using Domain.Interfaces;
using MediatR;

namespace Application.Features.TrainingTopics.Commands
{
    public record DeleteTrainingTopicCommand(int Id) : IRequest<bool>;

    public class DeleteTrainingTopicCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteTrainingTopicCommand, bool>
    {
        public async Task<bool> Handle(DeleteTrainingTopicCommand request, CancellationToken cancellationToken)
        {
            var topic = await unitOfWork.TrainingTopics.GetByIdAsync(request.Id);
            if (topic == null) return false;

            unitOfWork.TrainingTopics.Delete(topic);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}