# Output Schema

`PerceptionTests` writes participant results as UTF-8 encoded `.json` files named `testResult_0001.json`, `testResult_0002.json`, and so on.

An example export is available in [`examples/sample-result.json`](examples/sample-result.json).

## Top-Level Structure

Each exported result file contains the following top-level keys:

```text
metadata
questionnaire
Experiment1
Experiment2
Experiment3
```

## `metadata`

The `metadata` object records run-level context for the export.

Current fields:

- `applicationVersion`: semantic version string taken from the application assembly.
- `exportedAtUtc`: UTC timestamp of export in ISO 8601 format.
- `workstationName`: machine name of the workstation that saved the file.
- `sessionId`: generated identifier for the participant run.
- `questionnaireVersion`: version string of the questionnaire branch used for that run.
- `experimentConfiguration`: snapshot of the experiment definitions active during that run.

## `questionnaire`

The `questionnaire` object stores participant descriptors under stable field identifiers rather than question text.

Current exported fields:

- `isMusician`
- `gender`
- `age`
- `handedness`
- `musicalEducationDescription`
- `instrumentLearningStartAge`
- `instrumentPracticeYears`
- `hasAbsolutePitch`
- `primaryPerformanceGenre`
- `hasAmateurMusicPerformanceExperience`
- `amateurMusicActivityDetails`
- `preferredListeningMusic`
- `studyYearAndSpecialization`

Fields that do not apply to the selected questionnaire branch are exported as `null`.

## Experiment Groups

Each of `Experiment1`, `Experiment2`, and `Experiment3` contains three named session slots:

```text
Session1
Session2
Session3
```

Each session value is either:

- `null`, when no session result is available in that slot,
- or a session-result object.

## Session-Result Object

Each populated session-result object contains:

- `attempts`: ordered array of response attempts.
- `validResponseCount`: number of attempts with a non-null mapped tone duration.
- `nonResponseCount`: number of attempts without a mapped tone duration.
- `averageMappedToneDurationMilliseconds`: arithmetic mean across valid mapped tone durations, or `null` if none were captured.

## Attempt Object

Each item in the `attempts` array contains:

- `attemptNumber`: 1-based sequence number within the session.
- `rawResponseTimeMilliseconds`: measured elapsed time from playback start to the participant stop action.
- `mappedToneDurationMilliseconds`: mapped tone duration at the response point, or `null` when no valid mapped value was captured during the stimulus.
- `responseCapturedWithinStimulus`: Boolean convenience field indicating whether a valid mapped response was captured.

## `experimentConfiguration`

The configuration snapshot is embedded in `metadata.experimentConfiguration` and currently contains three arrays:

- `experiment1`
- `experiment2`
- `experiment3`

Each session definition contains:

- `sessionId`
- `startToneDurationMilliseconds`
- `endToneDurationMilliseconds`
- `nominalSampleDurationMilliseconds`
- `durationMapping`
- `frequenciesHz`
- `lowFrequencyGainBelowHz`
- `lowFrequencyGainMultiplier`
- `attackReleaseMilliseconds`
- `requiredValidResponses`

Supported `durationMapping` values are currently:

- `linear`
- `logarithmic`
- `hyperbolic`
- `sqrt`
- `root3`
- `arctan`

The distributed experiment configuration uses `hyperbolic` for all sessions unless intentionally changed by the researcher.

## Notes On Incomplete Runs

Files produced from interrupted runs may contain `null` session slots or fewer completed attempts than expected by the nominal study protocol. Final participant result files should therefore be interpreted together with the session-level counts rather than assuming that every slot is fully populated.
