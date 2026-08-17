using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class TrainingReportAttendeeTopicConfiguration
        : IEntityTypeConfiguration<TrainingReportAttendeeTopic>
    {
        public void Configure(
            EntityTypeBuilder<TrainingReportAttendeeTopic> builder)
        {
            builder.ToTable(
                "AttendeeTrainingTopics",
                table =>
                {


                    table.HasCheckConstraint(
                        "CK_AttendeeTrainingTopics_TotalHours",
                        "[totalHours] IS NULL OR " +
                        "([totalHours] >= 0 AND [totalHours] <= 56)"
                    );


                    table.HasCheckConstraint(
                        "CK_AttendeeTrainingTopics_HoursMonday",
                        "[hoursMonday] IS NULL OR " +
                        "([dayMonday] = 1 AND " +
                        "[hoursMonday] >= 0 AND [hoursMonday] <= 8)"
                    );

                    table.HasCheckConstraint(
                        "CK_AttendeeTrainingTopics_HoursTuesday",
                        "[hoursTuesday] IS NULL OR " +
                        "([dayTuesday] = 1 AND " +
                        "[hoursTuesday] >= 0 AND [hoursTuesday] <= 8)"
                    );

                    table.HasCheckConstraint(
                        "CK_AttendeeTrainingTopics_HoursWednesday",
                        "[hoursWednesday] IS NULL OR " +
                        "([dayWednesday] = 1 AND " +
                        "[hoursWednesday] >= 0 AND [hoursWednesday] <= 8)"
                    );

                    table.HasCheckConstraint(
                        "CK_AttendeeTrainingTopics_HoursThursday",
                        "[hoursThursday] IS NULL OR " +
                        "([dayThursday] = 1 AND " +
                        "[hoursThursday] >= 0 AND [hoursThursday] <= 8)"
                    );

                    table.HasCheckConstraint(
                        "CK_AttendeeTrainingTopics_HoursFriday",
                        "[hoursFriday] IS NULL OR " +
                        "([dayFriday] = 1 AND " +
                        "[hoursFriday] >= 0 AND [hoursFriday] <= 8)"
                    );

                    table.HasCheckConstraint(
                        "CK_AttendeeTrainingTopics_HoursSaturday",
                        "[hoursSaturday] IS NULL OR " +
                        "([daySaturday] = 1 AND " +
                        "[hoursSaturday] >= 0 AND [hoursSaturday] <= 8)"
                    );

                    table.HasCheckConstraint(
                        "CK_AttendeeTrainingTopics_HoursSunday",
                        "[hoursSunday] IS NULL OR " +
                        "([daySunday] = 1 AND " +
                        "[hoursSunday] >= 0 AND [hoursSunday] <= 8)"
                    );
                }
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



            builder.Property(x => x.DayMonday)
                .HasColumnName("dayMonday")
                .HasDefaultValue(false);

            builder.Property(x => x.DayTuesday)
                .HasColumnName("dayTuesday")
                .HasDefaultValue(false);

            builder.Property(x => x.DayWednesday)
                .HasColumnName("dayWednesday")
                .HasDefaultValue(false);

            builder.Property(x => x.DayThursday)
                .HasColumnName("dayThursday")
                .HasDefaultValue(false);

            builder.Property(x => x.DayFriday)
                .HasColumnName("dayFriday")
                .HasDefaultValue(false);

            builder.Property(x => x.DaySaturday)
                .HasColumnName("daySaturday")
                .HasDefaultValue(false);

            builder.Property(x => x.DaySunday)
                .HasColumnName("daySunday")
                .HasDefaultValue(false);


            builder.Property(x => x.HoursMonday)
                .HasColumnName("hoursMonday")
                .HasPrecision(4, 2);

            builder.Property(x => x.HoursTuesday)
                .HasColumnName("hoursTuesday")
                .HasPrecision(4, 2);

            builder.Property(x => x.HoursWednesday)
                .HasColumnName("hoursWednesday")
                .HasPrecision(4, 2);

            builder.Property(x => x.HoursThursday)
                .HasColumnName("hoursThursday")
                .HasPrecision(4, 2);

            builder.Property(x => x.HoursFriday)
                .HasColumnName("hoursFriday")
                .HasPrecision(4, 2);

            builder.Property(x => x.HoursSaturday)
                .HasColumnName("hoursSaturday")
                .HasPrecision(4, 2);

            builder.Property(x => x.HoursSunday)
                .HasColumnName("hoursSunday")
                .HasPrecision(4, 2);


            builder.Property(x => x.TotalHours)
                .HasColumnName("totalHours")
                .HasPrecision(4, 2);



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