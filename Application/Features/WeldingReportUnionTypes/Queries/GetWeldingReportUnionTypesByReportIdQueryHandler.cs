using Domain.Interfaces;
using Application.DTOs;
using MediatR;

namespace Application.Features.WeldingReportUnionTypes.Queries
{
    public record GetWeldingReportUnionTypesByReportIdQuery(int ReportId) : IRequest<IEnumerable<WeldingReportUnionTypeDto>>;

    public class GetWeldingReportUnionTypesByReportIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetWeldingReportUnionTypesByReportIdQuery, IEnumerable<WeldingReportUnionTypeDto>>
    {
        public async Task<IEnumerable<WeldingReportUnionTypeDto>> Handle(GetWeldingReportUnionTypesByReportIdQuery request, CancellationToken cancellationToken)
        {
            var allTypes = await unitOfWork.WeldingReportUnionTypes.GetAllAsync();
            var filteredTypes = allTypes.Where(t => t.ReportId == request.ReportId);

            return filteredTypes.Select(t => new WeldingReportUnionTypeDto
            {
                Id = t.Id,
                ReportId = t.ReportId,
                UnionTypeName = t.UnionName
            });
        }
    }
}