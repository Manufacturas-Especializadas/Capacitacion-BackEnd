using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ITrainingEventRepository TrainingEvents { get; }
        IEmployeeRepository Employees { get; }
        IProductionLineRepository ProductionLines { get; }

        IGenericRepository<EventAttendee> EventAttendees { get; }
        IGenericRepository<TopicEvaluation> TopicEvaluations { get; }

        IGenericRepository<TrainingTopic> TrainingTopics { get; }
        IGenericRepository<TrainingReport> TrainingReports { get; }

        IGenericRepository<WeldingReportUnionType> WeldingReportUnionTypes { get; }

        IGenericRepository<TrainingRoom> TrainingRooms { get; }

        IWelderEvaluationRepository WelderEvaluations { get; }

        IGenericRepository<WelderPracticalAnswer> WelderPracticalAnswers { get; }

        IGenericRepository<WelderUnionAnswer> WelderUnionAnswers { get; }

        IGenericRepository<Tutors> Tutors { get; }

        IGenericRepository<TutoringProgram> TutoringPrograms { get; }

        IGenericRepository<Answer> Answers { get; }

        IGenericRepository<Sections> Sections { get; }

        IGenericRepository<Question> Questions { get; }

        IGenericRepository<QuestionTypes> QuestionTypes { get; }

        IGenericRepository<QuestionOption> QuestionOptions { get; }

        IGenericRepository<OptionsCatalog> OptionsCatalogs { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}