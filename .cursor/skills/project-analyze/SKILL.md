---
name: project-analyze
description: >-
  Stage 1 — always run first. Analyzes whatever is in project/ regardless of
  technology (web, iOS, Android, Flutter, Unity, backend, desktop, or mixed).
  Extracts product purpose, architecture, domain models, network calls, flows,
  and styling into analysis/ metadata. Recommends kotlin_app_template or
  unity_app_template for Stage 2. Use when the user pastes a project, asks to
  analyze, or starts the rebuild pipeline.
---

# PROJECT_ANALYZE (Stage 1)

**Always execute this skill first.** Input is `project/` unless the user gives another path.

The source is **any technology**. Do **not** assume Android, Kotlin, or Unity. Detect what is actually there, then produce a technology-agnostic **rebuild picture** that Stage 2 applies onto Kotlin Android or Unity.

Act as a senior software-architecture analyst.

## Rules

- Separate **CONFIRMED** facts from **INFERRED** facts. Never present decompiled or minified output as original source.
- Do not fabricate missing classes, names, comments, or architecture.
- Do not copy or echo secrets, keystores, certificates, API tokens, or signing material into reports (redact values).
- If ownership is not confirmed, analyze only; do not plan verbatim source recovery of third-party code.
- Record third-party **brands, logos, icons, fonts, and styling** as copyright/IP observations — do **not** copy them into reports as reusable assets unless the user confirms ownership.

## What to understand (the picture)

Build a complete working model of the product, not a file inventory:

1. **What it is** — product purpose, users, core value
2. **How it is structured** — architecture, modules, layers, entry points
3. **What data it uses** — domain models, entities, DTOs, schemas, relationships
4. **How it talks to the world** — network/API calls, hosts, methods, payloads, auth
5. **How users move** — screens/pages/scenes, navigation, key flows
6. **How state lives** — local storage, caches, databases, sessions
7. **How it looks** — branding, theme, typography, layout patterns (observation only)

Original stack is recorded for context. Target stack is always **Kotlin Android** (non-game) or **Unity** (game).

## Workflow

1. Confirm `project/` has files. If empty, stop and ask the user to paste the source project.
2. Inventory top-level layout. Look for **any** of: `package.json`, `pubspec.yaml`, `Podfile`, `*.xcodeproj`, `Cargo.toml`, `go.mod`, `pyproject.toml`, `AndroidManifest.xml`, `apktool.yml`, Gradle files, `Assets/` (Unity), `ProjectSettings/`, `smali/`, `sources/`, `lib/`, Dockerfiles, etc. See [detection-signatures.md](detection-signatures.md).
3. Detect **original runtime / source technology**. Record evidence paths. Confidence: high / medium / low.
4. Classify **app category** (this chooses Stage 2, not the original engine):
   - **game** — game engines, gameplay loop, levels/score, game assets
   - **non_game** — utilities, finance, social, productivity, tools, web apps, backends, etc.
5. Map **architecture**: entry points, modules/packages, layering (UI / domain / data / network), patterns (MVC, MVVM, clean, monolith, etc.).
6. Extract **domain models**: names, fields, types, relationships, source paths. Prefer schemas, types, entities, serializers, Room/Realm/CoreData, API DTOs.
7. Extract **network**: base URLs (no tokens), endpoints, HTTP methods, request/response shapes, auth scheme, retries/offline. Redact secrets.
8. Extract **flows**: named user/system journeys with ordered steps and the screens they touch.
9. Extract identifiers and structure when present (package / bundle id / app label / version). For non-Android sources, record the closest equivalent.
10. Audit branding, styling, and copyright-sensitive material (observation only). Flag each as **owned | third_party | unknown**.
11. Note what compilation, minification, or missing files destroyed.
12. Choose **Stage 2 template** (strict — only two options):
    - **game** → `unity_app_template`
    - **everything else** → `kotlin_app_template` (even if the original was Flutter, React, iOS, Node, Python, Java, etc.)
13. Suggest a **project folder name** in kebab-case from product name or user intent. The user may override in Stage 2.

## Outputs

Create `analysis/` and write all four files. `rebuild-metadata.json` is the contract Stage 2 **must** apply.

- `analysis/analysis-report.md`
- `analysis/technology-detection.json`
- `analysis/rebuild-metadata.json`
- `analysis/reconstruction-plan.md`

### `technology-detection.json`

```json
{
  "detected_technology": "kotlin|java|flutter|react_native|react|vue|next|ios_swift|unity|unreal|godot|ionic|capacitor|cordova|maui|node|python|unknown",
  "product_type": "mobile_app|web_app|game|backend|desktop|mixed|unknown",
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

Use empty arrays / nulls when a field does not apply to the source (e.g. no Android manifest on a web app).

### `rebuild-metadata.json` (Stage 2 contract)

This is the **picture**. Stage 2 copies Kotlin or Unity, then applies this file.

```json
{
  "summary": "One paragraph: what the product does and how it works.",
  "source_technology": "",
  "app_category": "game|non_game",
  "recommended_template": "kotlin_app_template|unity_app_template",
  "suggested_project_folder": "my-app-name",
  "display_name": "",
  "application_id_or_bundle": "",
  "architecture": {
    "pattern": "",
    "entry_points": [],
    "modules": [],
    "layers": [],
    "notes": []
  },
  "domain_models": [
    {
      "name": "",
      "fields": [{ "name": "", "type": "", "required": true }],
      "relationships": [],
      "source_paths": []
    }
  ],
  "network": {
    "base_urls_redacted": [],
    "auth": { "scheme": "none|bearer|basic|session|oauth|custom", "notes": "" },
    "endpoints": [
      {
        "method": "GET",
        "path": "/example",
        "purpose": "",
        "request_shape": {},
        "response_shape": {},
        "used_by_flows": []
      }
    ]
  },
  "storage": {
    "local": [],
    "remote": []
  },
  "flows": [
    {
      "name": "",
      "trigger": "",
      "steps": [],
      "screens": []
    }
  ],
  "ui": {
    "screens": [
      {
        "name": "",
        "purpose": "",
        "key_actions": [],
        "models_used": []
      }
    ],
    "navigation": "",
    "theme": {
      "colors": [],
      "typography": [],
      "layout_patterns": []
    }
  },
  "target_scaffold": {
    "kotlin": {
      "package_hint": "com.example.app",
      "architecture_hint": "MVVM + repository + Retrofit stubs",
      "packages_to_create": ["ui", "data", "domain", "network"]
    },
    "unity": {
      "product_name": "",
      "scenes": [],
      "script_folders": ["Assets/Scripts/Game", "Assets/Scripts/UI"]
    }
  },
  "confirmed": [],
  "inferred": []
}
```

Fill only what evidence supports. Empty lists are fine. Do not invent endpoints or models.

### `analysis-report.md`

Use headings: Executive summary, Original technology, What the product does, Architecture, Domain models, Network/API (redacted), User/system flows, UI/navigation, Storage, App category (game vs non-game), Identifiers, Branding & copyright observations, Styling/themes, Confirmed vs inferred, Lost information, Stage 2 recommendation.

### `reconstruction-plan.md`

- **Template to copy:** `kotlin_app_template` or `unity_app_template` and why
- **Suggested project folder name**
- How to map source architecture onto Kotlin MVVM or Unity scenes/scripts
- Screens/scenes/routes to recreate
- Domain models to stub in Stage 2
- Network mocks vs real owned APIs
- Assets the user **may** reuse only if they own them (with copyright notes)
- Explicit non-goals (no keystore copy, no third-party binaries, no DRM bypass)

## Next stage

After writing the four files, Stage 2 is [project-create](../project-create/SKILL.md). Create **applies** `rebuild-metadata.json`; it does not ignore it.
