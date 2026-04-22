namespace PaycheckCalc.Core.Models;

/// <summary>
/// Domain model for annual paycheck projections.
/// Contains annualized amounts, projected year-to-date totals,
/// remaining paycheck count, and estimated over/under withholding.
/// </summary>
public sealed class AnnualProjection
{
    // ── Pay period info ─────────────────────────────────────
    public int PayPeriodsPerYear { get; init; }
    public int CurrentPaycheckNumber { get; init; }
    public int RemainingPaychecks { get; init; }

    // ── Annualized amounts (per-period × periods/year) ──────
    public decimal AnnualizedGrossPay { get; init; }
    public decimal AnnualizedPreTaxDeductions { get; init; }
    public decimal AnnualizedPostTaxDeductions { get; init; }
    public decimal AnnualizedFederalTaxableWages { get; init; }
    public decimal AnnualizedFicaTaxableWages { get; init; }
    public decimal AnnualizedStateTaxableWages { get; init; }
    public decimal AnnualizedFederalWithholding { get; init; }
    public decimal AnnualizedStateWithholding { get; init; }
    public decimal AnnualizedFica { get; init; }
    /// <summary>
    /// Annualized Social Security withholding (the 6.2% OASDI portion of FICA,
    /// capped at the Social Security wage base). Broken out from
    /// <see cref="AnnualizedFica"/> so the UI can display Social Security and
    /// Medicare separately.
    /// </summary>
    public decimal AnnualizedSocialSecurity { get; init; }
    /// <summary>
    /// Annualized Medicare withholding (the 1.45% regular Medicare rate plus
    /// any 0.9% Additional Medicare withholding). Broken out from
    /// <see cref="AnnualizedFica"/>.
    /// </summary>
    public decimal AnnualizedMedicare { get; init; }
    public decimal AnnualizedNetPay { get; init; }

    // ── Projected YTD (per-period × current paycheck number) ─
    public decimal ProjectedYtdGrossPay { get; init; }
    public decimal ProjectedYtdFederalWithholding { get; init; }
    public decimal ProjectedYtdStateWithholding { get; init; }
    public decimal ProjectedYtdFica { get; init; }
    public decimal ProjectedYtdNetPay { get; init; }

    // ── Under/over withholding estimate ─────────────────────
    public decimal EstimatedAnnualFederalLiability { get; init; }
    public decimal EstimatedAnnualFicaLiability { get; init; }
    public decimal AnnualizedTotalWithholding { get; init; }
    public decimal EstimatedTotalLiability { get; init; }

    /// <summary>
    /// Positive = over-withholding (likely refund),
    /// negative = under-withholding (likely owe).
    /// </summary>
    public decimal OverUnderWithholding { get; init; }
}
