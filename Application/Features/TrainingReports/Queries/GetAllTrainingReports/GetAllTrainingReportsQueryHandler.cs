using Application.DTOs.TrainingReports;
using Domain.Common;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.TrainingReports.Queries.GetAllTrainingReports
{
    public class GetAllTrainingReportsQueryHandler(IUnitOfWork unitOfWork) :
        IRequestHandler<GetAllTrainingReportsQuery, PagedResult<TrainingReportSummaryDto>>
    {
        public async Task<PagedResult<TrainingReportSummaryDto>> Handle(
            GetAllTrainingReportsQuery request,
            CancellationToken cancellationToken)
        {
            var reports = await unitOfWork.TrainingReports.GetAllReportsSummaryAsync(
                request.Page, request.PageSize, request.Search,
                request.TrainingType, request.WeekNumber, cancellationToken);

            return new PagedResult<TrainingReportSummaryDto>
            {
                Items = reports.Items.Select(report => new TrainingReportSummaryDto
                {
                    Id = report.Id,
                    TrainingType = report.TrainingType,
                    LeaderName = report.LeaderName,
                    WeekNumber = report.WeekNumber,
                    AttendeesCount = report.AttendeesCount,
                    TotalTrainingMinutes = report.TotalTrainingMinutes,
                    CreatedAt = report.CreatedAt
                }).ToList(),
                Page = reports.Page,
                PageSize = reports.PageSize,
                TotalCount = reports.TotalCount
            };
        }
    }
}
