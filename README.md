# PaycheckCalc

Desktop paycheck and annual tax calculator built with [Avalonia UI](https://avaloniaui.net/) and the .NET 11 preview SDK. The current codebase focuses on 2026 federal, state, and local withholding plus annual tax planning workflows.

---

## Features

- Per-paycheck withholding calculator for federal, state, local, and FICA taxes
- Self-employment tax calculator with QBI deduction support
- Annual tax workflow with:
  - Form 1040 calculation
  - annual projection
  - credits
  - jobs and year-to-date inputs
  - other income and adjustments
  - quarterly estimates
  - what-if analysis
- Saved paycheck history and saved annual scenarios
- Scenario comparison for current vs saved data and multi-scenario saved-paycheck comparisons
- CSV export for paycheck and self-employment results
- Dynamic per-state input fields driven by each state's calculator schema

---

## Tax Coverage

### Federal

- IRS Publication 15-T percentage-method withholding tables for 2026
- FICA withholding, including Social Security wage-base handling and Additional Medicare Tax
- Form 1040 annual income-tax calculation for 2026
- Form 1040-ES quarterly estimated-tax calculation
- Common annual-tax credit calculators, including education, saver's, child-tax, and NIIT-related flows

### State

All 50 states and the District of Columbia are supported through `StateCalculatorRegistry`.

- Dedicated calculator implementations are used for states with custom rules or data files
- No-income-tax states use `NoIncomeTaxWithholdingAdapter`
- Remaining supported states use percentage-method adapters backed by shared configuration

### Local

- Pennsylvania: Act 32 EIT and LST
- New York: New York City resident tax
- Ohio: RITA and CCA municipal withholding
- Maryland: county income-tax surtax

The repository also includes geocoding and jurisdiction-resolution service code, but the current desktop app is centered on manual calculator inputs.

---

## Project Structure

```text
PaycheckCalc.slnx
├── PaycheckCalc.Core/          # Calculation engine, models, tax logic, storage contracts
│   ├── Data/                   # 2026 tax tables and lookup JSON files
│   ├── Export/                 # CSV exporters
│   ├── Geocoding/              # Geocoding contracts and cache types
│   ├── Models/                 # Domain models and DTOs
│   ├── Pay/                    # Paycheck and projection calculators
│   ├── Storage/                # Repository interfaces
│   └── Tax/                    # Federal, state, local, annual, and self-employment tax logic
├── PaycheckCalc.Avalonia/      # Avalonia desktop UI
│   ├── Helpers/
│   ├── Mappers/
│   ├── Models/
│   ├── Services/
│   ├── Storage/
│   ├── ViewModels/
│   └── Views/
└── PaycheckCalc.Tests/         # xUnit test suite
```

---

## Prerequisites

- [.NET 11 SDK preview 3](https://dotnet.microsoft.com/download/dotnet/11.0) matching `global.json`

---

## Getting Started

```bash
git clone https://github.com/Fibonacci-0112/AvaloniaCalculator.git
cd AvaloniaCalculator

# build
dotnet build PaycheckCalc.slnx

# run the desktop app
dotnet run --project PaycheckCalc.Avalonia/PaycheckCalc.Avalonia.csproj

# run tests
dotnet test PaycheckCalc.slnx
```

---

## Data and Storage

- Tax tables are stored under `PaycheckCalc.Core/Data`
- Saved paychecks, saved annual scenarios, and CSV exports are written under the user's local application data folder in a `PaycheckCalc` directory

---

## Supported Pay Frequencies

`Weekly` · `Biweekly` · `Semimonthly` · `Monthly` · `Quarterly` · `Semiannual` · `Annual` · `Daily`

---

## Deductions and Export

- Pre-tax and post-tax deductions are supported as fixed amounts or percentages
- Paycheck and self-employment result exports are available as CSV
- PDF export is not currently available in the desktop app

---

## Tech Stack

| Layer | Technology |
|---|---|
| UI framework | [Avalonia UI](https://avaloniaui.net/) 12.0.1 |
| UI pattern | [CommunityToolkit.Mvvm](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/) 8.4.2 |
| Dependency injection | Microsoft.Extensions.DependencyInjection 11.0.0-preview.3.26207.106 |
| HTTP client | Microsoft.Extensions.Http 11.0.0-preview.3.26207.106 |
| Serialization | Newtonsoft.Json 13.0.5-beta1 |
| Test framework | xUnit 2.9.3 |
| Target framework | .NET 11 preview (`net11.0`) |
