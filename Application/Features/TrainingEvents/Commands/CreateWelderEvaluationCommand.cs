using Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.WelderEvaluations.Commands
{
    public class CreateWelderEvaluationCommand : IRequest<int>
    {
        public string EmployeeNumber { get; set; } = string.Empty;

        public DateTime EvaluationDate { get; set; }

        public string EvaluatorName { get; set; } = string.Empty;

        public string ExclusiveTestReference { get; set; } = "Sin requerimiento";

        public string? ExclusiveTestResult { get; set; }

        public decimal? PracticalGrade { get; set; }

        public decimal? UnionGrade { get; set; }

        public decimal? FinalAverage { get; set; }

        public string? MasteryLevel { get; set; }

        public List<PracticalAnswerDto> PracticalAnswers { get; set; } = new();

        public List<UnionAnswerDto> UnionAnswers { get; set; } = new();

        public IFormFile? EvidencePhoto { get; set; }

        public IFormFile? SignatureColaborador { get; set; }

        public IFormFile? SignatureCoordinadorArea { get; set; }

        public IFormFile? SignatureCoordCapacitacion { get; set; }

        public IFormFile? SignatureSupervisor { get; set; }

        public IFormFile? SignatureEvaluador { get; set; }
    }
}