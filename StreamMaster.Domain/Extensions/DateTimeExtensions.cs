using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace StreamMaster.Domain.Extensions;

public static class SMDT
{
    public static DateTime UtcNow => DateTime.UtcNow.SetKindUtc();
    public static DateTime? SetKindUtc(this DateTime? dateTime)
    {
        return dateTime.HasValue ? dateTime.Value.SetKindUtc() : null;
    }
    public static DateTime SetKindUtc(this DateTime dateTime)
    {
        return dateTime.Kind == DateTimeKind.Utc ? dateTime : DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
    }
}

public static class UtcDateAnnotation
{
    private const string IsUtcAnnotation = "IsUtc";
    private static readonly ValueConverter<DateTime, DateTime> UtcConverter = new(convertTo => DateTime.SpecifyKind(convertTo, DateTimeKind.Utc), convertFrom => convertFrom);

    public static PropertyBuilder<TProperty> IsUtc<TProperty>(this PropertyBuilder<TProperty> builder, bool isUtc = true)
    {
        return builder.HasAnnotation(IsUtcAnnotation, isUtc);
    }

    //public static bool IsUtc(this IMutableProperty property)
    //{

    //    return ((bool?)property.FindAnnotation(IsUtcAnnotation)?.Value) ?? true);

    //}

    /// <summary>
    /// Make sure this is called after configuring all your entities.
    /// </summary>
    public static void ApplyUtcDateTimeConverter(this ModelBuilder builder)
    {
        foreach (IMutableEntityType entityType in builder.Model.GetEntityTypes())
        {
            foreach (IMutableProperty property in entityType.GetProperties())
            {
                //if (!property.IsUtc())
                //{
                //    continue;
                //}

                if (property.ClrType == typeof(DateTime) ||
                    property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(UtcConverter);
                }
            }
        }
    }
}
