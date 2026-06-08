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

        IGenericRepository<TrainingRoom> TrainingRooms { get; }

        IWelderEvaluationRepository WelderEvaluations { get; }

        IGenericRepository<WelderPracticalAnswer> WelderPracticalAnswers { get; }

        IGenericRepository<WelderUnionAnswer> WelderUnionAnswers { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}