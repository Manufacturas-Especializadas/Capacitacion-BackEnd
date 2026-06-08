namespace Domain.Entities
{
    public class WelderEvaluation
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public Employee? Employee { get; set; }

        public DateTime EvaluationDate { get; set; }
        public string EvaluatorName { get; set; } = string.Empty;

        public string ExclusiveTestReference { get; set; } = "Sin requerimiento";

        public int? TotalPoints { get; set; }

        public string? EvidencePhotoUrl { get; set; }

        public string? SignatureColaboradorUrl { get; set; }

        public string? SignatureCoordinadorAreaUrl { get; set; }

        public string? SignatureCoordCapacitacionUrl { get; set; }

        public string? SignatureSupervisorUrl { get; set; }

        public string? SignatureEvaluadorUrl { get; set; }

        public decimal? PracticalGrade { get; set; }

        public decimal? UnionGrade { get; set; }

        public decimal? FinalAverage { get; set; }

        public string? MasteryLevel { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<WelderPracticalAnswer> PracticalAnswers { get; set; } = new List<WelderPracticalAnswer>();

        public ICollection<WelderUnionAnswer> UnionAnswers { get; set; } = new List<WelderUnionAnswer>();
    }
}