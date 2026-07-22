using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Infrastructure.Persistence.Configurations
{
    public class OptionsCatalogConfiguration : IEntityTypeConfiguration<OptionsCatalog>
    {
        public void Configure(EntityTypeBuilder<OptionsCatalog> builder)
        {
            builder.ToTable("OptionsCatalog");

            builder.HasKey(o => o.Id);
        }
    }
}