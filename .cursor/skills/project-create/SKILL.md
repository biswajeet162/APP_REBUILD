---
name: project-create
description: >-
  Stage 2 — copies kotlin_app_template or unity_app_template, then applies
  Stage 1 rebuild metadata (architecture, models, network stubs, screens/scenes,
  identifiers) into a named folder. Game → Unity; everything else → Kotlin.
  Use when the user asks to create, scaffold, copy the template, new Unity
  project, new game, Stage 2, or project-create.
---

# PROJECT_CREATE (Stage 2)

Act as a senior scaffolding engineer. Copy the matching template, then **apply the Stage 1 picture**. Do not invent a new toolchain. Full feature reconstruction is Stage 3.

## Golden rule

**Never build in `project-template/`.** Templates are read-only bases. Every app gets its own folder at the repo root (e.g. `calculator/`, `2d-racer/`).

## Prerequisites

Run [project-analyze](../project-analyze/SKILL.md) first **when rebuilding from `project/`**.

Required inputs (when analysis ran):

- `analysis/rebuild-metadata.json` — **the picture; apply this**
- `analysis/analysis-report.md`
- `analysis/technology-detection.json`
- `analysis/reconstruction-plan.md`

If `rebuild-metadata.json` is missing, run Stage 1 before creating. Do not scaffold from memory.

If the user explicitly asks to **create a new Unity game** with no source in `project/`, skip analysis and go straight to **Ask name → Copy template** (no metadata to apply).

## Template selection (strict — only two)

Use the **conclusion** from analysis, not the original source technology.

| Analysis result | Template to copy |
|-----------------|------------------|
| `app_category` = **game** OR `is_game` = true OR user asks for Unity/game | `project-template/unity_app_template` |
| **Everything else** | `project-template/kotlin_app_template` |

A React, iOS, Flutter, Node, or Python source still becomes **Kotlin Android** unless it is a game.

Do **not** use Flutter, React Native, or any other template.

## Unity — ask for the game name first

When creating a **Unity** project:

1. **Ask the user:** “What should the game folder be called?” (e.g. `2d-racer`, `puzzle-game`, `space-shooter`)
2. If they already gave a name in chat, use it. Else use `suggested_project_folder` from metadata.
3. Normalize to kebab-case: `2D Racer` → `2d-racer`
4. Copy `project-template/unity_app_template` → `{game-name}/`
5. Apply `rebuild-metadata.json` (below)
6. Confirm: “Copied Unity base to `{game-name}/` and applied analysis metadata.”

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

## After copy — apply the analysis picture

Keep the template's **debug** signing. Never copy keystores/certs from `project/`.

Read `analysis/rebuild-metadata.json` and apply it to `{ProjectName}/`. This is the difference between an empty template and a project that matches the analyzed product.

### Always apply

1. Display name / product name from `display_name`
2. Application id / package / bundle hint when present (`application_id_or_bundle`, `target_scaffold.kotlin.package_hint`)
3. Folder/package skeleton from `architecture` and `target_scaffold`
4. **Domain model stubs** (data classes / ScriptableObjects / serializable types) matching `domain_models` — fields only, no fake business logic
5. **Network stubs** matching `network.endpoints` — interfaces/clients with method + path + DTO shapes; no real secrets; mock or empty implementations
6. **Screen / scene placeholders** matching `ui.screens` and `flows` — named destinations, empty or minimal UI, wired navigation where the template allows
7. Theme hints (colors/typography) only when they are **owned**, not third-party brands
8. `{ProjectName}/PROJECT_STATUS.md` listing what metadata was applied vs deferred to Stage 3

### Kotlin specifically

- Ensure `{ProjectName}/local.properties` has `sdk.dir`
- Set app label and `applicationId` from metadata when valid
- Create packages from `target_scaffold.kotlin.packages_to_create` (default: `ui`, `data`, `domain`, `network`)
- Map flows → navigation graph destinations
- Map models → `domain`/`data` types
- Map endpoints → Retrofit (or equivalent template API) interfaces

### Unity specifically

- Set product name from metadata
- Create scenes listed in `target_scaffold.unity.scenes` (or `ui.screens` if scenes are empty)
- Create script folders from `target_scaffold.unity.script_folders`
- Stub model types and a thin API/service layer for analyzed network calls
- **Before Android builds on Windows, read `project-template/unity_app_template/UNITY_ANDROID_RUNBOOK.md`.**

### Do not do in Stage 2

- Full feature implementation (that is Stage 3)
- Copy source files, smali, IL2CPP, or third-party binaries from `project/`
- Copy signing keys or another publisher's identity
- Invent extra screens, models, or APIs that analysis did not record

## Validate build (required before Stage 3)

From `{ProjectName}/`:

- **Kotlin:** `.\gradlew.bat assembleDebug`
- **Unity:** batch build or Unity Editor open; APK via `BuildAndroid.BuildDebugApk` (see template README)

If stubs break the compile, fix types/usings until debug build succeeds. Empty-but-compiling stubs are required.

## Outputs

- `{ProjectName}/` — copied template **with analysis metadata applied**
- `{ProjectName}/PROJECT_STATUS.md` — template copied, metadata applied, build status, next steps
- `analysis/project-create-status.json`:

```json
{
  "project_folder": "2d-racer",
  "template": "unity_app_template",
  "copy_source": "project-template/unity_app_template",
  "metadata_applied": true,
  "metadata_source": "analysis/rebuild-metadata.json",
  "applied": ["display_name", "models", "network_stubs", "screens"],
  "deferred_to_stage_3": ["business_logic", "owned_assets"],
  "build_validated": true
}
```

## Do not

- Edit `project-template/unity_app_template` or `project-template/kotlin_app_template` for a specific game/app
- Copy decompiled IL2CPP/smali into the new folder
- Copy signing keys or another publisher's identity
- Ignore `rebuild-metadata.json` and ship a blank starter

## Next stage

[project-build](../project-build/SKILL.md)
