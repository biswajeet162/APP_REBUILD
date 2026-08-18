---
name: project-rebuild-pipeline
description: >-
  Runs the authorized Android rebuild pipeline: analyze project/, copy Kotlin or
  Unity template into a named project folder, reconstruct owned UI/functionality,
  then audit Google Play policy. Use when the user pastes a decompiled app,
  asks to analyze/rebuild, or mentions project-analyze, project-create,
  project-build, or play-policy-compliance.
---

# Project rebuild pipeline

Run this pipeline for apps the user **owns or is explicitly authorized** to rebuild. Do not reconstruct another publisher's app, crack licenses, bypass DRM, or clone store listings.

## Workspace contract

| Path | Role |
|------|------|
| `project/` | User drops the decompiled/extracted app here. **Stage 1 input.** |
| `project-template/` | **Only two starters:** `kotlin_app_template`, `unity_app_template`. |
| `analysis/` | Stage 1 reports. |
| `{project-name}/` | Stage 2 output — named folder at repo root (e.g. `calculator/`, `2d-game/`). Stage 3 work happens here. |
| `compliance/` | Stage 4 Play policy reports. |

If `project/` is missing or empty, stop and tell the user to paste the decompiled project into `project/`.

## Stage 1 always first

**project-analyze** must run before **project-create**. Never skip analysis.

## Template rule (strict)

| Decompiled app | Stage 2 template |
|----------------|------------------|
| **Game** | Copy `unity_app_template` |
| **Everything else** | Copy `kotlin_app_template` |

No Flutter. No React Native. Copy only — into a user-named folder under the repo root.

## Authorization gate

Before Stage 2 or 3:

1. Confirm the dump in `project/` is an app the user owns or is authorized to rebuild.
2. If ownership is not confirmed, run **Stage 1 only** and stop.

## Stages

Read and follow each skill in order:

1. [project-analyze](../project-analyze/SKILL.md) → write `analysis/`
2. [project-create](../project-create/SKILL.md) → copy template into `{project-name}/` and prove Kotlin builds
3. [project-build](../project-build/SKILL.md) → reconstruct owned behavior in `{project-name}/`
4. [play-policy-compliance](../play-policy-compliance/SKILL.md) → write `compliance/`

If the user names a single stage, run only that stage and its required inputs.

## Stop conditions

- Stage 1 incomplete → do not run Stage 2
- Destination folder already exists → ask for another name or confirm overwrite
- Kotlin scaffold build fails → fix before Stage 3
- Third-party IP / secrets / signing material → do not copy; record in reports
- User asks to evade Play review or reuse another app's package/signature → refuse

## Progress

```
Pipeline:
- [ ] Stage 1 analyze (project/)
- [ ] Stage 2 create + compile ({project-name}/)
- [ ] Stage 3 reconstruct
- [ ] Stage 4 Play policy
```
