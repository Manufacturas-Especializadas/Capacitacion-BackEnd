namespace Application.DTOs
{
    public class WelderEvaluationDetailDto
    {
        public int Id { get; set; }

        public string EmployeeNumber { get; set; } = string.Empty;

        public string EmployeeName { get; set; } = string.Empty;

        public DateTime EvaluationDate { get; set; }

        public decimal? FinalAverage { get; set; }

        public string? EvidencePhotoUrl { get; set; }

        public string? SignatureColaboradorUrl { get; set; }

        public string? SignatureCoordinadorAreaUrl { get; set; }

        public string? SignatureCoordCapacitacionUrl { get; set; }

        public string? SignatureSupervisorUrl { get; set; }

        public string? SignatureEvaluadorUrl { get; set; }

        public string MasteryLevel { get; set; } = string.Empty;

        public List<PracticalAnswerDto> PracticalAnswers { get; set; } = new();

        public List<UnionAnswerDto> UnionAnswers { get; set; } = new();
    }
}