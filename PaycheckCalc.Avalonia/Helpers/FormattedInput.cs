using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System.Globalization;

namespace PaycheckCalc.Avalonia.Helpers;

/// <summary>
/// Attached property that improves the editing experience for
/// money / decimal <see cref="TextBox"/> inputs used alongside
/// <see cref="MoneyConverter"/> or <see cref="DecimalFormatConverter"/>.
/// <para>
/// When <c>FormattedInput.Mode</c> is set on a <see cref="TextBox"/>:
/// </para>
/// <list type="bullet">
///   <item>
///     On <c>GotFocus</c> the raw unformatted value is displayed so the
///     user can edit digits directly without having to delete the
///     leading <c>$</c> or trailing decimals.
///   </item>
///   <item>
///     On <c>LostFocus</c> the value is re-formatted using the current
///     binding so the field consistently re-renders as
///     <c>$22.00</c> (money) or <c>40.00</c> (decimal), even if the
///     underlying source value did not change.
///   </item>
/// </list>
/// </summary>
public static class FormattedInput
{
    public enum FormatKind
    {
        None,
        Money,
        Decimal
    }

    /// <summary>
    /// Attached property selecting which formatting mode to apply to a
    /// <see cref="TextBox"/>. Defaults to <see cref="FormatKind.None"/>
    /// (no-op).
    /// </summary>
    public static readonly AttachedProperty<FormatKind> ModeProperty =
        AvaloniaProperty.RegisterAttached<TextBox, FormatKind>(
            "Mode", typeof(FormattedInput), FormatKind.None);

    public static FormatKind GetMode(TextBox element) => element.GetValue(ModeProperty);
    public static void SetMode(TextBox element, FormatKind value) => element.SetValue(ModeProperty, value);

    private static readonly CultureInfo UsCulture = CultureInfo.GetCultureInfo("en-US");

    static FormattedInput()
    {
        ModeProperty.Changed.AddClassHandler<TextBox>(OnModeChanged);
    }

    private static void OnModeChanged(TextBox textBox, AvaloniaPropertyChangedEventArgs args)
    {
        // Detach previous handlers regardless — safe no-op if never attached.
        textBox.GotFocus -= OnGotFocus;
        textBox.LostFocus -= OnLostFocus;

        if (args.NewValue is FormatKind kind && kind != FormatKind.None)
        {
            textBox.GotFocus += OnGotFocus;
            textBox.LostFocus += OnLostFocus;
        }
    }

    private static void OnGotFocus(object? sender, RoutedEventArgs e)
{
    if (sender is not TextBox tb) return;
    var mode = GetMode(tb);
    if (mode == FormatKind.None) return;

    var text = tb.Text ?? string.Empty;
    var raw = text.Replace("$", "", StringComparison.Ordinal)
                  .Replace(",", "", StringComparison.Ordinal)
                  .Trim();

    if (!string.Equals(raw, text, StringComparison.Ordinal))
    {
        tb.Text = raw;
        tb.SelectAll();
    }
}

    private static void OnLostFocus(object? sender, RoutedEventArgs e)
    {
        if (sender is not TextBox tb) return;
        var mode = GetMode(tb);
        if (mode == FormatKind.None) return;

        // Force a reformat using the *current* text in the box. This
        // guarantees the field redisplays as "$22.00" or "40.00" even
        // when the bound value did not change (i.e. the binding did not
        // trigger a target refresh of its own).
        Dispatcher.UIThread.Post(() =>
        {
            var text = (tb.Text ?? string.Empty).Trim();
            if (text.Length == 0)
            {
                tb.Text = mode == FormatKind.Money ? "$0.00" : "0.00";
                return;
            }

            var cleaned = text.Replace("$", "", StringComparison.Ordinal)
                              .Replace(",", "", StringComparison.Ordinal);
            if (!decimal.TryParse(cleaned, NumberStyles.Float, UsCulture, out var parsed))
                return; // leave user-entered garbage so they can fix it

            tb.Text = mode switch
            {
                FormatKind.Money => parsed.ToString("C2", UsCulture),
                FormatKind.Decimal => parsed.ToString("0.00", UsCulture),
                _ => tb.Text
            };
        }, DispatcherPriority.Background);
    }
}
