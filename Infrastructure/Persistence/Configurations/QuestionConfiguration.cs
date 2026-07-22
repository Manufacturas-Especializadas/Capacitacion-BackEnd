using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Infrastructure.Persistence.Configurations
{
    public class QuestionConfiguration : IEntityTypeConfiguration<Question>
    {
        public void Configure(EntityTypeBuilder<Question> builder)
        {
            builder.HasKey(q => q.Id);

            builder.HasOne(q => q.ParentQuestion)
                .WithMany(q => q.ChildQuestions)
                .HasForeignKey(q => q.ParentQuestionId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(q => q.ShowWhenOption)
                .WithMany()
                .HasForeignKey(q => q.ShowWhenOptionId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}