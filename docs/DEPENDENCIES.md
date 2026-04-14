# Build And Runtime Requirements

This document summarizes the software environment required to build and run `PerceptionTests`.

## Supported Operating Environment

`PerceptionTests` is a Windows desktop application built with WPF.

Recommended operating environment:

- Windows 10 or Windows 11
- desktop audio support enabled
- speakers or headphones for participant playback
- keyboard input for participant responses during listening sessions

## Build Requirements

The repository is currently configured to build with:

- .NET SDK `9.0.102`, as pinned in `global.json`
- compatible `.NET 9.0.x` SDK installations in equivalent build environments

Application target framework:

- `net8.0-windows`

No legacy .NET Framework developer pack is required.

## Core Application Packages

The main application currently depends on the following NuGet packages:

- `Extended.Wpf.Toolkit` `5.0.0`
- `Newtonsoft.Json` `13.0.3`
- `System.Configuration.ConfigurationManager` `8.0.0`
- `System.Windows.Extensions` `8.0.0`

## Test-Project Packages

If the bundled test project is built and run, the following additional packages are used:

- `Microsoft.NET.Test.Sdk` `17.11.1`
- `xunit` `2.9.2`
- `xunit.runner.visualstudio` `2.8.2`

## Build Entry Point

Primary solution file:

- [`src/PerceptionTests.sln`](../src/PerceptionTests.sln)

Main application project:

- [`src/PerceptionTests/PerceptionTests.csproj`](../src/PerceptionTests/PerceptionTests.csproj)

## Configuration Files Used At Runtime

The following files are part of the released application workflow:

- [`src/PerceptionTests/App.config`](../src/PerceptionTests/App.config)
- [`src/PerceptionTests/experiment-config.json`](../src/PerceptionTests/experiment-config.json)
- [`src/PerceptionTests/questionnaire-config.json`](../src/PerceptionTests/questionnaire-config.json)

These files define runtime paths and the researcher-editable experiment and questionnaire configurations used by the application.

## Minimal Manual Build Commands

```powershell
dotnet restore .\src\PerceptionTests.sln
dotnet build .\src\PerceptionTests.sln -c Release --no-restore
```

If the test project is included in the public release and you want to run tests:

```powershell
dotnet test .\src\PerceptionTests.sln -c Release
```
