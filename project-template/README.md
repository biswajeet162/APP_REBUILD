# Project templates (Stage 2)

Only **two** starters are used by the pipeline:

| Template | Use when | Editable? |
|----------|----------|-----------|
| `kotlin_app_template/` | Non-game apps | **Base only** — copy to `{name}/`, work in the copy |
| `unity_app_template/` | Games | **Base only** — copy to `{name}/`, work in the copy |

**Never modify these template folders for a specific app.** Always copy first, then build in `{project-name}/`.

**Unity Android issues?** See `unity_app_template/UNITY_ANDROID_RUNBOOK.md`.

**Export APK / AAB (share & Play Store)?** See `unity_app_template/UNITY_BUILD_APK.md`.

Legacy folders (`flutter_app_template`, `react_native_app_template`) are not used by current skills.

## Unity new-game flow

1. Ask user: **What is the game name?** (e.g. `2d-racer`)
2. Copy: `project-template/unity_app_template` → `{game-name}/`
3. Build/reconstruct in `{game-name}/` (Stage 3 / `/build`)

```powershell
powershell -File .cursor/skills/project-create/scripts/copy-template.ps1 -Technology unity -ProjectName 2d-racer
```
