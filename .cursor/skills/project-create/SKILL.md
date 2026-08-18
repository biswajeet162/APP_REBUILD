---
name: project-create
description: >-
  Stage 2 — copies kotlin_app_template or unity_app_template from project-template/
  into a named folder at the workspace root after Stage 1 analysis. Game → Unity;
  everything else → Kotlin. Use when the user asks to create, scaffold, copy
  the template, Stage 2, or project-create.
---

# PROJECT_CREATE (Stage 2)

Act as a senior scaffolding engineer. **Copy only** — do not invent a new toolchain. Feature reconstruction is Stage 3.

## Prerequisites

Run [project-analyze](../project-analyze/SKILL.md) first. Required inputs:

- `analysis/analysis-report.md`
- `analysis/technology-detection.json`
- `analysis/reconstruction-plan.md`

If those files are missing, run Stage 1 before continuing.

## Template selection (strict — only two)

| Analysis result | Template to copy |
|-----------------|------------------|
| `app_category` = **game** OR `is_game` = true OR `recommended_template` = `unity_app_template` | `project-template/unity_app_template` |
| **Everything else** | `project-template/kotlin_app_template` |

Do **not** use Flutter, React Native, or any other template. Do **not** scaffold from CLI unless the user explicitly overrides the pipeline.

## Project folder name

Copy into a **new folder at the workspace root** named by the user or by analysis:

- Prefer the name the user gives in chat (e.g. `expense tracker`, `calculator`, `2D game`).
- If none given, use `suggested_project_folder` from `technology-detection.json`.
- Normalize to a safe folder name (spaces → hyphens, lowercase): `expense-tracker`, `calculator`, `2d-game`.

**Do not** use `new-project/` unless the user explicitly asks for that name.

## Copy command

Run from the workspace root:

```powershell
powershell -File .cursor/skills/project-create/scripts/copy-template.ps1 -Technology kotlin -ProjectName expense-tracker
# or
powershell -File .cursor/skills/project-create/scripts/copy-template.ps1 -Technology unity -ProjectName 2d-game
```

`-Technology` must be `kotlin` or `unity` and must match Stage 1's `recommended_template`.

The script copies the template into `{ProjectName}/` and excludes build caches (`build`, `.gradle`, `Library`, `Temp`, `Logs`, `node_modules`, `.idea`, `.cxx`, etc.).

Do not copy those directories by hand.

## After copy — minimal customization only

Keep the template's **debug** signing. Never copy keystores/certs from `project/`.

1. **Kotlin:** ensure `{ProjectName}/local.properties` has `sdk.dir`. Optionally set app label, `applicationId`, and package from analysis when the user owns the app.
2. **Unity:** set product name in README / ProjectSettings notes; open in Unity Editor to refresh if needed.
3. Leave the template's starter screen/scene — do not paste decompiled code.

Add empty stub folders only if the reconstruction plan needs them (`ui/screens/`, `Assets/Scripts/Game/`). No fake business logic.

## Validate build (required before Stage 3)

From `{ProjectName}/`:

- **Kotlin:** `.\gradlew.bat assembleDebug`
- **Unity:** confirm project opens in Unity Editor; Android export setup documented in template README (full IL2CPP build requires Unity Hub)

Fix scaffold errors until the Kotlin project compiles. Do not start Stage 3 on a broken Kotlin scaffold.

## Outputs

- `{ProjectName}/` — copied template (buildable for Kotlin)
- `{ProjectName}/README.md` — update or append: technology choice, folder name, build/run commands
- `{ProjectName}/PROJECT_STATUS.md` — template copied, `applicationId`/product name decision, build command, pass/fail, next Stage 3 steps

Also write `analysis/project-create-status.json`:

```json
{
  "project_folder": "expense-tracker",
  "template": "kotlin_app_template|unity_app_template",
  "copy_source": "project-template/kotlin_app_template",
  "build_validated": true,
  "build_command": ".\\gradlew.bat assembleDebug"
}
```

## Do not

- Copy decompiled smali, jadx output, IL2CPP binaries, or proprietary bundles into the new folder
- Copy signing keys, tokens, or another publisher's `google-services.json`
- Copy third-party icons, logos, or branded assets from `project/` without confirmed ownership

## Next stage

[project-build](../project-build/SKILL.md)
