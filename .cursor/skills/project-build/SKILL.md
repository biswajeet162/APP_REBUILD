---
name: project-build
description: >-
  Reconstructs authorized UI and functionality inside the Stage 2 project folder
  from Stage 1 analysis and the decompiled dump in project/. Clean-room rebuild,
  not a proprietary-source copy. Use when the user asks to build, reconstruct,
  port screens, copy owned assets, Stage 3, or project-build.
---

# PROJECT_BUILD (Stage 3)

Act as a senior reconstruction engineer. Work **in** the project folder created by Stage 2 (e.g. `calculator/`, `2d-game/` — see `analysis/project-create-status.json` → `project_folder`). The dump in `project/` is a **reference**, not a file tree to paste over the new app.

## Inputs

- `project/` (authorized reference)
- `analysis/analysis-report.md`
- `analysis/technology-detection.json`
- `analysis/reconstruction-plan.md`
- `analysis/project-create-status.json` (project folder name and template)
- `{project_folder}/` that already **compiles** (Kotlin) or opens in Unity (games)

If the Stage 2 folder is missing or Kotlin scaffold does not build, run [project-create](../project-create/SKILL.md) first.

## Legal boundary

This is for apps the user owns or is authorized to rebuild.

- Recreate observable UI/UX and behavior in **new** Kotlin or Unity source.
- Do not dump smali, jadx Java, IL2CPP, or minified bundles into the project folder as the app source.
- Do not copy signing keys, tokens, DRM, or another publisher's `google-services.json`.
- Do not bypass licensing, paywalls, authentication, or integrity checks.
- Do not present the result as the original source.
- Third-party art/fonts/audio without a license → original placeholders + gap report.

Owned, non-secret resources **may** be copied when the user has rights: launcher icons, images, colors, strings, fonts they created. Record each copy in `RECONSTRUCTION_STATUS.md`.

## Reconstruction order

Build and smoke-check after each major slice:

1. App startup / theme
2. Navigation / routes
3. Main screen
4. Shared UI components
5. Local state / storage
6. Individual features
7. Network layer (owned APIs or clearly marked mocks)
8. Background / native integrations
9. Error / loading / empty states
10. Visual refinement
11. Debug build validation

## What to port vs rewrite

| From `project/` | Action |
|-----------------|--------|
| `res/values/colors.xml`, `strings.xml`, `themes` (native) | Port into Kotlin Compose theme or Unity UI if owned |
| Layout XML (native) | Recreate as Compose screens or Unity UI — do not keep smali |
| Images/fonts from decompiled assets | Copy only owned assets into `{project_folder}/` |
| Activities / route names | Recreate equivalent screens/scenes and navigation |
| API hosts the user owns | Wire with new client code; keep secrets out of the repo |
| Unknown backends | Mock adapters, marked `MOCK` |
| `.so` / IL2CPP / engine data | Do not copy |

## Outputs

Update `{project_folder}/` plus:

- `{project_folder}/RECONSTRUCTION_STATUS.md` — what was implemented, asset copies, mocks
- `{project_folder}/GAP_REPORT.md` — reference behavior not yet rebuilt
- `{project_folder}/BUILD_VALIDATION.md` — commands run, result, crash notes

## Quality gates

- `{project_folder}/` still compiles (Kotlin) or builds in Unity
- App has a defined route/screen for every implemented flow
- No secrets in source or resources
- Permissions are minimized and justified (do not inherit the dump's dangerous permissions by default)
- Missing work is listed in the gap report

## Next stage

[play-policy-compliance](../play-policy-compliance/SKILL.md)
