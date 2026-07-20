using Domain.Interfaces;
using MediatR;

namespace Application.Features.TrainingReports.Commands
{
    public record DeleteTutoringProgramCommand(int Id) : IRequest<bool>;

    public class DeleteTutoringProgramCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteTutoringProgramCommand, bool>
    {
        public async Task<bool> Handle(DeleteTutoringProgramCommand request, CancellationToken cancellationToken)
        {
            var program = await unitOfWork.TutoringPrograms.GetByIdAsync(request.Id);

            if (program == null)
                return false;

            unitOfWork.TutoringPrograms.Delete(program);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}