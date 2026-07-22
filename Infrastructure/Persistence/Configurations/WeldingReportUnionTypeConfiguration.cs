using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class WeldingReportUnionTypeConfiguration : IEntityTypeConfiguration<WeldingReportUnionType>
    {
        public void Configure(EntityTypeBuilder<WeldingReportUnionType> builder)
        {
            builder.ToTable("WeldingReportUnionTypes");
            builder.HasKey(w => w.Id);

            builder.Property(w => w.Id).HasColumnName("id");
            builder.Property(w => w.ReportId).HasColumnName("reportId");
            builder.Property(w => w.ListNumber).HasColumnName("listNumber");
            builder.Property(w => w.UnionName).HasColumnName("unionName").HasMaxLength(100).IsRequired();
        }
    }
}