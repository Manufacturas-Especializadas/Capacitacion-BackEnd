using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories
{
    public class UnitOfWork(
        ApplicationDbContext context,
        ITrainingEventRepository trainingEvents,
        IEmployeeRepository employees,
        IProductionLineRepository productionLines,
        IWelderEvaluationRepository welderEvaluations, 
        IGenericRepository<EventAttendee> eventAttendees,
        IGenericRepository<TopicEvaluation> topicEvaluations,
        IGenericRepository<TrainingRoom>? trainingRooms,
        IGenericRepository<TrainingTopic> trainingTopics,
        IGenericRepository<TrainingReport> trainingReports,
        IGenericRepository<WelderPracticalAnswer> welderPracticalAnswer,
        IGenericRepository<WelderUnionAnswer> welderUnionAnswer) : IUnitOfWork
    {
        private readonly ApplicationDbContext _context = context;

        public ITrainingEventRepository TrainingEvents { get; } = trainingEvents;
        public IEmployeeRepository Employees { get; } = employees;
        public IProductionLineRepository ProductionLines { get; } = productionLines;

        public IWelderEvaluationRepository WelderEvaluations { get; } = welderEvaluations;

        public IGenericRepository<TrainingTopic> TrainingTopics {  get; } = trainingTopics;

        public IGenericRepository<TrainingReport> TrainingReports { get;  } = trainingReports;

        public IGenericRepository<EventAttendee> EventAttendees { get; } = eventAttendees;
        public IGenericRepository<TopicEvaluation> TopicEvaluations { get; } = topicEvaluations;
        public IGenericRepository<TrainingRoom>? TrainingRooms { get; } = trainingRooms;
        public IGenericRepository<WelderPracticalAnswer> WelderPracticalAnswers { get; } = welderPracticalAnswer;
        public IGenericRepository<WelderUnionAnswer> WelderUnionAnswers { get; } = welderUnionAnswer;

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}