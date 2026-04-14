# Researcher Configuration Guide

This guide explains how to edit the released configuration files safely:

- [`experiment-config.json`](../src/PerceptionTests/experiment-config.json)
- [`questionnaire-config.json`](../src/PerceptionTests/questionnaire-config.json)

These files are intended for controlled researcher-facing adjustments to the released application. Treat any substantive change as part of the study protocol and document it accordingly.

## Recommended Workflow

1. Edit the configuration file as valid UTF-8 JSON.
2. Save the file and rebuild or relaunch the application as required by your deployment workflow.
3. Confirm that the application starts without configuration errors.
4. Run a short validation session before collecting participant data.
5. Keep a dated copy of the configuration used for data collection.

## `experiment-config.json`

This file defines the listening-session protocol used by the application.

### Typical Safe Edits

The following changes may be appropriate when intentionally revising the released protocol:

- updating `frequenciesHz`,
- adjusting `startToneDurationMilliseconds` and `endToneDurationMilliseconds`,
- adjusting `nominalSampleDurationMilliseconds`,
- revising `lowFrequencyGainBelowHz` and `lowFrequencyGainMultiplier` when low-frequency compensation is part of the protocol,
- changing `requiredValidResponses` when the study design requires a different number of valid responses.

### Fields To Treat With Extra Caution

The following values are part of the released structure and should not be changed casually:

- `sessionId`
- `durationMapping`
- the overall session set across `experiment1`, `experiment2`, and `experiment3`
- `attackReleaseMilliseconds`, unless you have confirmed that the released application behavior matches the intended protocol change

### Validation Rules Enforced By The Released Application

The current application validates the following conditions:

- every expected session must exist exactly once,
- each session must define at least one frequency,
- timing values must be positive,
- `startToneDurationMilliseconds` must be greater than or equal to `endToneDurationMilliseconds`,
- `requiredValidResponses` must be greater than `0`,
- `attackReleaseMilliseconds` cannot be negative,
- `durationMapping` must be `"hyperbolic"`.

### Research Guidance

If you modify any experiment value, treat that modification as a protocol change. Record it in study notes, version control, or release notes, and retain the edited JSON together with the collected data.

## `questionnaire-config.json`

This file defines the questionnaire wording and form structure shown to participants.

### Typical Safe Edits

The following changes are appropriate when revising participant-facing wording while preserving the released export structure:

- editing `label` text,
- editing participant-facing `options[].label` values,
- adjusting `required` when the study protocol intentionally changes whether an answer is mandatory,
- adjusting `min`, `max`, and `step` for integer fields when the scientific meaning of the item remains unchanged,
- updating `version` when questionnaire wording or structure changes in a way that should be tracked.

### Fields To Treat As Stable Contracts

The following values should not be changed casually:

- `fieldId`, because it is tied to the released export schema,
- field ordering, because the current application validates the expected ordered field set,
- `type`, except within the set of field types supported by the released application,
- the separation between `musician` and `nonMusician` branches.

### `visibleWhen`

`visibleWhen` controls conditional display of a field. Use it only when:

- the controlling `fieldId` already exists in the same branch,
- the controlling value matches an actual stored value,
- the hidden field should not contribute an answer when the condition is not met.

### Validation Rules Enforced By The Released Application

The current application validates the following conditions:

- each branch must define a `version`,
- each branch must define the expected ordered field set,
- `fieldId` values must be unique within a branch,
- every field must define a supported `type`,
- `choice` fields must define a non-empty `options` array,
- `text` fields cannot define `options`,
- integer numeric fields must define `min`, `max`, and `step`,
- integer numeric fields must satisfy `max >= min` and `step > 0`,
- `visibleWhen.fieldId` must point to another field in the same branch,
- `visibleWhen` cannot reference the field on which it is declared.

### Research Guidance

The questionnaire configuration is intentionally constrained. It supports controlled changes to wording and presentation, but it is not designed as an unrestricted mechanism for inventing new exported variables. If the study requires a genuinely new questionnaire variable, treat that as a code-and-protocol change rather than a wording-only edit.

## Minimal Validation Checklist Before Data Collection

Before using a changed configuration in participant testing, confirm all of the following:

1. the JSON file is valid,
2. the application starts without a configuration error,
3. the correct questionnaire branch renders as expected,
4. a short validation run produces a valid result file,
5. the configuration used for data collection has been archived.
