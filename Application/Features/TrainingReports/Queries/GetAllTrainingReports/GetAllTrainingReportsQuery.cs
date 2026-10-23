using Application.DTOs.TrainingReports;
using Domain.Common;
using MediatR;

namespace Application.Features.TrainingReports.Queries.GetAllTrainingReports
{
    public record GetAllTrainingReportsQuery(
        int Page = 1,
        int PageSize = 25,
        string? Search = null,
        string? TrainingType = null,
        int? WeekNumber = null) : IRequest<PagedResult<TrainingReportSummaryDto>>;
}
