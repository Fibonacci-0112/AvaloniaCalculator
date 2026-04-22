using Avalonia.Data.Converters;
using System.Globalization;

namespace PaycheckCalc.Avalonia.Helpers;

public sealed class GreaterThanZeroConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is decimal d && d > 0m;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
