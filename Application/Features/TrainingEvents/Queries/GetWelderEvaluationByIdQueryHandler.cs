using Application.DTOs;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.TrainingEvents.Queries
{
    public record GetWelderEvaluationByIdQuery(int Id) : IRequest<WelderEvaluationDetailDto>;

    public class GetWelderEvaluationByIdQueryHandler(IUnitOfWork unitOfWork)
        : IRequestHandler<GetWelderEvaluationByIdQuery, WelderEvaluationDetailDto>
    {
        public async Task<WelderEvaluationDetailDto> Handle(GetWelderEvaluationByIdQuery request, CancellationToken cancellationToken)
        {
            var eval = await unitOfWork.WelderEvaluations.GetByIdWithRelationsAsync(request.Id);

            if (eval == null) throw new Exception("Evaluación no encontrada");

            return new WelderEvaluationDetailDto
            {
                Id = eval.Id,
                EmployeeNumber = eval.Employee?.EmployeeNumber ?? "N/A",
                EmployeeName = eval.Employee?.Name ?? "Sin nombre",
                EvaluationDate = eval.EvaluationDate,
                FinalAverage = eval.FinalAverage,
                MasteryLevel = eval.MasteryLevel!,
                EvidencePhotoUrl = eval.EvidencePhotoUrl,
                SignatureColaboradorUrl = eval.SignatureColaboradorUrl,
                SignatureCoordinadorAreaUrl = eval.SignatureCoordinadorAreaUrl,
                SignatureCoordCapacitacionUrl = eval.SignatureCoordCapacitacionUrl,
                SignatureSupervisorUrl = eval.SignatureSupervisorUrl,
                SignatureEvaluadorUrl = eval.SignatureEvaluadorUrl,
                PracticalAnswers = eval.PracticalAnswers.Select(a => new PracticalAnswerDto
                {
                    SectionTitle = a.SectionTitle,
                    QuestionText = a.QuestionText,
                    Score = a.Score
                }).ToList(),
                UnionAnswers = eval.UnionAnswers.Select(a => new UnionAnswerDto
                {
                    AttributeName = a.AttributeName,
                    AnswerText = a.AnswerText,
                    Score = a.Score
                }).ToList()
            };
        }
    }
}
