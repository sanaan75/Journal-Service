using Journal_Service.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Journal_Service.Data;

public static class PropertyBuilderExt
{
    public static PropertyBuilder<TProperty> SetAsImpactFactor<TProperty>(this PropertyBuilder<TProperty> builder)
    {
        return builder.HasPrecision(18, Category.ImpactFactorPrecision);
    }
}