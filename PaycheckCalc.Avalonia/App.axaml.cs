using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using PaycheckCalc.Avalonia.Services;
using PaycheckCalc.Avalonia.Storage;
using PaycheckCalc.Avalonia.ViewModels;
using PaycheckCalc.Core.Geocoding;
using PaycheckCalc.Core.Pay;
using PaycheckCalc.Core.Storage;
using PaycheckCalc.Core.Tax.Alabama;
using PaycheckCalc.Core.Tax.Arizona;
using PaycheckCalc.Core.Tax.Arkansas;
using PaycheckCalc.Core.Tax.California;
using PaycheckCalc.Core.Tax.Colorado;
using PaycheckCalc.Core.Tax.Connecticut;
using PaycheckCalc.Core.Tax.Delaware;
using PaycheckCalc.Core.Tax.DistrictOfColumbia;
using PaycheckCalc.Core.Tax.Federal;
using PaycheckCalc.Core.Tax.Federal.Annual;
using PaycheckCalc.Core.Tax.Fica;
using PaycheckCalc.Core.Tax.Georgia;
using PaycheckCalc.Core.Tax.Hawaii;
using PaycheckCalc.Core.Tax.Idaho;
using PaycheckCalc.Core.Tax.Illinois;
using PaycheckCalc.Core.Tax.Indiana;
using PaycheckCalc.Core.Tax.Iowa;
using PaycheckCalc.Core.Tax.Kansas;
using PaycheckCalc.Core.Tax.Kentucky;
using PaycheckCalc.Core.Tax.Local;
using PaycheckCalc.Core.Tax.Local.Maryland;
using PaycheckCalc.Core.Tax.Local.NewYork;
using PaycheckCalc.Core.Tax.Local.Ohio;
using PaycheckCalc.Core.Tax.Local.Pennsylvania;
using PaycheckCalc.Core.Tax.Louisiana;
using PaycheckCalc.Core.Tax.Maine;
using PaycheckCalc.Core.Tax.Maryland;
using PaycheckCalc.Core.Tax.Massachusetts;
using PaycheckCalc.Core.Tax.Michigan;
using PaycheckCalc.Core.Tax.Minnesota;
using PaycheckCalc.Core.Tax.Mississippi;
using PaycheckCalc.Core.Tax.Missouri;
using PaycheckCalc.Core.Tax.Montana;
using PaycheckCalc.Core.Tax.Nebraska;
using PaycheckCalc.Core.Tax.NewJersey;
using PaycheckCalc.Core.Tax.NewMexico;
using PaycheckCalc.Core.Tax.NewYork;
using PaycheckCalc.Core.Tax.NorthCarolina;
using PaycheckCalc.Core.Tax.NorthDakota;
using PaycheckCalc.Core.Tax.Ohio;
using PaycheckCalc.Core.Tax.Oklahoma;
using PaycheckCalc.Core.Tax.Oregon;
using PaycheckCalc.Core.Tax.Pennsylvania;
using PaycheckCalc.Core.Tax.RhodeIsland;
using PaycheckCalc.Core.Tax.SelfEmployment;
using PaycheckCalc.Core.Tax.SouthCarolina;
using PaycheckCalc.Core.Tax.State;
using PaycheckCalc.Core.Tax.State.Annual;
using PaycheckCalc.Core.Tax.Utah;
using PaycheckCalc.Core.Tax.Vermont;
using PaycheckCalc.Core.Tax.Virginia;
using PaycheckCalc.Core.Tax.Washington;
using PaycheckCalc.Core.Tax.WestVirginia;
using PaycheckCalc.Core.Tax.Wisconsin;
using PaycheckCalc.Core.Tax.Wyoming;
using PaycheckCalc.Core.Models;
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

    /// <summary>
    /// Read a JSON data file that was copied to the output directory alongside the executable.
    /// </summary>
    private static string ReadDataFile(string fileName)
    {
        var exeDir = AppContext.BaseDirectory;
        var path = Path.Combine(exeDir, fileName);
        return File.ReadAllText(path);
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        var dataDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "PaycheckCalc");
        Directory.CreateDirectory(dataDir);

        // ── Repositories ─────────────────────────────────────────
        services.AddSingleton<IPaycheckRepository>(_ => new JsonPaycheckRepository(dataDir));
        services.AddSingleton<IAnnualScenarioRepository>(_ => new JsonAnnualScenarioRepository(dataDir));

        // ── FICA / Federal ────────────────────────────────────────
        services.AddSingleton(new FicaCalculator());

        services.AddSingleton(_ =>
        {
            var json = ReadDataFile("us_irs_15t_2026_percentage_automated.json");
            return new Irs15TPercentageCalculator(json);
        });

        services.AddSingleton(_ =>
        {
            var json = ReadDataFile("federal_1040_brackets_2026.json");
            return new Federal1040TaxCalculator(json);
        });

        services.AddSingleton<Schedule1Calculator>();
        services.AddSingleton<WithholdingSuggestionCalculator>();

        // ── Data-driven state calculators ─────────────────────────
        services.AddSingleton(_ =>
        {
            var json = ReadDataFile("ar_withholding_2026.json");
            return new ArkansasFormulaCalculator(json);
        });

        services.AddSingleton(_ =>
        {
            var json = ReadDataFile("ok_ow2_2026_percentage.json");
            return new OklahomaOw2PercentageCalculator(json);
        });

        services.AddSingleton(_ =>
        {
            var json = ReadDataFile("ca_method_b_2026.json");
            return new CaliforniaPercentageCalculator(json);
        });

        services.AddSingleton(sp =>
        {
            var json = ReadDataFile("co_dr0004_2026.json");
            return new ColoradoWithholdingCalculator(json);
        });

        services.AddSingleton(sp =>
        {
            var json = ReadDataFile("connecticut_withholding_2026.json");
            return new ConnecticutWithholdingCalculator(json);
        });

        // ── State calculator registry ─────────────────────────────
        services.AddSingleton(sp =>
        {
            var registry = new StateCalculatorRegistry();

            registry.Register(new AlabamaWithholdingCalculator());
            registry.Register(new ArizonaWithholdingCalculator());

            var arCalc = sp.GetRequiredService<ArkansasFormulaCalculator>();
            registry.Register(new ArkansasWithholdingCalculator(arCalc));

            var caCalc = sp.GetRequiredService<CaliforniaPercentageCalculator>();
            registry.Register(new CaliforniaWithholdingCalculator(caCalc));

            var coCalc = sp.GetRequiredService<ColoradoWithholdingCalculator>();
            registry.Register(coCalc);

            var ctCalc = sp.GetRequiredService<ConnecticutWithholdingCalculator>();
            registry.Register(ctCalc);

            registry.Register(new DelawareWithholdingCalculator());
            registry.Register(new DistrictOfColumbiaWithholdingCalculator());
            registry.Register(new GeorgiaWithholdingCalculator());
            registry.Register(new HawaiiWithholdingCalculator());
            registry.Register(new IdahoWithholdingCalculator());
            registry.Register(new IllinoisWithholdingCalculator());
            registry.Register(new IndianaWithholdingCalculator());
            registry.Register(new IowaWithholdingCalculator());
            registry.Register(new KansasWithholdingCalculator());
            registry.Register(new KentuckyWithholdingCalculator());
            registry.Register(new LouisianaWithholdingCalculator());
            registry.Register(new MaineWithholdingCalculator());
            registry.Register(new MarylandWithholdingCalculator());
            registry.Register(new MassachusettsWithholdingCalculator());
            registry.Register(new MichiganWithholdingCalculator());
            registry.Register(new MinnesotaWithholdingCalculator());
            registry.Register(new MississippiWithholdingCalculator());
            registry.Register(new MissouriWithholdingCalculator());
            registry.Register(new MontanaWithholdingCalculator());
            registry.Register(new NebraskaWithholdingCalculator());
            registry.Register(new NewJerseyWithholdingCalculator());
            registry.Register(new NewMexicoWithholdingCalculator());
            registry.Register(new NewYorkWithholdingCalculator());
            registry.Register(new NorthCarolinaWithholdingCalculator());
            registry.Register(new NorthDakotaWithholdingCalculator());
            registry.Register(new OhioWithholdingCalculator());
            registry.Register(new OregonWithholdingCalculator());
            registry.Register(new RhodeIslandWithholdingCalculator());
            registry.Register(new SouthCarolinaWithholdingCalculator());
            registry.Register(new UtahWithholdingCalculator());
            registry.Register(new VermontWithholdingCalculator());
            registry.Register(new VirginiaWithholdingCalculator());
            registry.Register(new WashingtonWithholdingCalculator());
            registry.Register(new WestVirginiaWithholdingCalculator());
            registry.Register(new WisconsinWithholdingCalculator());
            registry.Register(new WyomingWithholdingCalculator());

            var okCalc = sp.GetRequiredService<OklahomaOw2PercentageCalculator>();
            registry.Register(new OklahomaWithholdingCalculator(okCalc));

            registry.Register(new PennsylvaniaWithholdingCalculator());

            // States with no individual income tax
            UsState[] noTaxStates = [UsState.AK, UsState.FL, UsState.NV, UsState.NH, UsState.SD, UsState.TN, UsState.TX];
            foreach (var state in noTaxStates)
                registry.Register(new NoIncomeTaxWithholdingAdapter(state));

            // Remaining states via percentage method
            foreach (var (state, config) in StateTaxConfigs2026.Configs)
                registry.Register(new PercentageMethodWithholdingAdapter(state, config));

            return registry;
        });

        // ── Local calculator registry ─────────────────────────────
        services.AddSingleton(_ =>
        {
            var registry = new LocalCalculatorRegistry();

            var paEitJson = ReadDataFile("pa_eit_2026.json");
            registry.Register(new PaEitCalculator(new PaEitRateTable(paEitJson)));
            registry.Register(new PaLstCalculator());

            var nycJson = ReadDataFile("nyc_withholding_2026.json");
            registry.Register(new NycWithholdingCalculator(nycJson));

            var ohRitaJson = ReadDataFile("oh_rita_2026.json");
            registry.Register(new OhRitaCalculator(ohRitaJson));

            var ohCcaJson = ReadDataFile("oh_cca_2026.json");
            registry.Register(new OhCcaCalculator(ohCcaJson));

            var mdJson = ReadDataFile("md_county_surtax_2026.json");
            registry.Register(new MdCountyCalculator(mdJson));

            return registry;
        });

        // ── Pay / Projection calculators ─────────────────────────
        services.AddSingleton(sp =>
            new PayCalculator(
                sp.GetRequiredService<StateCalculatorRegistry>(),
                sp.GetRequiredService<FicaCalculator>(),
                sp.GetRequiredService<Irs15TPercentageCalculator>(),
                sp.GetRequiredService<LocalCalculatorRegistry>()));

        services.AddSingleton(sp =>
            new AnnualProjectionCalculator(
                sp.GetRequiredService<Irs15TPercentageCalculator>(),
                sp.GetRequiredService<FicaCalculator>()));

        // ── Self-employment calculators ────────────────────────────
        services.AddSingleton(sp =>
            new SelfEmploymentTaxCalculator(sp.GetRequiredService<FicaCalculator>()));

        services.AddSingleton<QbiDeductionCalculator>();

        services.AddSingleton(sp =>
            new SelfEmploymentCalculator(
                sp.GetRequiredService<SelfEmploymentTaxCalculator>(),
                sp.GetRequiredService<QbiDeductionCalculator>(),
                sp.GetRequiredService<Irs15TPercentageCalculator>(),
                sp.GetRequiredService<StateCalculatorRegistry>()));

        // ── Annual Form 1040 calculators ──────────────────────────
        services.AddSingleton(sp =>
            new AnnualStateTaxCalculator(sp.GetRequiredService<StateCalculatorRegistry>()));

        services.AddSingleton(sp =>
            new Form1040Calculator(
                sp.GetRequiredService<Federal1040TaxCalculator>(),
                sp.GetRequiredService<Schedule1Calculator>(),
                sp.GetRequiredService<SelfEmploymentTaxCalculator>(),
                sp.GetRequiredService<QbiDeductionCalculator>(),
                sp.GetRequiredService<FicaCalculator>(),
                stateTax: sp.GetRequiredService<AnnualStateTaxCalculator>()));

        services.AddSingleton<Form1040ESCalculator>();

        // ── Geocoding / address services ──────────────────────────
        services.AddSingleton<IGeocodingCache, InMemoryGeocodingCache>();
        services.AddHttpClient<GoogleMapsGeocodingService>();
        services.AddSingleton<JurisdictionResolver>();

        // ── Session services ──────────────────────────────────────
        services.AddSingleton<AnnualTaxSession>();
        services.AddSingleton<ComparisonSession>();

        // ── ViewModels ────────────────────────────────────────────
        services.AddSingleton<CalculatorViewModel>();
        services.AddSingleton<SavedPaychecksViewModel>();
        services.AddSingleton<CompareViewModel>();
        services.AddSingleton<SelfEmploymentViewModel>();
        services.AddSingleton<AnnualTaxViewModel>();
        services.AddSingleton<AnnualProjectionViewModel>();
        services.AddSingleton<JobsAndYtdViewModel>();
        services.AddSingleton<OtherIncomeAdjustmentsViewModel>();
        services.AddSingleton<CreditsViewModel>();
        services.AddSingleton<QuarterlyEstimatesViewModel>();
        services.AddSingleton<WhatIfViewModel>();
        services.AddSingleton<MainWindowViewModel>();
    }
}
