using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class TrainingReportConfiguration : IEntityTypeConfiguration<TrainingReport>
    {
        public void Configure(EntityTypeBuilder<TrainingReport> builder)
        {
            builder.ToTable("TrainingReports");
            builder.HasKey(tr => tr.Id);
            builder.Property(tr => tr.Id).HasColumnName("id");
            builder.Property(tr => tr.TrainingType).HasColumnName("trainingType").HasMaxLength(50).IsRequired();
            builder.Property(tr => tr.LeaderName).HasColumnName("leaderName").HasMaxLength(100).IsRequired();
            builder.Property(tr => tr.LeaderPayroll).HasColumnName("leaderPayroll").HasMaxLength(20).IsRequired();
            builder.Property(tr => tr.WeekNumber).HasColumnName("weekNumber");
            builder.Property(tr => tr.Observations).HasColumnName("observations").HasColumnType("VARCHAR(MAX)");

            builder.Property(tr => tr.InstructorSignatureUrl).HasColumnName("instructorSignatureUrl").HasColumnType("VARCHAR(MAX)");
            builder.Property(tr => tr.CoordinatorSignatureUrl).HasColumnName("coordinatorSignatureUrl").HasColumnType("VARCHAR(MAX)");
            builder.Property(tr => tr.SecuritySignatureUrl).HasColumnName("securitySignatureUrl").HasColumnType("VARCHAR(MAX)");

            builder.Property(tr => tr.CreatedAt).HasColumnName("createdAt").HasDefaultValueSql("GETDATE()");

            builder.HasMany(tr => tr.WeldingUnionTypes)
                   .WithOne(w => w.Report)
                   .HasForeignKey(w => w.ReportId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(tr => tr.Attendees)
                   .WithOne(a => a.Report)
                   .HasForeignKey(a => a.ReportId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}