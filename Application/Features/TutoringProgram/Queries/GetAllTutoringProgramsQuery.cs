using Application.DTOs;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.TutoringProgram.Queries
{
    public record GetAllTutoringProgramsQuery() : IRequest<IEnumerable<TutoringProgramListDto>>;

    public class GetAllTutoringProgramsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllTutoringProgramsQuery, IEnumerable<TutoringProgramListDto>>
    {
        public async Task<IEnumerable<TutoringProgramListDto>> Handle(GetAllTutoringProgramsQuery request, CancellationToken cancellationToken)
        {
            var programs = await unitOfWork.TutoringPrograms.GetAllAsync(p => p.Tutor);

            if (programs == null || !programs.Any())
            {
                return Enumerable.Empty<TutoringProgramListDto>();
            }

            var result = programs.Select(p => new TutoringProgramListDto
            {
                Id = p.Id,
                TutorId = p.TutorId,
                TutorName = p.Tutor?.TutorName ?? "",
                CollaboratorName = p.CollaboratorName,
                PayrollNumber = p.PayrollNumber,
                Area = p.Area,
                WeekId = p.WeekId,
                CreatedDate = p.CreatedDate
            })
            .OrderByDescending(p => p.CreatedDate)
            .ToList();

            return result;
        }
    }
}