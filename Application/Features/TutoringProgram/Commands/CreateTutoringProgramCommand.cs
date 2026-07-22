using Domain.Interfaces;
using Application.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.Features.TutoringProgram.Commands
{
    public record CreateTutoringProgramCommand(CreateTutoringProgramDto Data) : IRequest<TutoringProgramDto>;

    public class CreateTutoringProgramCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateTutoringProgramCommand, TutoringProgramDto>
    {
        public async Task<TutoringProgramDto> Handle(CreateTutoringProgramCommand request, CancellationToken cancellationToken)
        {
            TimeZoneInfo mexicoTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time (Mexico)");

            DateTime nowInMexico = TimeZoneInfo.ConvertTime(DateTime.UtcNow, mexicoTimeZone);

            var program = new Domain.Entities.TutoringProgram
            {
                TutorId = request.Data.TutorId,
                CollaboratorName = request.Data.CollaboratorName,
                PayrollNumber = request.Data.PayrollNumber,
                Area = request.Data.Area,
                WeekId = request.Data.WeekId,
                CreatedDate = nowInMexico,
                Answers = request.Data.Answers.Select(a => new Answer
                {
                    QuestionId = a.QuestionId,
                    OptionId = a.OptionId,
                    RatingValue = a.RatingValue,
                    TextValue = a.TextValue
                }).ToList()
            };

            await unitOfWork.TutoringPrograms.AddAsync(program);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return new TutoringProgramDto
            {
                Id = program.Id,
                TutorId = program.TutorId,
                CollaboratorName = program.CollaboratorName,
                PayrollNumber = program.PayrollNumber,
                Area = program.Area,
                WeekId = program.WeekId,
                CreatedDate = nowInMexico,
                Answers = request.Data.Answers
            };
        }
    }
}