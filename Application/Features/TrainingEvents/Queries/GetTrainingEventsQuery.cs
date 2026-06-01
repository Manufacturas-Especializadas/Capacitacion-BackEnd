using Application.DTOs;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.TrainingEvents.Queries
{
    public record GetTrainingEventsQuery() : IRequest<List<TrainingEventSummaryDto>>;

    public class GetTrainingEventsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetTrainingEventsQuery, List<TrainingEventSummaryDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<List<TrainingEventSummaryDto>> Handle(GetTrainingEventsQuery request, CancellationToken cancellationToken)
        {
            var events = await _unitOfWork.TrainingEvents.GetAllEventsSummaryAsync();

            return events.Select(e => new TrainingEventSummaryDto
            {
                Id = e.Id,
                CourseName = e.CourseName,
                InstructorName = e.InstructorName,
                DateFrom = e.DateFrom.ToString("dd-MM-yyyy"),
                DateTo = e.DateTo.ToString("dd-MM-yyyy"),
                Status = e.Status,
                AttendeeCount = e.Attendees.Count
            }).ToList();
        }
    }
}