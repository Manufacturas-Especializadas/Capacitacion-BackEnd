using Domain.Interfaces;
using MediatR;

namespace Application.Features.WeldingReportUnionTypes.Commands
{
    public record DeleteWeldingReportUnionTypeCommand(int Id) : IRequest<bool>;

    public class DeleteWeldingReportUnionTypeCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteWeldingReportUnionTypeCommand, bool>
    {
        public async Task<bool> Handle(DeleteWeldingReportUnionTypeCommand request, CancellationToken cancellationToken)
        {
            var entity = await unitOfWork.WeldingReportUnionTypes.GetByIdAsync(request.Id);

            if (entity == null) return false;

            unitOfWork.WeldingReportUnionTypes.Delete(entity);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}