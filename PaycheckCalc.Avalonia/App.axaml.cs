using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using PaycheckCalc.Avalonia.Services;
using PaycheckCalc.Avalonia.Storage;
using PaycheckCalc.Avalonia.ViewModels;
using PaycheckCalc.Core.Pay;
using PaycheckCalc.Core.Storage;
using PaycheckCalc.Core.Tax.Federal.Annual;
using PaycheckCalc.Core.Tax.State;
using PaycheckCalc.Core.Tax.SelfEmployment;
using System;
using System.IO;

namespace PaycheckCalc.Avalonia;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        Services = services.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = Services.GetRequiredService<MainWindowViewModel>()
            };
        }
        base.OnFrameworkInitializationCompleted();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        var dataDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "PaycheckCalc");
        Directory.CreateDirectory(dataDir);

        services.AddSingleton<IPaycheckRepository>(_ => new JsonPaycheckRepository(dataDir));
        services.AddSingleton<IAnnualScenarioRepository>(_ => new JsonAnnualScenarioRepository(dataDir));
        services.AddSingleton<AnnualTaxSession>();
        services.AddSingleton<ComparisonSession>();
        services.AddHttpClient<GoogleMapsGeocodingService>();
        services.AddSingleton<JurisdictionResolver>();

        // Core calculators
        services.AddSingleton<StateCalculatorRegistry>();
        services.AddSingleton<PayCalculator>();
        services.AddSingleton<AnnualProjectionCalculator>();
        services.AddSingleton<SelfEmploymentCalculator>();
        services.AddSingleton<Form1040Calculator>();
        services.AddSingleton<Form1040ESCalculator>();

        // ViewModels
        services.AddTransient<CalculatorViewModel>();
        services.AddTransient<SelfEmploymentViewModel>();
        services.AddTransient<SavedPaychecksViewModel>();
        services.AddTransient<AnnualTaxViewModel>();
        services.AddTransient<AnnualProjectionViewModel>();
        services.AddTransient<CompareViewModel>();
        services.AddTransient<CreditsViewModel>();
        services.AddTransient<JobsAndYtdViewModel>();
        services.AddTransient<OtherIncomeAdjustmentsViewModel>();
        services.AddTransient<QuarterlyEstimatesViewModel>();
        services.AddTransient<WhatIfViewModel>();
        services.AddTransient<MainWindowViewModel>();
    }
}
