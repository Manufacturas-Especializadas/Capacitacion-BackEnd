using Application.DTOs;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.TutoringProgram.Queries
{
    public record GetTutoringProgramByIdQuery(int Id) : IRequest<TutoringProgramDto>;

    public class GetTutoringProgramByIdQueryHandler(IUnitOfWork unitOfWork): IRequestHandler<GetTutoringProgramByIdQuery, TutoringProgramDto?>
    {
        public async Task<TutoringProgramDto?> Handle(GetTutoringProgramByIdQuery request, CancellationToken cancellation)
        {
            var program = await unitOfWork.TutoringPrograms.GetByIdAsync(request.Id);

            if (program == null) return null;

            var allAnswers = await unitOfWork.Answers.GetAllAsync();
            var programAnswers = allAnswers.Where(a => a.TutoringProgramId == program.Id).ToList();

            return new TutoringProgramDto
            {
                Id = program.Id,
                TutorId = program.TutorId,
                CollaboratorName = program.CollaboratorName,
                PayrollNumber = program.PayrollNumber,
                Area = program.Area,
                WeekId = program.WeekId,
                CreatedDate = program.CreatedDate,
                Answers = programAnswers.Select(a => new TutoringAnswerDto
                {
                    QuestionId = a.QuestionId,
                    OptionId = a.OptionId,
                    RatingValue = a.RatingValue,
                    TextValue = a.TextValue
                }).ToList()
            };
        }
    }
}