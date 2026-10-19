using Application.DTOs;
using Domain.Interfaces;
using Domain.Common;
using MediatR;
using System.Globalization;

namespace Application.Features.TrainingEvents.Queries
{
    public record GetTrainingEventsQuery(
        int Page = 1,
        int PageSize = 25,
        string? Search = null,
        string? Status = null) : IRequest<PagedResult<TrainingEventSummaryDto>>;

    public class GetTrainingEventsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetTrainingEventsQuery, PagedResult<TrainingEventSummaryDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<PagedResult<TrainingEventSummaryDto>> Handle(GetTrainingEventsQuery request, CancellationToken cancellationToken)
        {
            var events = await _unitOfWork.TrainingEvents.GetAllEventsSummaryAsync(
                request.Page, request.PageSize, request.Search, request.Status, cancellationToken);

            return new PagedResult<TrainingEventSummaryDto>
            {
                Items = events.Items.Select(e => new TrainingEventSummaryDto
                {
                    Id = e.Id,
                    CourseName = e.CourseName,
                    InstructorName = e.InstructorName,
                    DateFrom = e.DateFrom.ToString("dd-MM-yyyy"),
                    DateTo = e.DateTo.ToString("dd-MM-yyyy"),
                    WeekNumber = ISOWeek.GetWeekOfYear(e.DateFrom),
                    Status = e.Status,
                    AttendeeCount = e.AttendeeCount,
                    TopicCount = e.TopicCount
                }).ToList(),
                Page = events.Page,
                PageSize = events.PageSize,
                TotalCount = events.TotalCount
            };
        }
    }
}
