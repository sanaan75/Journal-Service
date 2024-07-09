using Journal_Service.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Journal_Service.Data;

public class JournalConfig : IEntityTypeConfiguration<Journal>
{
    public void Configure(EntityTypeBuilder<Journal> builder)
    {
        builder.Property(i => i.Issn).HasMaxLength(8);
        builder.HasIndex(i => i.Issn).IsUnique().HasFilter("[Issn] IS NOT NULL");
        
        builder.Property(i => i.EIssn).HasMaxLength(8);
        builder.HasIndex(i => i.EIssn).IsUnique().HasFilter("[EIssn] IS NOT NULL");
    }
}