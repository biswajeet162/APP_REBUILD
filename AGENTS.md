# Android rebuild workspace

This repo is a **four-stage pipeline** for apps you **own or are authorized** to rebuild.

## Folders

| Folder | What you do |
|--------|-------------|
| `project/` | Paste the decompiled/extracted app here, then ask the agent to analyze or rebuild. |
| `project-template/` | Clean starters: Flutter, React Native, and Kotlin native. Do not dump APKs here. |
| `analysis/` | Created by Stage 1. |
| `new-project/` | Created by Stage 2, filled in by Stage 3. |
| `compliance/` | Created by Stage 4. |
| `.cursor/skills/` | The Cursor skills the agent must follow. |

## Pipeline

1. **project-analyze** — detect Flutter / React Native / native / other; write `analysis/`
2. **project-create** — copy the matching template into `new-project/` and compile
3. **project-build** — reconstruct owned UI/behavior in `new-project/` (clean-room, not a smali paste)
4. **play-policy-compliance** — audit Play policy, IP, privacy, release readiness

If the user only names one stage, run that stage.

## Agent rules

- Read the matching skill under `.cursor/skills/` before acting.
- If `project/` is empty, ask the user to paste the dump there.
- Do not copy keystores, tokens, or another publisher's identity.
- Do not help evade Play review or crack licensing.
