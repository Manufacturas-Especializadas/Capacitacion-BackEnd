using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class WelderPracticalAnswerConfiguration : IEntityTypeConfiguration<WelderPracticalAnswer>
    {
        public void Configure(EntityTypeBuilder<WelderPracticalAnswer> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.SectionTitle).HasMaxLength(150).IsRequired();
            builder.Property(e => e.QuestionText).HasMaxLength(500).IsRequired();

            builder.HasOne(e => e.Evaluation)
                   .WithMany(e => e.PracticalAnswers)
                   .HasForeignKey(e => e.EvaluationId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}