using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using static System.Collections.Specialized.BitVector32;

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
        IGenericRepository<WeldingReportUnionType> weldingReportUnionTypes,
        IGenericRepository<Tutors> tutors,
        IGenericRepository<TutoringProgram> tutoringPrograms,
        IGenericRepository<Answer> answers,
        IGenericRepository<WelderUnionAnswer> welderUnionAnswer,
        IGenericRepository<Sections> sections,
        IGenericRepository<Question> questions,
        IGenericRepository<QuestionTypes> questionTypes,
        IGenericRepository<QuestionOption> questionOptions,
        IGenericRepository<OptionsCatalog> optionsCatalogs,
        IGenericRepository<FollowUpWeek> followUpWeek,
        IGenericRepository<User> users) : IUnitOfWork
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

        public IGenericRepository<WeldingReportUnionType> WeldingReportUnionTypes { get; } = weldingReportUnionTypes;

        public IGenericRepository<Tutors> Tutors { get; } = tutors;

        public IGenericRepository<TutoringProgram> TutoringPrograms { get; } = tutoringPrograms;

        public IGenericRepository<Answer> Answers { get; } = answers;

        public IGenericRepository<Sections> Sections { get; } = sections;

        public IGenericRepository<Question> Questions { get; } = questions;

        public IGenericRepository<QuestionTypes> QuestionTypes { get; } = questionTypes;

        public IGenericRepository<QuestionOption> QuestionOptions { get; } = questionOptions;

        public IGenericRepository<OptionsCatalog> OptionsCatalogs { get; } = optionsCatalogs;

        public IGenericRepository<FollowUpWeek> FollowUpWeek { get; } = followUpWeek;

        public IGenericRepository<User> Users { get; } = users;

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