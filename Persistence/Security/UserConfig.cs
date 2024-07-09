using Entities.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Security;

public class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(i => i.Username).HasMaxLength(50);
        builder.Property(i => i.Username).IsRequired();
        
        builder.HasIndex(i => i.Username).IsUnique();
    }
}