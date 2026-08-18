# Android project reconstruction skills

Four Cursor skills, plus a pipeline skill that runs them in order.

**Canonical location:** `.cursor/skills/` (these are what Cursor loads).

The `*.env` files in this folder are the original notes. Prefer the `SKILL.md` files.

| Stage | Skill | Input | Output |
|-------|-------|-------|--------|
| 1 | `project-analyze` | `project/` | `analysis/` |
| 2 | `project-create` | `analysis/` + `project-template/` | `{project-name}/` at repo root |
| 3 | `project-build` | `project/` + `analysis/` + `{project-name}/` | reconstructed project |
| 4 | `play-policy-compliance` | all of the above | `compliance/` |
| — | `project-rebuild-pipeline` | non-empty `project/` | runs 1→4 |

**Stage 1 always runs first.**

**Only two templates:** `project-template/kotlin_app_template`, `project-template/unity_app_template`.

- **Game** → Unity  
- **Everything else** → Kotlin

Stage 2 copies the template into a **named folder** you choose (e.g. `expense-tracker/`, `2d-game/`) — not a fixed `new-project/` path.
