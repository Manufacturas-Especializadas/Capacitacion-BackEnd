using Domain.Interfaces;
using Application.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.Features.WeldingReportUnionTypes.Commands
{
    public record CreateWeldingReportUnionTypeCommand(CreateWeldingReportUnionTypeDto Data) : IRequest<WeldingReportUnionTypeDto>;

    public class CreateWeldingReportUnionTypeCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateWeldingReportUnionTypeCommand, WeldingReportUnionTypeDto>
    {
        public async Task<WeldingReportUnionTypeDto> Handle(CreateWeldingReportUnionTypeCommand request, CancellationToken cancellationToken)
        {
            var unionType = new WeldingReportUnionType
            {
                ReportId = request.Data.ReportId,
                UnionName = request.Data.UnionTypeName.ToUpper()
            };

            await unitOfWork.WeldingReportUnionTypes.AddAsync(unionType);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return new WeldingReportUnionTypeDto
            {
                Id = unionType.Id,
                ReportId = unionType.ReportId,
                UnionTypeName = unionType.UnionName
            };
        }
    }
}