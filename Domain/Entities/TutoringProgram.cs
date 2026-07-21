namespace Domain.Entities
{
    public class TutoringProgram
    {
        public int Id { get; set; }

        public int TutorId { get; set; }

        public string CollaboratorName { get; set; } = null!;

        public int PayrollNumber { get; set; }

        public string Area { get; set; } = null!;

        public int WeekId { get; set; }

        public DateTime CreatedDate { get; set; }

        public Tutors Tutor { get; set; } = null!;

        public FollowUpWeek Week { get; set; } = null!;

        public ICollection<Answer> Answers { get; set; } = new List<Answer>();
    }
}