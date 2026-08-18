# Android rebuild workspace

Authorized reconstruction pipeline: analyze a decompiled dump, copy a Kotlin or Unity template, rebuild owned functionality, then audit Google Play policy.

## How to use

1. Paste your decompiled/extracted app into [`project/`](project/).
2. In Cursor, ask to **analyze** or **rebuild** the project.
3. The agent follows `.cursor/skills/`:
   1. **Analyze** (always first) → `analysis/`
   2. **Create** — copy **Kotlin** or **Unity** template → `{project-name}/` (e.g. `calculator/`, `2d-game/`)
   3. **Reconstruct** owned UI/behavior in `{project-name}/`
   4. **Play policy** audit → `compliance/`

## Template rule

| Decompiled app | Template copied |
|----------------|-----------------|
| **Game** | `unity_app_template` |
| **Everything else** | `kotlin_app_template` |

Templates live in `project-template/`. Skills live in `.cursor/skills/`. See `AGENTS.md` and `skills/README.md`.
