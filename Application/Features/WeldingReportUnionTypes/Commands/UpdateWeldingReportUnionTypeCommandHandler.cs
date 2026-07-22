using Application.DTOs;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.WeldingReportUnionTypes.Commands
{
    public record UpdateWeldingReportUnionTypeCommand(int Id, CreateWeldingReportUnionTypeDto Data) : IRequest<bool>;

    public class UpdateWeldingReportUnionTypeCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateWeldingReportUnionTypeCommand, bool>
    {
        public async Task<bool> Handle(UpdateWeldingReportUnionTypeCommand request, CancellationToken cancellationToken)
        {
            var entity = await unitOfWork.WeldingReportUnionTypes.GetByIdAsync(request.Id);

            if (entity == null) return false;

            entity.ReportId = request.Data.ReportId;
            entity.UnionName = request.Data.UnionTypeName.ToUpper();

            unitOfWork.WeldingReportUnionTypes.Update(entity);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}