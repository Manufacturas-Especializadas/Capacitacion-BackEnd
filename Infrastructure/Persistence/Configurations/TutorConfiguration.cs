using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Infrastructure.Persistence.Configurations
{
    public class TutorConfiguration : IEntityTypeConfiguration<Tutors>
    {
        public void Configure(EntityTypeBuilder<Tutors> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.TutorName);
        }
    }
}
