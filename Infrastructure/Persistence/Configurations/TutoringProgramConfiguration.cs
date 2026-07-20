using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Infrastructure.Persistence.Configurations
{
    public class TutoringProgramConfiguration : IEntityTypeConfiguration<TutoringProgram>
    {
        public void Configure(EntityTypeBuilder<TutoringProgram> builder)
        {
            builder.HasKey(tp => tp.Id);

            builder.Property(tp => tp.CollaboratorName)
                .HasMaxLength(70)
                .IsRequired();

            builder.Property(tp => tp.Area)
                .HasMaxLength(60)
                .IsRequired();

            builder.Property(tp => tp.CreatedDate)
                .HasDefaultValueSql("GETDATE()");
        }
    }
}