using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasIndex(e => e.EmployeeNumber).IsUnique();

            builder.Property(e => e.EmployeeNumber).HasMaxLength(20).IsRequired();

            builder.HasOne(e => e.ProductionLine)
                    .WithMany(pl => pl.Employee)
                    .HasForeignKey(e => e.LineId)
                    .OnDelete(DeleteBehavior.Restrict);
        }
    }
}