---
name: project-rebuild-pipeline
description: >-
  Runs the authorized rebuild pipeline: analyze any-technology source in
  project/, copy Kotlin or Unity template and apply analysis metadata, reconstruct
  owned UI/functionality, then audit Google Play policy. Use when the user pastes
  a project, asks to analyze/rebuild, or mentions project-analyze, project-create,
  project-build, or play-policy-compliance.
---

# Project rebuild pipeline

Run this pipeline for products the user **owns or is explicitly authorized** to rebuild. Do not reconstruct another publisher's app, crack licenses, bypass DRM, or clone store listings.

Source in `project/` may be **any technology**. Stage 2 always targets Kotlin Android or Unity.

## Workspace contract

| Path | Role |
|------|------|
| `project/` | User drops the source project here (any stack). **Stage 1 input.** |
| `project-template/` | **Only two starters:** `kotlin_app_template`, `unity_app_template`. |
| `analysis/` | Stage 1 reports + `rebuild-metadata.json` (the picture Stage 2 applies). |
| `{project-name}/` | Stage 2 output — named folder at repo root (e.g. `calculator/`, `2d-game/`). Stage 3 work happens here. |
| `compliance/` | Stage 4 Play policy reports. |

If `project/` is missing or empty, stop and tell the user to paste the source project into `project/`.

## Stage 1 always first

**project-analyze** must run before **project-create**. Never skip analysis.

## Template rule (strict)

| Source product | Stage 2 template |
|----------------|------------------|
| **Game** | Copy `unity_app_template`, then apply `analysis/rebuild-metadata.json` |
| **Everything else** | Copy `kotlin_app_template`, then apply `analysis/rebuild-metadata.json` |

Original stack can be React, iOS, Flutter, Node, Android, etc. Target is still only Kotlin or Unity. Never edit `project-template/` in place.

## Authorization gate

Before Stage 2 or 3:

1. Confirm the source in `project/` is a product the user owns or is authorized to rebuild.
2. If ownership is not confirmed, run **Stage 1 only** and stop.

## Stages

Read and follow each skill in order:

1. [project-analyze](../project-analyze/SKILL.md) → write `analysis/` including `rebuild-metadata.json`
2. [project-create](../project-create/SKILL.md) → copy template, apply metadata into `{project-name}/`, prove debug build
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
- [ ] Stage 2 create + apply metadata + compile ({project-name}/)
- [ ] Stage 3 reconstruct
- [ ] Stage 4 Play policy
```
