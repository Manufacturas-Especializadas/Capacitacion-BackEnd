using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class TrainingReportAttendeeConfiguration : IEntityTypeConfiguration<TrainingReportAttendee>
    {
        public void Configure(EntityTypeBuilder<TrainingReportAttendee> builder)
        {
            builder.ToTable("TrainingReportAttendees");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).HasColumnName("id");
            builder.Property(a => a.ReportId).HasColumnName("reportId");
            builder.Property(a => a.EmployeeId).HasColumnName("employeeId");
            builder.Property(a => a.LineId).HasColumnName("lineId");
            builder.Property(a => a.DayMonday).HasColumnName("dayMonday").HasDefaultValue(false);
            builder.Property(a => a.DayTuesday).HasColumnName("dayTuesday").HasDefaultValue(false);
            builder.Property(a => a.DayWednesday).HasColumnName("dayWednesday").HasDefaultValue(false);
            builder.Property(a => a.DayThursday).HasColumnName("dayThursday").HasDefaultValue(false);
            builder.Property(a => a.DayFriday).HasColumnName("dayFriday").HasDefaultValue(false);
            builder.Property(a => a.DaySaturday).HasColumnName("daySaturday").HasDefaultValue(false);
            builder.Property(a => a.DaySunday).HasColumnName("daySunday").HasDefaultValue(false);

            builder.Property(a => a.CustomerClient).HasColumnName("customerClient").HasMaxLength(100);
            builder.Property(a => a.UnionClassification).HasColumnName("unionClassification").HasMaxLength(100);
            builder.Property(a => a.WeldingPercentage).HasColumnName("weldingPercentage").HasMaxLength(20);
            builder.Property(a => a.Diameter).HasColumnName("diameter").HasMaxLength(50);
            builder.Property(a => a.Shift).HasColumnName("shift").HasMaxLength(20);
            builder.Property(a => a.Machinery).HasColumnName("machinery").HasMaxLength(100);
            builder.Property(a => a.Ast).HasColumnName("ast").HasMaxLength(100);

            builder.Property(a => a.TraineeSignatureUrl).HasColumnName("traineeSignatureUrl").HasColumnType("VARCHAR(MAX)");
            builder.Property(a => a.SupervisorSignatureUrl).HasColumnName("supervisorSignatureUrl").HasColumnType("VARCHAR(MAX)");

            builder.HasOne(a => a.Employee)
                   .WithMany()
                   .HasForeignKey(a => a.EmployeeId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.ProductionLine)
                   .WithMany()
                   .HasForeignKey(a => a.LineId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(a => a.Topics)
                   .WithMany(t => t.Attendees)
                   .UsingEntity<Dictionary<string, object>>(
                       "AttendeeTrainingTopics",
                       j => j.HasOne<TrainingTopic>().WithMany().HasForeignKey("topicId").OnDelete(DeleteBehavior.NoAction),
                       j => j.HasOne<TrainingReportAttendee>().WithMany().HasForeignKey("attendeeId").OnDelete(DeleteBehavior.Cascade)
                   );
        }
    }
}