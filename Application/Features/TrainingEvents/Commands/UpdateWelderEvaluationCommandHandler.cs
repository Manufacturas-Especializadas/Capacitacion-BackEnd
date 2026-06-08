using Domain.Entities;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;


namespace Application.Features.TrainingEvents.Commands
{
    public class UpdatePracticalAnswerDto
    {
        public string SectionTitle { get; set; } = string.Empty;
        public string QuestionText { get; set; } = string.Empty;
        public int Score { get; set; }
    }

    public class UpdateUnionAnswerDto
    {
        public string AttributeName { get; set; } = string.Empty;
        public string? AnswerText { get; set; }
        public decimal? Score { get; set; }
    }

    public class UpdateWelderEvaluationCommand : IRequest<int>
    {
        public int Id { get; set; }

        public string EmployeeNumber { get; set; } = string.Empty;

        public string EvaluatorName { get; set; } = string.Empty;

        public string ExclusiveTestReference { get; set; } = string.Empty;

        public string? ExclusiveTestResult { get; set; }

        public int TotalPoints { get; set; }

        public decimal PracticalGrade { get; set; }

        public decimal UnionGrade { get; set; }

        public decimal FinalAverage { get; set; }

        public string MasteryLevel { get; set; } = string.Empty;

        public List<UpdatePracticalAnswerDto> PracticalAnswers { get; set; } = new();

        public List<UpdateUnionAnswerDto> UnionAnswers { get; set; } = new();

        public IFormFile? EvidencePhoto { get; set; }

        public IFormFile? SignatureColaborador { get; set; }

        public IFormFile? SignatureCoordinadorArea { get; set; }

        public IFormFile? SignatureCoordCapacitacion { get; set; }

        public IFormFile? SignatureSupervisor { get; set; }

        public IFormFile? SignatureEvaluador { get; set; }
    }

    public class UpdateWelderEvaluationCommandHandler(IUnitOfWork unitOfWork, IBlobStorageService blobStorageService) : IRequestHandler<UpdateWelderEvaluationCommand, int>
    {
        public async Task<int> Handle(UpdateWelderEvaluationCommand request, CancellationToken cancellationToken)
        {
            var eval = await unitOfWork.WelderEvaluations.GetByIdWithRelationsAsync(request.Id);
            if (eval == null) throw new Exception("No existe");

            eval.EvaluatorName = request.EvaluatorName;
            eval.TotalPoints = request.TotalPoints;
            eval.ExclusiveTestReference = request.ExclusiveTestReference;
            eval.TotalPoints = request.TotalPoints; 
            eval.FinalAverage = request.FinalAverage;
            eval.PracticalGrade = request.PracticalGrade;
            eval.UnionGrade = request.UnionGrade;
            eval.MasteryLevel = request.MasteryLevel;

            eval.PracticalAnswers.Clear();
            eval.UnionAnswers.Clear();

            foreach (var pa in request.PracticalAnswers)
            {
                eval.PracticalAnswers.Add(new WelderPracticalAnswer
                {
                    SectionTitle = pa.SectionTitle,
                    QuestionText = pa.QuestionText,
                    Score = pa.Score
                });
            }

            foreach (var ua in request.UnionAnswers)
            {
                eval.UnionAnswers.Add(new WelderUnionAnswer
                {
                    AttributeName = ua.AttributeName,
                    AnswerText = ua.AnswerText,
                    Score = ua.Score
                });
            }

            eval.EvidencePhotoUrl = await UploadIfPresent(request.EvidencePhoto, eval.EvidencePhotoUrl!);
            eval.SignatureColaboradorUrl = await UploadIfPresent(request.SignatureColaborador, eval.SignatureColaboradorUrl!);
            eval.SignatureCoordinadorAreaUrl = await UploadIfPresent(request.SignatureCoordinadorArea, eval.SignatureCoordinadorAreaUrl!);
            eval.SignatureCoordCapacitacionUrl = await UploadIfPresent(request.SignatureCoordCapacitacion, eval.SignatureCoordCapacitacionUrl!);
            eval.SignatureSupervisorUrl = await UploadIfPresent(request.SignatureSupervisor, eval.SignatureSupervisorUrl!);
            eval.SignatureEvaluadorUrl = await UploadIfPresent(request.SignatureEvaluador, eval.SignatureEvaluadorUrl!);

            unitOfWork.WelderEvaluations.Update(eval);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return eval.Id;
        }

        private async Task<string> UploadIfPresent(IFormFile? file, string currentUrl)
        {
            if (file == null) return currentUrl;

            if (!string.IsNullOrWhiteSpace(currentUrl))
            {
                await blobStorageService.DeleteFileWeldersAsync(currentUrl);
            }

            string fileName = $"{Guid.NewGuid()}_{file.FileName}";

            return await blobStorageService.UploadFileWeldersAsync(file, fileName);
        }
    }
}