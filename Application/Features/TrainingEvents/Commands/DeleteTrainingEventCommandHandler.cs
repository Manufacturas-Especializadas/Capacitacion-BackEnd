using Domain.Interfaces;
using MediatR;

namespace Application.Features.TrainingEvents.Commands
{
    public class DeleteTrainingEventCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteTrainingEventCommand, bool>
    {
        public async Task<bool> Handle(DeleteTrainingEventCommand request, CancellationToken cancellationToken)
        {
            var trainingEvent = await unitOfWork.TrainingEvents.GetByIdAsync(request.Id);

            if (trainingEvent == null)
            {
                throw new Exception($"El evento de capacitación con ID {request.Id} no existe.");
            }

            unitOfWork.TrainingEvents.Delete(trainingEvent);

            var result = await unitOfWork.SaveChangesAsync(cancellationToken);

            return result > 0;
        }
    }
}