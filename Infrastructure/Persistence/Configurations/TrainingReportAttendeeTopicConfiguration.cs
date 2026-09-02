using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class TrainingReportAttendeeTopicConfiguration
        : IEntityTypeConfiguration<TrainingReportAttendeeTopic>
    {
        public void Configure(
            EntityTypeBuilder<TrainingReportAttendeeTopic> builder
        )
        {
            builder.ToTable(
                "AttendeeTrainingTopics"
            );

            builder.HasKey(x => new
            {
                x.AttendeeId,
                x.TopicId
            });

            builder.Property(x => x.AttendeeId)
                .HasColumnName("attendeeId");

            builder.Property(x => x.TopicId)
                .HasColumnName("topicId");

            builder.HasOne(x => x.Attendee)
                .WithMany(x => x.Topics)
                .HasForeignKey(x => x.AttendeeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Topic)
                .WithMany(x => x.Attendees)
                .HasForeignKey(x => x.TopicId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}