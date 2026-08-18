---
name: project-rebuild-pipeline
description: >-
  Runs the authorized Android rebuild pipeline: analyze a decompiled dump in
  project/, copy a matching Flutter or React Native template into new-project/,
  reconstruct owned UI/functionality, then audit Google Play policy. Use when
  the user pastes a decompiled app into project/, asks to analyze/rebuild/
  reconstruct an APK, or mentions project-analyze, project-create, project-build,
  or play-policy-compliance.
---

# Project rebuild pipeline

Run this pipeline for apps the user **owns or is explicitly authorized** to rebuild (lost source, own APK, written license). Do not reconstruct another publisher's app, crack licenses, bypass DRM, or clone store listings.

## Workspace contract

| Path | Role |
|------|------|
| `project/` | User drops the decompiled/extracted app here. Do not invent contents. |
| `project-template/` | Clean starters. Currently `flutter_app_template` and `react_native_app_template`. |
| `analysis/` | Stage 1 reports. Create this folder. |
| `new-project/` | Stage 2 scaffold, then Stage 3 reconstruction. |
| `compliance/` | Stage 4 Play policy reports. |

If `project/` is missing or empty, stop and tell the user to paste the decompiled project into `project/`.

## Authorization gate

Before Stage 2 or 3:

1. Confirm the dump in `project/` is an app the user owns or is authorized to rebuild.
2. If ownership is not confirmed, run **Stage 1 only** (high-level analysis) and stop. Do not copy assets, recreate UI, or publish-prep another publisher's product.

## Stages

Read and follow each skill in order. Do not skip a skill file.

1. [project-analyze](../project-analyze/SKILL.md) → write `analysis/`
2. [project-create](../project-create/SKILL.md) → copy template into `new-project/` and prove it builds
3. [project-build](../project-build/SKILL.md) → reconstruct owned behavior into `new-project/`
4. [play-policy-compliance](../play-policy-compliance/SKILL.md) → write `compliance/`

If the user names a single stage (`analyze`, `create`, `build`, `play policy`), run only that stage and its required inputs.

## Stop conditions

- Unknown technology with low confidence → finish Stage 1, do not guess a template.
- Stage 2 build fails → fix the scaffold before Stage 3.
- Third-party IP / secrets / signing material found → do not copy it; record it in the gap/policy reports.
- User asks to evade Play review, disguise copied code, or reuse another app's package/signature → refuse.

## Progress

Track in the conversation:

```
Pipeline:
- [ ] Stage 1 analyze
- [ ] Stage 2 create + compile
- [ ] Stage 3 reconstruct
- [ ] Stage 4 Play policy
```
