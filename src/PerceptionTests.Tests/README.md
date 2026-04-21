# Test Project

This project contains automated tests for the [`PerceptionTests`](../PerceptionTests/) WPF application.

## Coverage Areas

- experiment configuration loading and validation,
- supported duration-mapping formulas,
- deterministic sample generation,
- response-time to tone-duration mapping,
- questionnaire configuration validation,
- dynamic questionnaire view models,
- questionnaire response mapping,
- checkpoint and final result persistence,
- JSON export schema and metadata.

## Running Tests

From the repository root:

```powershell
dotnet test .\src\PerceptionTests.Tests\PerceptionTests.Tests.csproj -c Release
```

or run the full verification script:

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\verify.ps1
```

## Parallelization

The test assembly disables parallel execution in [`AssemblyInfo.cs`](AssemblyInfo.cs). This is intentional because some tests replace global runtime catalog state while checking alternate experiment and questionnaire configurations.

## Test Helpers

[`ExperimentCatalogTestHelper`](ExperimentCatalogTestHelper.cs) creates temporary experiment and questionnaire configuration files, initializes runtime catalogs, and restores the default test configuration after tests that intentionally switch mappings or protocol definitions.
