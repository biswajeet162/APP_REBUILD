---
name: project-analyze
description: >-
  Stage 1 — always run first. Analyzes the decompiled dump in project/ for
  technology, signatures, branding/copyright assets, styling, and structure.
  Writes analysis/ and recommends kotlin_app_template or unity_app_template
  for Stage 2. Use when the user pastes a decompiled app, asks to analyze,
  or starts the rebuild pipeline.
---

# PROJECT_ANALYZE (Stage 1)

**Always execute this skill first.** Input is `project/` unless the user gives another path.

Act as a senior Android reverse-engineering and software-architecture analyst.

## Rules

- Separate **CONFIRMED** facts from **INFERRED** facts. Never present decompiled output as original source.
- Do not fabricate missing classes, names, comments, or architecture.
- Do not copy or echo secrets, keystores, certificates, API tokens, or signing material into reports (redact values).
- If ownership is not confirmed, analyze only; do not plan verbatim source recovery of third-party code.
- Record third-party **brands, logos, icons, fonts, and styling** as copyright/IP observations — do **not** copy them into reports as reusable assets unless the user confirms ownership.

## Workflow

1. Confirm `project/` has files. If empty, stop and ask the user to paste the decompiled dump.
2. Inventory top-level layout (`AndroidManifest.xml`, `apktool.yml`, `smali/`, `sources/`, `resources/`, `assets/`, `lib/`, Gradle files, engine folders).
3. Detect **original runtime technology** using [detection-signatures.md](detection-signatures.md). Record evidence paths.
4. Classify **app category**:
   - **game** — Unity/Unreal/Godot/IL2CPP markers, game-style manifest, game assets, score/level patterns
   - **non_game** — utilities, finance, social, productivity, tools, etc.
5. Extract identifiers and structure:
   - package / applicationId / app label
   - manifest: activities, services, receivers, providers, permissions, queries
   - min/target SDK, Gradle/AGP clues
   - native `.so` names
   - assets, databases, shared prefs filenames
   - networking hosts/paths **without** embedding tokens
   - third-party SDKs (Firebase, ads, analytics, billing)
   - UI/navigation clues
6. Audit **branding, styling, and copyright-sensitive material** (observation only):
   - launcher icons, in-app logos, splash screens
   - brand names, trademark strings, store listing text in resources
   - custom fonts, color palettes, themes, drawable styles
   - licensed audio/video references
   - Flag each as **owned | third_party | unknown** — never assume reuse rights
7. Note what compilation/decompilation destroyed (IL2CPP, Hermes bytecode, obfuscated names, missing resources).
8. Choose **Stage 2 template** (strict — only two options):
   - **game** → `unity_app_template`
   - **everything else** → `kotlin_app_template` (even if the original app was Flutter, React Native, Java, etc.)
9. Suggest a **project folder name** (e.g. `expense-tracker`, `calculator`, `2d-game`) from app label or user intent. Use kebab-case; the user may override in Stage 2.

## Outputs

Create `analysis/` and write:

- `analysis/analysis-report.md`
- `analysis/technology-detection.json`
- `analysis/reconstruction-plan.md`

### `technology-detection.json`

```json
{
  "detected_technology": "kotlin|java|flutter|react_native|unity|unreal|godot|ionic|capacitor|cordova|maui|unknown",
  "confidence": "high|medium|low",
  "app_category": "game|non_game",
  "is_game": false,
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
  "branding_and_copyright": {
    "icons": [],
    "logos": [],
    "fonts": [],
    "themes_and_colors": [],
    "audio_video": [],
    "third_party_brands": [],
    "ownership_notes": []
  },
  "lost_in_decompilation": [],
  "confirmed": [],
  "inferred": [],
  "recommended_template": "kotlin_app_template|unity_app_template",
  "suggested_project_folder": "my-app-name",
  "ownership_assumption": "user_owns|unconfirmed"
}
```

### `analysis-report.md`

Use headings: Executive summary, Technology, App category (game vs non-game), Identifiers, Manifest/components, Resources/assets, Branding & copyright observations, Styling/themes, Native libs, SDKs, Networking (redacted), UI/navigation, Confirmed vs inferred, Lost information, Stage 2 recommendation.

### `reconstruction-plan.md`

- **Template to copy:** `kotlin_app_template` or `unity_app_template` and why
- **Suggested project folder name**
- Screens/scenes/routes to recreate
- Data/storage layers
- Network mocks vs real owned APIs
- Assets the user **may** reuse only if they own them (with copyright notes)
- Explicit non-goals (no keystore copy, no third-party binaries, no DRM bypass)

## Next stage

After writing the three files, Stage 2 is [project-create](../project-create/SKILL.md).
