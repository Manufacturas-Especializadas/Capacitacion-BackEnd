using Domain.Interfaces;
using Application.DTOs;
using MediatR;

namespace Application.Features.WeldingReportUnionTypes.Queries
{
    public record GetAllWeldingReportUnionTypesQuery() : IRequest<IEnumerable<WeldingReportUnionTypeDto>>;

    public class GetAllWeldingReportUnionTypesQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllWeldingReportUnionTypesQuery, IEnumerable<WeldingReportUnionTypeDto>>
    {
        public async Task<IEnumerable<WeldingReportUnionTypeDto>> Handle(GetAllWeldingReportUnionTypesQuery request, CancellationToken cancellationToken)
        {
            var types = await unitOfWork.WeldingReportUnionTypes.GetAllAsync();

            return types.Select(t => new WeldingReportUnionTypeDto
            {
                Id = t.Id,
                ReportId = t.ReportId,
                UnionTypeName = t.UnionName
            });
        }
    }
}