# PaycheckCalc

A cross-platform paycheck and tax calculator built with [Avalonia UI](https://avaloniaui.net/) and .NET 9. It covers federal, state, and local withholding for all 50 US states and the District of Columbia, with support for self-employment income, annual tax projections, quarterly estimate planning, and more.

---

## Features

| Section | Description |
|---|---|
| **Calculator** | Per-paycheck federal + state + local withholding using 2026 tax tables |
| **Self-Employment** | Schedule C / SE tax estimation with QBI deduction (Form 8995/8995-A) |
| **Saved Paychecks** | Persist and review past paycheck calculations (JSON storage) |
| **Annual Tax** | Full Form 1040 walkthrough including Schedule 1 and common credits |
| **Annual Projection** | Projects full-year withholding, YTD totals, and over/under-withholding estimate |
| **Compare** | Side-by-side comparison of two paycheck scenarios |
| **Credits** | Child Tax Credit, Education Credits (Form 8863), Saver's Credit (Form 8880), NIIT (Form 8960) |
| **Jobs & YTD** | Multi-job W-2 inputs with year-to-date Social Security / Medicare coordination |
| **Other Income** | Adjustments for interest, dividends, capital gains, alimony, student loan interest, etc. |
| **Quarterly Estimates** | Form 1040-ES installment planner |
| **What-If** | Scenario modelling — change pay, deductions, or withholding and see the impact |

---

## Tax Coverage

### Federal
- **IRS Publication 15-T** — Percentage method (automated payroll tables, 2026)
- **FICA** — Social Security (6.2 %, $176,100 wage base) and Medicare (1.45 % + 0.9 % Additional Medicare Tax)
- **Form 1040** — 2026 income-tax brackets, standard deduction, Schedule 1 adjustments
- **Form 1040-ES** — quarterly estimated-tax safe harbour calculation

### State (all 50 states + DC)
Every state is registered in `StateCalculatorRegistry`. States with no income tax (e.g. TX, FL, WA) use a no-op adapter; states with complex formulas have dedicated calculators using 2026 withholding tables.

### Local
| Locality | Coverage |
|---|---|
| Pennsylvania | Act 32 EIT (earned-income tax) + LST (local services tax) |
| New York | New York City resident income tax |
| Ohio | RITA and CCA municipal withholding |
| Maryland | County income-tax surtax |

Address-based jurisdiction resolution is available via the Google Maps Geocoding API — the app can auto-detect local tax jurisdictions from a typed home/work address.

---

## Project Structure

```
PaycheckCalc.slnx
├── PaycheckCalc.Core/          # Domain logic — no UI dependencies
│   ├── Models/                 # Input/output DTOs (PaycheckInput, PaycheckResult, …)
│   ├── Pay/                    # PayCalculator, AnnualProjectionCalculator
│   ├── Tax/
│   │   ├── Federal/            # 15-T percentage calculator + annual Form 1040 calculators
│   │   ├── Fica/               # Social Security & Medicare
│   │   ├── State/              # IStateWithholdingCalculator + per-state implementations
│   │   └── Local/              # ILocalWithholdingCalculator + PA/NY/OH/MD implementations
│   ├── Geocoding/              # IGeocodingService, AddressService
│   ├── Export/                 # CsvPaycheckExporter, CsvSelfEmploymentExporter
│   └── Storage/                # IPaycheckRepository, IAnnualScenarioRepository
│
├── PaycheckCalc.Avalonia/      # Desktop UI (Avalonia 11, MVVM)
│   ├── Views/                  # .axaml views for each navigation section
│   ├── ViewModels/             # CommunityToolkit.Mvvm view-models
│   ├── Services/               # JurisdictionResolver, GoogleMapsGeocodingService
│   ├── Storage/                # JSON-backed repository implementations
│   └── Mappers/                # UI model ↔ domain model mappers
│
└── PaycheckCalc.Tests/         # xUnit test project
    └── (per-state + feature unit tests)
```

---

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9)

---

## Getting Started

```bash
# Clone
git clone https://github.com/Fibonacci-0112/AvaloniaCalculator.git
cd AvaloniaCalculator

# Run the desktop app
dotnet run --project PaycheckCalc.Avalonia

# Run tests
dotnet test PaycheckCalc.Tests
```

---

## Configuration

### Google Maps Geocoding (optional)

Address-to-jurisdiction resolution uses the [Google Maps Geocoding API](https://developers.google.com/maps/documentation/geocoding). Set your API key before running:

```bash
export GOOGLE_MAPS_API_KEY=your_key_here
```

If no key is supplied the geocoding feature is disabled; users can still select their state and local jurisdiction manually.

---

## Pay Frequencies

`Weekly` · `Biweekly` · `Semimonthly` · `Monthly` · `Quarterly` · `Semiannual` · `Annual` · `Daily`

---

## Deductions

Pre-tax and post-tax deductions are supported as either a fixed dollar amount or a percentage of gross pay.

---

## Export

Calculated results can be exported to **CSV** for both W-2 paychecks and self-employment summaries.

---

## Tech Stack

| Layer | Technology |
|---|---|
| UI framework | [Avalonia UI](https://avaloniaui.net/) 11.2.7 |
| UI pattern | MVVM via [CommunityToolkit.Mvvm](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/) 8.4.0 |
| DI container | Microsoft.Extensions.DependencyInjection 9.0 |
| Serialisation | Newtonsoft.Json 13.0.3 |
| HTTP client | Microsoft.Extensions.Http 9.0 |
| Test framework | xUnit 2.9.2 |
| Target runtime | .NET 9 |