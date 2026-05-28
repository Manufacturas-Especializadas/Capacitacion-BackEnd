using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class TopicEvaluationConfiguration : IEntityTypeConfiguration<TopicEvaluation>
    {
        public void Configure(EntityTypeBuilder<TopicEvaluation> builder)
        {
            builder.HasKey(te => te.Id);

            builder.HasIndex(te => new { te.AttendeeId, te.TopicId }).IsUnique();

            builder.Property(te => te.Grade).HasPrecision(5, 2);

            builder.Property(te => te.AttendanceStatus)
                   .HasMaxLength(20)
                   .HasDefaultValue("EMPTY");

            builder.HasOne(te => te.Attendee)
                   .WithMany(ea => ea.Evaluations)
                   .HasForeignKey(te => te.AttendeeId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(te => te.Topic)
                   .WithMany(et => et.Evaluations)
                   .HasForeignKey(te => te.TopicId)
                   .OnDelete(DeleteBehavior.ClientCascade);
        }
    }
}