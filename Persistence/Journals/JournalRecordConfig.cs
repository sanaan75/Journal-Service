using Entities.Journals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Journals;

public class JournalRecordConfig : IEntityTypeConfiguration<JournalRecord>
{
    public void Configure(EntityTypeBuilder<JournalRecord> builder)
    {
        builder.HasIndex(i => i.Year);

        builder.Property(i => i.JournalIf).SetAsImpactFactor();
        builder.Property(i => i.JournalMif).SetAsImpactFactor();
        builder.Property(i => i.Aif).SetAsImpactFactor();
    }
}