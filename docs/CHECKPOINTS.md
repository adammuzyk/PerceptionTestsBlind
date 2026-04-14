# Checkpoint Files

`PerceptionTests` writes intermediate checkpoint files during data collection to reduce the risk of data loss before the final export step is reached.

## Purpose

Checkpoint files serve as recovery artifacts for interrupted or incomplete runs. They are not intended to replace the final participant result file.

## Location And Naming

Checkpoint files are written under:

```text
checkpoints/
```

using names of the form:

```text
checkpoint_<sessionId>.json
```

The checkpoint directory is created automatically under the configured result path.

## When A Checkpoint Is Written

A checkpoint is saved after each completed listening session. This means a checkpoint may contain:

- questionnaire data collected so far,
- completed session results up to that point,
- the active metadata block,
- the experiment-configuration snapshot for that run.

As a result, checkpoint files can preserve substantial progress even when a participant does not reach the final thank-you page.

## Relationship To Final Result Files

Checkpoint files and final result files use the same export structure, but they serve different roles.

- Checkpoints are intermediate safety saves.
- Final files are the canonical outputs for completed participant runs.

Final files are written as numbered JSON files such as:

```text
testResult_0001.json
testResult_0002.json
```

When the final save succeeds, the application deletes the corresponding checkpoint file for that participant run.

## Recommended Research Use

For routine analysis:

1. Treat final `testResult_XXXX.json` files as the primary dataset.
2. Use checkpoint files only for recovery, troubleshooting, or inspection of interrupted runs.
3. Do not analyze a checkpoint file together with the corresponding final file from the same participant.

## How To Recognize An Interrupted Run

A checkpoint file may indicate an interrupted or incomplete run when one or more of the following conditions apply:

- a checkpoint exists but no final `testResult_XXXX.json` was written for that participant run,
- one or more session entries remain `null`,
- the run did not reach the final export step.

## Archival Guidance

For long-term retention, the most important artifacts are usually:

- final participant result files,
- the released application version,
- the experiment and questionnaire configurations used for data collection,
- any downstream analysis code or derived datasets.

Checkpoint files are useful operationally, but they are typically not the primary archival record once a successful final export has been confirmed.
