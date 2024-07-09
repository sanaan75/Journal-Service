using Journal_Service.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Journal_Service.Data;

public class JournalRecordConfig : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasIndex(i => i.Year);

        builder.Property(i => i.If).SetAsImpactFactor();
        builder.Property(i => i.Mif).SetAsImpactFactor();
        builder.Property(i => i.Aif).SetAsImpactFactor();
    }
}