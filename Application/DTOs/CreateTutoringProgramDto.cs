namespace Application.DTOs
{
    public class CreateTutoringProgramDto
    {
        public int TutorId { get; set; }

        public string CollaboratorName { get; set; } = null!;

        public int PayrollNumber { get; set; }

        public string Area { get; set; } = null!;

        public int WeekId { get; set; }

        public List<TutoringAnswerDto> Answers { get; set; } = new();
    }
}