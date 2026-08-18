---
name: project-create
description: >-
  Creates a clean buildable app in new-project/ by copying the matching
  Flutter or React Native template from project-template/ after Stage 1
  analysis. Use when the user asks to create the new project, scaffold,
  copy the template, Stage 2, or project-create.
---

# PROJECT_CREATE (Stage 2)

Act as a senior Android scaffolding engineer. Build a **dummy but compiling** project that matches Stage 1 technology. Reconstruction of features is Stage 3.

## Inputs

Required:

- `analysis/analysis-report.md`
- `analysis/technology-detection.json`
- `analysis/reconstruction-plan.md`

If those files are missing, run [project-analyze](../project-analyze/SKILL.md) first.

## Template map

| `detected_technology` | Action |
|-----------------------|--------|
| `flutter` | Copy `project-template/flutter_app_template` → `new-project/` |
| `react_native` | Copy `project-template/react_native_app_template` → `new-project/` |
| `kotlin` | Copy `project-template/kotlin_app_template` → `new-project/` |
| `java` | Copy `project-template/kotlin_app_template` → `new-project/` (pure Kotlin starter; rewrite in Java in Stage 3 only if required) |
| `ionic` / `capacitor` / `cordova` | Scaffold with the official Ionic/Capacitor CLI into `new-project/`. |
| `unity` / `unreal` / `godot` / `maui` / `unknown` | Do **not** guess. Write `new-project/PROJECT_STATUS.md` explaining why a mobile template was not copied. Stop. |

## Copy rules (Flutter / RN)

Run from the workspace root:

```powershell
powershell -File .cursor/skills/project-create/scripts/copy-template.ps1 -Technology flutter
# or
powershell -File .cursor/skills/project-create/scripts/copy-template.ps1 -Technology react_native
# or
powershell -File .cursor/skills/project-create/scripts/copy-template.ps1 -Technology kotlin
```

The script **replaces** `new-project/` and excludes `node_modules`, `build`, `.dart_tool`, `.gradle`, `.idea`, `.cxx`, `Pods`.

Do not copy those directories by hand. The RN template's `node_modules` and Android build trees are huge and must stay out of `new-project/`.

## After copy — customize, do not clone identity blindly

Keep the template's **debug** signing. Never copy keystores/certs from `project/`.

Set display name from the analyzed app label when the user owns the app.

**applicationId / namespace / bundle id:**

- If the user owns this package and needs the same Play identity, keep the analyzed applicationId.
- If ownership is unconfirmed, or the id belongs to another publisher, generate a new unique id and document it. Do not impersonate.

Then:

1. Flutter: `flutter pub get` in `new-project/`. Optionally rename `pubspec.yaml` `name` and Android `applicationId`/`namespace`.
2. React Native: `npm install` in `new-project/` (never copy `node_modules`).
3. Kotlin native: ensure `local.properties` has `sdk.dir`. Optionally rename package/`applicationId`/app label.
4. Leave a working Hello/diagnostic screen from the template.

Add empty layers only as stubs if the reconstruction plan needs them (e.g. `lib/screens/`, `src/screens/`, `src/services/`) — no fake business logic.

## Validate build (required before Stage 3)

From `new-project/`:

- Flutter: `flutter build apk --debug`
- React Native Android: `npm run android` is optional; required compile check is `cd android; .\gradlew.bat assembleDebug`
- Kotlin native: `.\gradlew.bat assembleDebug` from `new-project/`

Fix scaffold errors until compile succeeds. Do not start Stage 3 on a broken project.

## Outputs

- `new-project/` (buildable)
- `new-project/README.md` — technology, toolchain, structure, build/run commands, unresolved items
- `new-project/PROJECT_STATUS.md` — copied template path, applicationId decision, build command used, pass/fail, next Stage 3 steps

## Do not

- Copy decompiled Java/Kotlin/smali/Dart/JS into the scaffold
- Copy signing keys, `google-services.json` from another publisher, tokens, or proprietary `.so` binaries
- Invent source that Stage 1 did not support

## Next stage

[project-build](../project-build/SKILL.md)
