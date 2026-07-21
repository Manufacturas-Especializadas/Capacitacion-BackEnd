using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.TrainingReports.Commands
{
    public record UpdateTutoringProgramCommand(UpdateTutoringProgramDto Data) : IRequest<bool>;

    public class UpdateTutoringProgramCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateTutoringProgramCommand, bool>
    {
        public async Task<bool> Handle(UpdateTutoringProgramCommand request, CancellationToken cancellationToken)
        {
            var program = await unitOfWork.TutoringPrograms.GetByIdAsync(request.Data.Id);

            if (program == null)
                return false;

            program.TutorId = request.Data.TutorId;
            program.CollaboratorName = request.Data.CollaboratorName;
            program.PayrollNumber = request.Data.PayrollNumber;
            program.Area = request.Data.Area;
            program.WeekId = request.Data.WeekId;

            unitOfWork.TutoringPrograms.Update(program);

            var oldAnswers = await unitOfWork.Answers.GetAllAsync();
            var currentProgramAnswers = oldAnswers.Where(a => a.TutoringProgramId == program.Id).ToList();

            if (currentProgramAnswers.Any())
            {
                unitOfWork.Answers.DeleteRange(currentProgramAnswers);
            }

            var newAnswers = request.Data.Answers.Select(a => new Answer
            {
                TutoringProgramId = program.Id,
                QuestionId = a.QuestionId,
                OptionId = a.OptionId,
                RatingValue = a.RatingValue,
                TextValue = a.TextValue
            }).ToList();

            await unitOfWork.Answers.AddRangeAsync(newAnswers);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}