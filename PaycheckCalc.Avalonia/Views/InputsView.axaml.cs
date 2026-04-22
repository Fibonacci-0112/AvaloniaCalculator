using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using PaycheckCalc.Avalonia.ViewModels;
using System.Threading.Tasks;

namespace PaycheckCalc.Avalonia.Views;

public partial class InputsView : UserControl
{
    public InputsView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Handles the "Save Paycheck" button. When no paycheck is currently
    /// loaded, prompts for a name via a small modal dialog and delegates
    /// to <see cref="CalculatorViewModel.SaveWithNameAsync"/>. Otherwise
    /// overwrites the existing entry via <see cref="CalculatorViewModel.SaveCurrentAsync"/>.
    /// </summary>
    private async void SavePaycheckButton_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not CalculatorViewModel vm) return;

        if (vm.HasLoadedPaycheck)
        {
            await vm.SaveCurrentAsync();
            return;
        }

        var name = await PromptForNameAsync("Save Paycheck", "Enter a name for this paycheck:");
        if (string.IsNullOrWhiteSpace(name)) return;
        await vm.SaveWithNameAsync(name.Trim());
    }

    /// <summary>
    /// Minimal owner-modal prompt dialog. Avalonia desktop has no built-in
    /// input dialog so we compose a small Window ad hoc.
    /// </summary>
    private async Task<string?> PromptForNameAsync(string title, string message)
    {
        var owner = TopLevel.GetTopLevel(this) as Window;
        if (owner is null) return null;

        var textBox = new TextBox { Width = 260 };
        string? result = null;

        var okButton = new Button { Content = "Save", IsDefault = true, MinWidth = 80 };
        var cancelButton = new Button { Content = "Cancel", IsCancel = true, MinWidth = 80 };

        var dialog = new Window
        {
            Title = title,
            Width = 320,
            Height = 170,
            CanResize = false,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            SizeToContent = SizeToContent.Height,
            Content = new StackPanel
            {
                Margin = new Avalonia.Thickness(16),
                Spacing = 8,
                Children =
                {
                    new TextBlock { Text = message },
                    textBox,
                    new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Spacing = 8,
                        Children = { okButton, cancelButton }
                    }
                }
            }
        };

        okButton.Click += (_, _) =>
        {
            result = textBox.Text;
            dialog.Close();
        };
        cancelButton.Click += (_, _) =>
        {
            result = null;
            dialog.Close();
        };

        dialog.Opened += (_, _) => textBox.Focus();
        await dialog.ShowDialog(owner);
        return result;
    }
}
