using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class WelderUnionAnswerConfiguration : IEntityTypeConfiguration<WelderUnionAnswer>
    {
        public void Configure(EntityTypeBuilder<WelderUnionAnswer> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.AttributeName).HasMaxLength(100).IsRequired();
            builder.Property(e => e.AnswerText).HasMaxLength(255);
            builder.Property(e => e.Score).HasColumnType("decimal(5,2)");

            builder.HasOne(e => e.Evaluation)
                   .WithMany(e => e.UnionAnswers)
                   .HasForeignKey(e => e.EvaluationId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}