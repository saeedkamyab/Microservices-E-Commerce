namespace Catalog.Application.Common;

public static class StringToEnumConvertor
{
    public static TEnum? ToNullableEnum<TEnum>(string? value)
       where TEnum : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        return Enum.TryParse<TEnum>(value, true, out var result)
            ? result
            : null;
    }
    public static TEnum ToEnum<TEnum>(
       string? value,
       TEnum defaultValue = default)
       where TEnum : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value))
            return defaultValue;

        return Enum.TryParse<TEnum>(value, true, out var result)
            ? result
            : defaultValue;
    }
}
