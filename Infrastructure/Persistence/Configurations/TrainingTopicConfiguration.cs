using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class TrainingTopicConfiguration : IEntityTypeConfiguration<TrainingTopic>
    {
        public void Configure(EntityTypeBuilder<TrainingTopic> builder)
        {
            builder.ToTable("TrainingTopics");
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id).HasColumnName("id");
            builder.Property(t => t.TrainingType).HasColumnName("trainingType").HasMaxLength(50).IsRequired();
            builder.Property(t => t.TopicCode).HasColumnName("topicCode").HasMaxLength(10).IsRequired();
            builder.Property(t => t.TopicName).HasColumnName("topicName").HasMaxLength(200).IsRequired();
        }
    }
}
