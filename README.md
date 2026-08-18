# Android rebuild workspace

Authorized reconstruction pipeline: analyze a decompiled dump, copy a clean template, rebuild owned functionality, then audit Google Play policy.

## How to use

1. Paste your decompiled/extracted app into [`project/`](project/).
2. In Cursor, ask to **analyze** or **rebuild** the project.
3. The agent follows `.cursor/skills/`:
   1. Analyze → `analysis/`
   2. Create (copy Flutter, React Native, or Kotlin template) → `new-project/`
   3. Reconstruct owned UI/behavior in `new-project/`
   4. Play policy audit → `compliance/`

Templates live in `project-template/`. Skills live in `.cursor/skills/`. See `AGENTS.md` and `skills/README.md`.
