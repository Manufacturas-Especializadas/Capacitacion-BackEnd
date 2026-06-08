using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;


namespace Application.Features.TrainingEvents.Commands
{
    public class UpdateWelderEvaluationCommand : IRequest<int>
    {
        public int Id { get; set; }
        public string EmployeeNumber { get; set; } = string.Empty;

        public string EvaluatorName { get; set; } = string.Empty;

        public string ExclusiveTestReference { get; set; } = string.Empty;

        public int TotalPoints { get; set; }

        public decimal PracticalGrade { get; set; }

        public decimal UnionGrade { get; set; }

        public decimal FinalAverage { get; set; }

        public string MasteryLevel { get; set; } = string.Empty;

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
            eval.FinalAverage = request.FinalAverage;
            eval.PracticalGrade = request.PracticalGrade;
            eval.UnionGrade = request.UnionGrade;
            eval.MasteryLevel = request.MasteryLevel;

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