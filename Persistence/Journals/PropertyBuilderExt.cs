using Entities.Journals;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Journals;

public static class PropertyBuilderExt
{
    public static PropertyBuilder<TProperty> SetAsImpactFactor<TProperty>(this PropertyBuilder<TProperty> builder)
    {
        return builder.HasPrecision(18, JournalRecord.ImpactFactorPrecision);
    }
}