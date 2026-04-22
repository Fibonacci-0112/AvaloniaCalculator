using Avalonia.Controls;
using PaycheckCalc.Avalonia.ViewModels;
using PaycheckCalc.Avalonia.Views;
using System;

namespace PaycheckCalc.Avalonia;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is MainWindowViewModel vm)
        {
            ShowPage(0, vm);
        }
    }

    private void NavList_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is ListBox lb && lb.SelectedIndex >= 0 && DataContext is MainWindowViewModel vm)
        {
            ShowPage(lb.SelectedIndex, vm);
        }
    }

    private void ShowPage(int index, MainWindowViewModel vm)
    {
        ContentArea.Content = index switch
        {
            0 => new InputsView { DataContext = vm.Calculator },
            1 => new SelfEmploymentView { DataContext = vm.SelfEmployment },
            2 => new SavedPaychecksView { DataContext = vm.SavedPaychecks },
            3 => new AnnualTaxView { DataContext = vm.AnnualTax },
            4 => new AnnualProjectionView { DataContext = vm.AnnualProjection },
            5 => new CompareView { DataContext = vm.Compare },
            6 => new CreditsView { DataContext = vm.Credits },
            7 => new JobsAndYtdView { DataContext = vm.JobsAndYtd },
            8 => new OtherIncomeAdjustmentsView { DataContext = vm.OtherIncomeAdjustments },
            9 => new QuarterlyEstimatesView { DataContext = vm.QuarterlyEstimates },
            10 => new WhatIfView { DataContext = vm.WhatIf },
            _ => (object?)null
        };
    }
}
