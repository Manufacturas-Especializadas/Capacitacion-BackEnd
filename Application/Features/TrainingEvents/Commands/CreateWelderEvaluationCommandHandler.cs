using Application.Features.WelderEvaluations.Commands;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.TrainingEvents.Commands
{
    public class CreateWelderEvaluationCommandHandler(
        IUnitOfWork unitOfWork,
        IBlobStorageService blobStorage)
        : IRequestHandler<CreateWelderEvaluationCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IBlobStorageService _blobStorage = blobStorage;

        public async Task<int> Handle(CreateWelderEvaluationCommand request, CancellationToken cancellationToken)
        {
            var employee = await _unitOfWork.Employees.GetByEmployeeNumberAsync(request.EmployeeNumber);

            if(employee == null)
            {
                throw new Exception($"No se encontró al empleado con nómina {request.EmployeeNumber}");
            }

            var evaluation = new WelderEvaluation
            {
                EmployeeId = employee.Id,
                EvaluationDate = request.EvaluationDate,
                EvaluatorName = request.EvaluatorName,
                ExclusiveTestReference = request.ExclusiveTestReference,
                TotalPoints = request.TotalPoints,
                PracticalGrade = request.PracticalGrade,
                UnionGrade = request.UnionGrade,
                FinalAverage = request.FinalAverage,
                MasteryLevel = request.MasteryLevel
            };

            if (request.EvidencePhoto != null)
                evaluation.EvidencePhotoUrl = await _blobStorage.UploadFileWeldersAsync(
                    request.EvidencePhoto,
                    $"evidence-{employee.EmployeeNumber}-{Guid.NewGuid()}.png");

            if (request.SignatureColaborador != null)
                evaluation.SignatureColaboradorUrl = await _blobStorage.UploadFileWeldersAsync(
                    request.SignatureColaborador,
                    $"sig-colab-{employee.EmployeeNumber}-{Guid.NewGuid()}.png");

            if (request.SignatureCoordinadorArea != null)
                evaluation.SignatureCoordinadorAreaUrl = await _blobStorage.UploadFileWeldersAsync(
                    request.SignatureCoordinadorArea,
                    $"sig-coordarea-{Guid.NewGuid()}.png");

            if (request.SignatureCoordCapacitacion != null)
                evaluation.SignatureCoordCapacitacionUrl = await _blobStorage.UploadFileWeldersAsync(
                    request.SignatureCoordCapacitacion,
                    $"sig-coordcap-{Guid.NewGuid()}.png");

            if (request.SignatureSupervisor != null)
                evaluation.SignatureSupervisorUrl = await _blobStorage.UploadFileWeldersAsync(
                    request.SignatureSupervisor,
                    $"sig-super-{Guid.NewGuid()}.png");

            if (request.SignatureEvaluador != null)
                evaluation.SignatureEvaluadorUrl = await _blobStorage.UploadFileWeldersAsync(
                    request.SignatureEvaluador,
                    $"sig-eval-{Guid.NewGuid()}.png");

            foreach (var answer in request.PracticalAnswers)
            {
                evaluation.PracticalAnswers.Add(new WelderPracticalAnswer
                {
                    SectionTitle = answer.SectionTitle,
                    QuestionText = answer.QuestionText,
                    Score = answer.Score
                });
            }

            foreach (var union in request.UnionAnswers)
            {
                evaluation.UnionAnswers.Add(new WelderUnionAnswer
                {
                    AttributeName = union.AttributeName,
                    AnswerText = union.AnswerText,
                    Score = union.Score
                });
            }

            await _unitOfWork.WelderEvaluations.AddAsync(evaluation);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return evaluation.Id;
        }
    }
}