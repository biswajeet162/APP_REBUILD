# Android rebuild workspace

This repo is a **four-stage pipeline** for products you **own or are authorized** to rebuild.

Source in `project/` can be **any technology** (web, iOS, Android, Flutter, Unity, backend, desktop). Stage 2 always creates **Kotlin Android** (non-game) or **Unity** (game).

## Folders

| Folder | What you do |
|--------|-------------|
| `project/` | Paste the source project here (any stack). **Stage 1 analyzes this.** |
| `project-template/` | **Read-only bases:** `kotlin_app_template`, `unity_app_template`. Copy to `{name}/`; never build games/apps here. |
| `analysis/` | Created by Stage 1 (reports + `rebuild-metadata.json`). |
| `{project-name}/` | Created by Stage 2 (e.g. `calculator/`, `2d-game/`). Stage 3 work happens here. |
| `compliance/` | Created by Stage 4. |
| `.cursor/skills/` | The Cursor skills the agent must follow. |

## Pipeline

1. **project-analyze** — always first; understand `project/` (any tech: purpose, architecture, models, network, flows) → `analysis/`
2. **project-create** — copy **Kotlin** (non-game) or **Unity** (game), then **apply** Stage 1 metadata into `{project-name}/`
3. **project-build** — reconstruct owned UI/behavior in `{project-name}/`
4. **play-policy-compliance** — audit Play policy, IP, privacy, release readiness

If the user only names one stage, run that stage.

## Agent rules

- Read the matching skill under `.cursor/skills/` before acting.
- **Stage 1 always runs before Stage 2.**
- Only two templates: Kotlin or Unity. Game → Unity; everything else → Kotlin.
- Stage 2 must apply `analysis/rebuild-metadata.json` (models, network stubs, screens) — not a blank starter.
- **Never edit `project-template/` for a specific app** — always copy to `{project-name}/` first.
- New Unity game: ask user for game name → copy `unity_app_template` → build in `{game-name}/`.
- If `project/` is empty, ask the user to paste the source project there.
- Do not copy keystores, tokens, or another publisher's identity.
- Do not help evade Play review or crack licensing.
