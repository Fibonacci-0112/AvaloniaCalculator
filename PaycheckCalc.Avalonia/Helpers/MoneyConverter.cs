using Avalonia.Data;
using Avalonia.Data.Converters;
using System.Globalization;

namespace PaycheckCalc.Avalonia.Helpers;

/// <summary>
/// Two-way converter for money <see cref="decimal"/> values bound to a
/// <see cref="string"/> (typically a <c>TextBox.Text</c>).
/// <para>
/// <b>Convert</b> (source → target): formats the decimal as
/// <c>$X.XX</c> (en-US, always with a "$" prefix) so every page renders
/// money values identically.
/// </para>
/// <para>
/// <b>ConvertBack</b> (target → source): accepts user input with or
/// without the currency symbol, thousand separators, or parentheses for
/// negatives (e.g. <c>"$1,234.56"</c>, <c>"1234.56"</c>, <c>"(12.34)"</c>).
/// Returns <see cref="BindingOperations.DoNothing"/> on unparseable
/// input so the source decimal stays untouched and the field does not
/// raise a validation error while the user is still typing.
/// </para>
/// </summary>
public sealed class MoneyConverter : IValueConverter
{
    public static readonly MoneyConverter Instance = new();

    private static readonly CultureInfo UsCulture = CultureInfo.GetCultureInfo("en-US");

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is decimal d)
            return d.ToString("C2", UsCulture);
        if (value is double f)
            return ((decimal)f).ToString("C2", UsCulture);
        return "$0.00";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string s)
            return BindingOperations.DoNothing;

        var trimmed = s.Trim();
        if (trimmed.Length == 0)
            return 0m;

        const NumberStyles currencyStyles =
            NumberStyles.Currency |
            NumberStyles.AllowThousands |
            NumberStyles.AllowParentheses |
            NumberStyles.AllowLeadingSign;

        if (decimal.TryParse(trimmed, currencyStyles, UsCulture, out var parsed))
            return decimal.Round(parsed, 2, MidpointRounding.AwayFromZero);

        return BindingOperations.DoNothing;
    }
}
