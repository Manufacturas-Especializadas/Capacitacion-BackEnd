using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Infrastructure.Persistence
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<ProductionLine> ProductionLines => Set<ProductionLine>();

        public DbSet<TrainingRoom> TrainingRooms => Set<TrainingRoom>();

        public DbSet<Employee> Employees => Set<Employee>();

        public DbSet<TrainingEvent> TrainingEvents => Set<TrainingEvent>();

        public DbSet<EventTopic> EventTopics => Set<EventTopic>();

        public DbSet<EventAttendee> EventAttendees => Set<EventAttendee>();

        public DbSet<TopicEvaluation> TopicEvaluations => Set<TopicEvaluation>();

        public DbSet<WelderEvaluation> WelderEvaluations => Set<WelderEvaluation>();

        public DbSet<WelderPracticalAnswer> WelderPracticalAnswers => Set<WelderPracticalAnswer>();

        public DbSet<WelderUnionAnswer> WelderUnionAnswers => Set<WelderUnionAnswer>();

        public DbSet<Tutors> Tutors => Set<Tutors>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}