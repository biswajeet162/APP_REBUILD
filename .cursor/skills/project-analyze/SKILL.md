---
name: project-analyze
description: >-
  Analyzes an authorized decompiled/extracted Android project in project/,
  detects Flutter vs React Native vs native vs other engines, and writes
  analysis/ reports plus a reconstruction plan. Use when the user asks to
  analyze a decompiled APK, jadx/apktool dump, Flutter/React Native artifacts,
  or to start Stage 1 / project-analyze.
---

# PROJECT_ANALYZE (Stage 1)

Act as a senior Android architecture analyst. Input is `project/` unless the user gives another path.

## Rules

- Separate **CONFIRMED** facts from **INFERRED** facts. Never present decompiled output as original source.
- Do not fabricate missing classes, names, comments, or architecture.
- Do not copy or echo secrets, keystores, certificates, API tokens, or signing material into reports (redact values).
- If ownership is not confirmed, analyze only; do not plan verbatim source recovery of third-party code.

## Workflow

1. Confirm `project/` has files. If empty, stop.
2. Inventory top-level layout (`AndroidManifest.xml`, `apktool.yml`, `smali/`, `sources/`, `resources/`, `assets/`, `lib/`, `pubspec.yaml`, `package.json`, `android/`).
3. Detect technology using [detection-signatures.md](detection-signatures.md). Record evidence paths.
4. Extract identifiers and structure:
   - package / applicationId / app label
   - manifest: activities, services, receivers, providers, permissions, queries
   - min/target SDK, Gradle/AGP clues
   - native `.so` names
   - assets, databases, shared prefs filenames
   - networking hosts/paths **without** embedding tokens
   - third-party SDKs (Firebase, ads, analytics, billing)
   - UI/navigation clues (activities, Flutter routes if present, RN screens if JS is readable)
5. Note what compilation/decompilation destroyed (Dart AOT, Hermes bytecode, obfuscated names, missing resources).
6. Write a Stage 2 plan: which template to copy, what can be reconstructed, what must stay placeholder.

## Outputs

Create `analysis/` and write:

- `analysis/analysis-report.md`
- `analysis/technology-detection.json`
- `analysis/reconstruction-plan.md`

### `technology-detection.json`

```json
{
  "detected_technology": "flutter|react_native|kotlin|java|ionic|capacitor|cordova|unity|unreal|godot|maui|unknown",
  "confidence": "high|medium|low",
  "evidence": ["path: why this file matters"],
  "package_name": "",
  "application_id": "",
  "app_label": "",
  "version_name": "",
  "version_code": null,
  "min_sdk": null,
  "target_sdk": null,
  "permissions": [],
  "components": { "activities": [], "services": [], "receivers": [], "providers": [] },
  "native_libs": [],
  "sdks": [],
  "lost_in_decompilation": [],
  "confirmed": [],
  "inferred": [],
  "recommended_template": "flutter_app_template|react_native_app_template|kotlin_app_template|none",
  "ownership_assumption": "user_owns|unconfirmed"
}
```

### `analysis-report.md`

Use headings: Executive summary, Technology, Identifiers, Manifest/components, Resources/assets, Native libs, SDKs, Networking (redacted), UI/navigation, Confirmed vs inferred, Lost information.

### `reconstruction-plan.md`

- Template to copy in Stage 2
- Screens/routes to recreate
- Data/storage layers
- Network mocks vs real owned APIs
- Assets the user may reuse **only if they own them**
- Explicit non-goals (no keystore copy, no third-party binaries, no DRM bypass)

## Next stage

After writing the three files, Stage 2 is [project-create](../project-create/SKILL.md).
