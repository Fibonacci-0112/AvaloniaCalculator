using CommunityToolkit.Mvvm.ComponentModel;

namespace PaycheckCalc.Avalonia.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    public CalculatorViewModel Calculator { get; }
    public SelfEmploymentViewModel SelfEmployment { get; }
    public SavedPaychecksViewModel SavedPaychecks { get; }
    public AnnualTaxViewModel AnnualTax { get; }
    public AnnualProjectionViewModel AnnualProjection { get; }
    public CompareViewModel Compare { get; }
    public CreditsViewModel Credits { get; }
    public JobsAndYtdViewModel JobsAndYtd { get; }
    public OtherIncomeAdjustmentsViewModel OtherIncomeAdjustments { get; }
    public QuarterlyEstimatesViewModel QuarterlyEstimates { get; }
    public WhatIfViewModel WhatIf { get; }

    public MainWindowViewModel(
        CalculatorViewModel calculator,
        SelfEmploymentViewModel selfEmployment,
        SavedPaychecksViewModel savedPaychecks,
        AnnualTaxViewModel annualTax,
        AnnualProjectionViewModel annualProjection,
        CompareViewModel compare,
        CreditsViewModel credits,
        JobsAndYtdViewModel jobsAndYtd,
        OtherIncomeAdjustmentsViewModel otherIncomeAdjustments,
        QuarterlyEstimatesViewModel quarterlyEstimates,
        WhatIfViewModel whatIf)
    {
        Calculator = calculator;
        SelfEmployment = selfEmployment;
        SavedPaychecks = savedPaychecks;
        AnnualTax = annualTax;
        AnnualProjection = annualProjection;
        Compare = compare;
        Credits = credits;
        JobsAndYtd = jobsAndYtd;
        OtherIncomeAdjustments = otherIncomeAdjustments;
        QuarterlyEstimates = quarterlyEstimates;
        WhatIf = whatIf;
    }
}
