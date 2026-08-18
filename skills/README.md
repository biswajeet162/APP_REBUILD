# Android project reconstruction skills

Four Cursor skills, plus a pipeline skill that runs them in order.

**Canonical location:** `.cursor/skills/` (these are what Cursor loads).

The `*.env` files in this folder are the original notes. Prefer the `SKILL.md` files.

| Stage | Skill | Input | Output |
|-------|-------|-------|--------|
| 1 | `project-analyze` | `project/` | `analysis/` |
| 2 | `project-create` | `analysis/` + `project-template/` | `new-project/` (must compile) |
| 3 | `project-build` | `project/` + `analysis/` + `new-project/` | reconstructed `new-project/` |
| 4 | `play-policy-compliance` | all of the above | `compliance/` |
| — | `project-rebuild-pipeline` | non-empty `project/` | runs 1→4 |

Templates today: `project-template/flutter_app_template`, `project-template/react_native_app_template`.
