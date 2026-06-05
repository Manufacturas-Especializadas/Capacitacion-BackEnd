namespace Application.DTOs
{
    public class WelderEvaluationDetailDto
    {
        public int Id { get; set; }

        public string EmployeeNumber { get; set; } = string.Empty;

        public string EmployeeName { get; set; } = string.Empty;

        public DateTime EvaluationDate { get; set; }

        public decimal FinalAverage { get; set; }

        public string MasteryLevel { get; set; } = string.Empty;
    }
}