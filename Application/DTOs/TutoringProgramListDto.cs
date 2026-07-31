namespace Application.DTOs
{
    public class TutoringProgramListDto
    {
        public int Id { get; set; }

        public int TutorId { get; set; }

        public string TutorName { get; set; } = string.Empty;

        public string CollaboratorName { get; set; } = null!;

        public int PayrollNumber { get; set; }

        public string Area { get; set; } = null!;

        public int WeekId { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}