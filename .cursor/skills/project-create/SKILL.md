---
name: project-create
description: >-
  Stage 2 — copies kotlin_app_template or unity_app_template from project-template/
  into a named folder at the workspace root. Game → Unity; everything else → Kotlin.
  For Unity: ask the user for the game name, copy the base template, never edit the
  template in place. Use when the user asks to create, scaffold, copy the template,
  new Unity project, new game, Stage 2, or project-create.
---

# PROJECT_CREATE (Stage 2)

Act as a senior scaffolding engineer. **Copy only** — do not invent a new toolchain. Feature reconstruction is Stage 3.

## Golden rule

**Never build in `project-template/`.** Templates are read-only bases. Every app gets its own folder at the repo root (e.g. `calculator/`, `2d-racer/`).

## Prerequisites

Run [project-analyze](../project-analyze/SKILL.md) first **when rebuilding from a decompiled dump in `project/`**.

If the user explicitly asks to **create a new Unity game** (no dump yet), skip analysis and go straight to **Ask name → Copy template**.

Required inputs (when analysis ran):

- `analysis/analysis-report.md`
- `analysis/technology-detection.json`
- `analysis/reconstruction-plan.md`

## Template selection (strict — only two)

| Analysis result | Template to copy |
|-----------------|------------------|
| `app_category` = **game** OR `is_game` = true OR user asks for Unity/game | `project-template/unity_app_template` |
| **Everything else** | `project-template/kotlin_app_template` |

Do **not** use Flutter, React Native, or any other template.

## Unity — ask for the game name first

When creating a **Unity** project:

1. **Ask the user:** “What should the game folder be called?” (e.g. `2d-racer`, `puzzle-game`, `space-shooter`)
2. If they already gave a name in chat, use it.
3. Normalize to kebab-case: `2D Racer` → `2d-racer`
4. Copy `project-template/unity_app_template` → `{game-name}/`
5. Confirm: “Copied Unity base to `{game-name}/`. We’ll build on top of this.”

Do **not** rename or customize the template folder itself. Do **not** skip asking if the name is unclear.

## Kotlin — project folder name

Same rules: ask or use analysis `suggested_project_folder`. Normalize to kebab-case.

**Do not** use `new-project/` unless the user explicitly asks for that name.

## Copy command

Run from the workspace root:

```powershell
powershell -File .cursor/skills/project-create/scripts/copy-template.ps1 -Technology kotlin -ProjectName expense-tracker
powershell -File .cursor/skills/project-create/scripts/copy-template.ps1 -Technology unity -ProjectName 2d-racer
```

The script copies into `{ProjectName}/` and excludes build caches (`Library`, `Temp`, `Logs`, `Builds`, `build`, `.gradle`, etc.).

## After copy — minimal customization only

Keep the template's **debug** signing. Never copy keystores/certs from `project/`.

1. **Kotlin:** ensure `{ProjectName}/local.properties` has `sdk.dir`. Optionally set app label and `applicationId`.
2. **Unity:** optionally set product name in README; Unity generates `Library/` on first open. The copied project already includes `BuildAndroid.cs` for phone builds. **Before Android builds on Windows, read `project-template/unity_app_template/UNITY_ANDROID_RUNBOOK.md`.**
3. Leave the starter screen/scene from the template.

Add empty stub folders only if needed (`Assets/Scripts/Game/`). No fake business logic in Stage 2.

## Validate build (required before Stage 3)

From `{ProjectName}/`:

- **Kotlin:** `.\gradlew.bat assembleDebug`
- **Unity:** batch build or Unity Editor open; APK via `BuildAndroid.BuildDebugApk` (see template README)

## Outputs

- `{ProjectName}/` — copied template
- `{ProjectName}/PROJECT_STATUS.md` — template copied, build status, next steps
- `analysis/project-create-status.json` (when analysis exists):

```json
{
  "project_folder": "2d-racer",
  "template": "unity_app_template",
  "copy_source": "project-template/unity_app_template",
  "build_validated": true
}
```

## Do not

- Edit `project-template/unity_app_template` or `project-template/kotlin_app_template` for a specific game/app
- Copy decompiled IL2CPP/smali into the new folder
- Copy signing keys or another publisher's identity

## Next stage

[project-build](../project-build/SKILL.md)
