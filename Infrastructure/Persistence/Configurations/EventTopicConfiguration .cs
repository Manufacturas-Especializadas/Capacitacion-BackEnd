using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class EventTopicConfiguration : IEntityTypeConfiguration<EventTopic>
    {
        public void Configure(EntityTypeBuilder<EventTopic> builder)
        {
            builder.HasKey(et => et.Id);

            builder.Property(et => et.TopicName).HasMaxLength(100).IsRequired();
            builder.HasOne(et => et.TrainingEvent)
                    .WithMany(te => te.Topics)
                    .HasForeignKey(et => et.EventId)
                    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}