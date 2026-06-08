using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class WelderEvaluationConfiguration : IEntityTypeConfiguration<WelderEvaluation>
    {
        public void Configure(EntityTypeBuilder<WelderEvaluation> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.EvaluatorName).HasMaxLength(150).IsRequired();
            builder.Property(e => e.ExclusiveTestReference).HasMaxLength(100);
            builder.Property(e => e.TotalPoints);
            builder.Property(e => e.MasteryLevel).HasMaxLength(100);

            builder.HasOne(e => e.Employee)
                   .WithMany()
                   .HasForeignKey(e => e.EmployeeId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(e => e.PracticalGrade).HasColumnType("decimal(5,2)");
            builder.Property(e => e.UnionGrade).HasColumnType("decimal(5,2)");
            builder.Property(e => e.FinalAverage).HasColumnType("decimal(5,2)");
        }
    }
}