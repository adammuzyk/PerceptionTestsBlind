# Source Layout

This folder contains the application source code and automated tests for `PerceptionTests`.

## Projects

- [`PerceptionTests/`](PerceptionTests/) - WPF desktop application targeting `net8.0-windows`.
- [`PerceptionTests.Tests/`](PerceptionTests.Tests/) - xUnit test project covering experiment configuration, questionnaire behavior, sample generation, response mapping, persistence, and JSON export.
- [`PerceptionTests.sln`](PerceptionTests.sln) - solution file used by local verification and CI.

## Common Commands

From the repository root:

```powershell
dotnet build .\src\PerceptionTests.sln -c Release
dotnet test .\src\PerceptionTests.Tests\PerceptionTests.Tests.csproj -c Release
powershell -ExecutionPolicy Bypass -File .\scripts\verify.ps1
```

## Application Notes

The WPF application loads researcher-editable runtime configuration files from the application output directory:

- [`experiment-config.json`](PerceptionTests/experiment-config.json)
- [`questionnaire-config.json`](PerceptionTests/questionnaire-config.json)

These files are copied from [`src/PerceptionTests/`](PerceptionTests/) during build. The application validates them at startup before running the participant workflow.

## Test Notes

Some tests initialize static runtime catalogs such as [`ExperimentCatalog`](PerceptionTests/Music/ExperimentCatalog.cs) and [`QuestionnaireCatalog`](PerceptionTests/Services/QuestionnaireCatalog.cs). The test assembly disables parallel execution in [`PerceptionTests.Tests/AssemblyInfo.cs`](PerceptionTests.Tests/AssemblyInfo.cs) so configuration-dependent tests do not interfere with each other.
