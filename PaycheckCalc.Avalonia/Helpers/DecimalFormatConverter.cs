using Avalonia.Data;
using Avalonia.Data.Converters;
using System.Globalization;

namespace PaycheckCalc.Avalonia.Helpers;

/// <summary>
/// Two-way converter that formats a <see cref="decimal"/> with two decimal
/// places (e.g. <c>40.00</c>) and parses user input back, accepting values
/// with or without thousands separators. Intended for hours / multiplier
/// fields that should read as decimal-format numbers.
/// </summary>
public sealed class DecimalFormatConverter : IValueConverter
{
    public static readonly DecimalFormatConverter Instance = new();

    private static readonly CultureInfo UsCulture = CultureInfo.GetCultureInfo("en-US");

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is decimal d)
            return d.ToString("0.00", UsCulture);
        if (value is double f)
            return f.ToString("0.00", UsCulture);
        return "0.00";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string s)
            return BindingOperations.DoNothing;

        var trimmed = s.Trim();
        if (trimmed.Length == 0)
            return 0m;

        const NumberStyles numberStyles =
            NumberStyles.Float |
            NumberStyles.AllowThousands;

        if (decimal.TryParse(trimmed, numberStyles, UsCulture, out var parsed))
            return parsed;

        return BindingOperations.DoNothing;
    }
}
