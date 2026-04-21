# PerceptionTests

`PerceptionTests` is a Windows WPF application for auditory perception experiments. The software presents a participant questionnaire, runs generated sound-based listening sessions, records the participant's response point, and saves structured results as JSON.

## Overview

The application supports a complete experimental run consisting of:

1. a welcome screen,
2. a participant questionnaire,
3. nine listening sessions grouped into three experiments,
4. automatic checkpoint saving after each completed session,
5. final export of the full participant result as JSON.

The software generates auditory stimuli programmatically at runtime rather than loading bundled test audio assets.

## Main Features

- Windows desktop interface implemented in WPF
- researcher-editable experiment configuration loaded from JSON
- researcher-editable questionnaire configuration loaded from JSON
- dynamic questionnaire rendering from a constrained configuration schema
- generated wave-based listening stimuli
- keyboard response capture during playback
- structured JSON export for downstream analysis
- checkpoint persistence after each completed listening session

## Repository Layout

```text
.
|-- docs/
|   |-- examples/
|   |-- CHECKPOINTS.md
|   |-- DATA_ANALYSIS.md
|   |-- DEPENDENCIES.md
|   |-- OUTPUT_SCHEMA.md
|   `-- RESEARCHER_CONFIG_GUIDE.md
|-- src/
|   |-- README.md
|   |-- PerceptionTests.sln
|   |-- PerceptionTests/
|   `-- PerceptionTests.Tests/
|-- CITATION.cff
|-- LICENSE
`-- global.json
```

See [`src/README.md`](src/README.md) for a short source-layout guide.

## Requirements

- Windows 10 or Windows 11
- .NET SDK compatible with building `net8.0-windows`
- desktop audio output support

The repository currently pins SDK `9.0.102` in [`global.json`](global.json). The application target framework is `net8.0-windows`.

## Build

### Command line

```powershell
dotnet restore .\src\PerceptionTests.sln
dotnet build .\src\PerceptionTests.sln -c Release --no-restore
dotnet test .\src\PerceptionTests.sln -c Release --no-build
```

The same restore/build/test sequence is wrapped by [`scripts/verify.ps1`](scripts/verify.ps1).

### Visual Studio

1. Open [`src/PerceptionTests.sln`](src/PerceptionTests.sln) in Visual Studio 2022 or newer.
2. Restore NuGet packages if prompted.
3. Build and run the [`PerceptionTests`](src/PerceptionTests/) project.

## Runtime Configuration

Runtime settings are defined in [`src/PerceptionTests/App.config`](src/PerceptionTests/App.config).

The main configurable paths are:

- `ResultPath` - folder where participant result files are written
- `WaveFilePath` - folder where generated `.wav` files are written when file saving is enabled
- `ExperimentConfigurationPath` - path to the experiment definition file
- `QuestionnaireConfigurationPath` - path to the questionnaire definition file

By default, the experiment protocol is loaded from [`src/PerceptionTests/experiment-config.json`](src/PerceptionTests/experiment-config.json), and the questionnaire definition is loaded from [`src/PerceptionTests/questionnaire-config.json`](src/PerceptionTests/questionnaire-config.json). These files are copied next to the executable during build so they can be edited without recompiling the application.

## Output

At the end of a completed run, the application writes numbered JSON files such as:

- `testResult_0001.json`
- `testResult_0002.json`

The exported result contains:

- participant questionnaire data,
- participant category information,
- experiment and session grouping,
- recorded response timing values,
- missed attempts,
- export metadata.

A checkpoint JSON file is also written after each completed listening session.

The output schema and an example result file are documented in:

- [`docs/OUTPUT_SCHEMA.md`](docs/OUTPUT_SCHEMA.md)
- [`docs/examples/sample-result.json`](docs/examples/sample-result.json)

## Documentation

- [`docs/RESEARCHER_CONFIG_GUIDE.md`](docs/RESEARCHER_CONFIG_GUIDE.md) - how to edit experiment and questionnaire configuration safely
- [`docs/OUTPUT_SCHEMA.md`](docs/OUTPUT_SCHEMA.md) - exported JSON structure
- [`docs/CHECKPOINTS.md`](docs/CHECKPOINTS.md) - checkpoint and final-save behavior
- [`docs/DATA_ANALYSIS.md`](docs/DATA_ANALYSIS.md) - notes for downstream analysis
- [`docs/DEPENDENCIES.md`](docs/DEPENDENCIES.md) - dependency and runtime notes

## Citation And License

- License: [`MIT`](LICENSE)
- Citation metadata: [`CITATION.cff`](CITATION.cff)

## Author

- Software author: [TO BE PROVIDED]
- Preferred citation metadata: [`CITATION.cff`](CITATION.cff)
