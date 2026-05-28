using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Infrastructure.Persistence
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<ProductionLine> ProductionLines => Set<ProductionLine>();

        public DbSet<TrainingRoom> Trainings => Set<TrainingRoom>();

        public DbSet<Employee> Employees => Set<Employee>();

        public DbSet<TrainingEvent> TrainingEvents => Set<TrainingEvent>();

        public DbSet<EventTopic> EventTopics => Set<EventTopic>();

        public DbSet<EventAttendee> EventAttendees => Set<EventAttendee>();

        public DbSet<TopicEvaluation> TopicEvaluations => Set<TopicEvaluation>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}