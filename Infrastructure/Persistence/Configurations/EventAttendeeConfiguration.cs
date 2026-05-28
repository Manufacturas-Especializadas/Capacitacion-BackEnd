using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class EventAttendeeConfiguration : IEntityTypeConfiguration<EventAttendee>
    {
        public void Configure(EntityTypeBuilder<EventAttendee> builder)
        {
            builder.HasKey(ea => ea.Id);

            builder.HasIndex(ea => new { ea.EventId, ea.EmployeeId }).IsUnique();

            builder.Property(ea => ea.ParticipantSignatureUrl).HasMaxLength(1000);

            builder.HasOne(ea => ea.TrainingEvent)
                   .WithMany(te => te.Attendees)
                   .HasForeignKey(ea => ea.EventId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ea => ea.Employee)
                   .WithMany(e => e.EventAttendances)
                   .HasForeignKey(ea => ea.EmployeeId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}