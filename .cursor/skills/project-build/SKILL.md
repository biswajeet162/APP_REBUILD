---
name: project-build
description: >-
  Stage 3 — reconstructs authorized UI and functionality inside the named project
  folder (e.g. 2d-racer/, calculator/) copied from kotlin or unity template. Triggered
  by /build, build, reconstruct, or project-build. Never edit project-template/ in
  place; work only in the user's project folder.
---

# PROJECT_BUILD (Stage 3)

Act as a senior reconstruction engineer. Work **only** in the user's project folder (e.g. `2d-racer/`, `calculator/`) — **never** in `project-template/`.

## Triggers

- User says **`/build`**, **build**, **reconstruct**, or **project-build**
- User wants to implement features on top of an existing copied project

## Before building — confirm project folder

1. Check `analysis/project-create-status.json` → `project_folder`, **or**
2. Ask the user which project folder to use, **or**
3. If no project exists yet and it's a **Unity game**: run [project-create](../project-create/SKILL.md) first — **ask game name**, copy `unity_app_template` → `{game-name}/`

If `{project_folder}/` is missing, create it via Stage 2 before writing code.

## Inputs

- `project/` (authorized reference dump, if rebuilding from decompile)
- `analysis/*` (when available)
- `{project_folder}/` — the copied Kotlin or Unity project

## Unity build workflow

All Unity code changes go in `{game-name}/`, not the template:

```
{game-name}/
  Assets/Scripts/       ← game logic here
  Assets/Scenes/        ← scenes here
  Assets/Editor/        ← BuildAndroid.cs (from template copy)
```

After code changes, build to phone:

```powershell
$env:GRADLE_USER_HOME = "D:\gradle"
$env:TEMP = "D:\tmp"
$env:TMP = "D:\tmp"
adb install -r Builds\Android\unity-template-debug.apk
```

Use `BuildAndroid.BuildDebugApk` from the copied project (see `unity_app_template` README).

**Windows Android build issues:** read `project-template/unity_app_template/UNITY_ANDROID_RUNBOOK.md` first (NDK version, path length, Gradle cache, launch activity).

## Kotlin build workflow

All Kotlin changes go in `{app-name}/app/src/main/kotlin/...`. Build with `.\gradlew.bat assembleDebug`.

## Legal boundary

For apps the user owns or is authorized to rebuild:

- Recreate UI/UX and behavior in **new** Kotlin or Unity source
- Do not dump smali, IL2CPP, or proprietary bundles into the project folder
- Do not copy signing keys or third-party branded assets without rights

## Reconstruction order

1. App startup / theme
2. Navigation / scenes
3. Main screen / core loop
4. Shared components
5. Features, storage, networking
6. Polish and debug build validation

## Outputs

Update `{project_folder}/` plus:

- `{project_folder}/RECONSTRUCTION_STATUS.md`
- `{project_folder}/GAP_REPORT.md`
- `{project_folder}/BUILD_VALIDATION.md`

## Quality gates

- Project still builds (Kotlin compile or Unity APK)
- No secrets in source
- Template base in `project-template/` remains unchanged

## Next stage

[play-policy-compliance](../play-policy-compliance/SKILL.md)
