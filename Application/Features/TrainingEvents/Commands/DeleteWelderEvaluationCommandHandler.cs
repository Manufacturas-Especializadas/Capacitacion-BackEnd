using Domain.Interfaces;
using MediatR;

namespace Application.Features.TrainingEvents.Commands
{
    public record DeleteWelderEvaluationCommand(int Id) : IRequest<Unit>;

    public class DeleteWelderEvaluationCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteWelderEvaluationCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteWelderEvaluationCommand request, CancellationToken cancellationToken)
        {
            var eval = await unitOfWork.WelderEvaluations.GetByIdAsync(request.Id);
            if (eval == null) throw new Exception("No encontrado");

            unitOfWork.WelderEvaluations.Delete(eval);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
