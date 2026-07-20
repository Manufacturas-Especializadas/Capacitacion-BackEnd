using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Infrastructure.Persistence.Configurations
{
    public class QuestionOptionConfiguration : IEntityTypeConfiguration<QuestionOption>
    {
        public void Configure(EntityTypeBuilder<QuestionOption> builder)
        {
            builder.HasKey(qo => new { qo.QuestionId, qo.OptionId });

            builder.HasOne(qo => qo.Question)
                .WithMany(q => q.QuestionOptions)
                .HasForeignKey(qo => qo.QuestionId);

            builder.HasOne(qo => qo.Option)
                .WithMany(o => o.QuestionOptions)
                .HasForeignKey(qo => qo.OptionId);
        }
    }
}