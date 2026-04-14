# Data Analysis Note

This note explains how to interpret the JSON files exported by `PerceptionTests` for downstream statistical analysis.

Use it together with [`OUTPUT_SCHEMA.md`](OUTPUT_SCHEMA.md) and the example export in [`examples/sample-result.json`](examples/sample-result.json).

## What One Export File Represents

Each final `testResult_XXXX.json` file represents one completed participant run.

A single export file combines:

- participant questionnaire data,
- run metadata,
- the questionnaire version used for that run,
- the experiment-configuration snapshot used during collection,
- session-level response histories for all three experiment groups.

## Primary Outcome Variables

For each attempt, the software records:

- `rawResponseTimeMilliseconds`
- `mappedToneDurationMilliseconds`
- `responseCapturedWithinStimulus`

These values should be interpreted as follows:

- `rawResponseTimeMilliseconds` is the elapsed time from playback start to the participant stop action.
- `mappedToneDurationMilliseconds` is the tone duration associated with the response point under the experiment's stimulus schedule.
- `responseCapturedWithinStimulus` indicates whether the participant response was captured while a valid mapped stimulus value was available.

For most analyses based on perceptual threshold-like behavior in this software, `mappedToneDurationMilliseconds` is the main design-specific dependent variable.

## Session-Level Interpretation

Each session contains:

- `attempts`
- `validResponseCount`
- `nonResponseCount`
- `averageMappedToneDurationMilliseconds`

Recommended interpretation:

- `attempts` is the full response history for that session.
- `validResponseCount` counts attempts with a non-null `mappedToneDurationMilliseconds`.
- `nonResponseCount` counts attempts without a valid mapped stimulus value.
- `averageMappedToneDurationMilliseconds` is the arithmetic mean across valid attempts only.

In the current released protocol, a completed session is expected to contribute three valid responses. Non-responses remain useful for descriptive summaries and quality control.

## Questionnaire Variables

Questionnaire data are exported under stable field identifiers rather than question text. The exported questionnaire block should therefore be treated as a structured set of participant descriptors.

Examples include:

- `isMusician`
- `gender`
- `age`
- `handedness`
- `hasAbsolutePitch`
- `instrumentPracticeYears`
- `hasAmateurMusicPerformanceExperience`
- `preferredListeningMusic`

Fields that do not apply to the selected questionnaire branch are expected to be `null`.

## Configuration Snapshot As Part Of The Scientific Record

The `metadata.experimentConfiguration` block should be retained as part of the scientific record for each exported run.

It captures the released session definitions active during data collection, including:

- session identifiers,
- stimulus frequencies,
- tone-duration range,
- nominal session duration,
- duration-mapping mode,
- low-frequency compensation settings,
- required valid responses.

The `metadata.questionnaireVersion` field should also be retained, especially when combining data collected across different wording revisions.

## Recommended Aggregation Strategy

A typical analysis workflow is:

1. Analyze one final JSON file per participant.
2. Derive either a session-level dataset or an attempt-level dataset.
3. Use `averageMappedToneDurationMilliseconds` as the session summary value when a single threshold-like measure is needed.
4. Retain `nonResponseCount` as a quality or compliance indicator.
5. Verify that questionnaire version and experiment configuration are comparable before pooling runs across data-collection periods.

## Session-Level Versus Attempt-Level Datasets

Two common analysis layouts are appropriate:

### Session-Level Dataset

Use one row per session when the analysis focuses on summary performance.

Recommended retained fields:

- participant identifier from the export file context,
- `sessionId`,
- experiment group and session label,
- `isMusician`,
- relevant questionnaire descriptors,
- `validResponseCount`,
- `nonResponseCount`,
- `averageMappedToneDurationMilliseconds`.

### Attempt-Level Dataset

Use one row per attempt when the analysis focuses on within-session dynamics or response history.

Recommended retained fields:

- participant identifier from the export file context,
- experiment group and session label,
- `attemptNumber`,
- `rawResponseTimeMilliseconds`,
- `mappedToneDurationMilliseconds`,
- `responseCapturedWithinStimulus`.

## Important Caveats

- `mappedToneDurationMilliseconds` is the primary design-specific outcome measure, but `rawResponseTimeMilliseconds` remains useful for audit and exploratory analysis.
- Checkpoint files are intermediate artifacts and should not be mixed with final `testResult_XXXX.json` files in completed analyses.
- Audio playback depends on the Windows audio environment used during collection. Hardware and operating-system latency therefore remain methodological constraints outside the JSON export itself.

## Practical Recommendation

Before starting formal statistical analysis, convert the exported JSON files into a flat analysis table and preserve a copy of the original JSON exports. This keeps the analysis workflow reproducible while maintaining a full record of the original software output.
