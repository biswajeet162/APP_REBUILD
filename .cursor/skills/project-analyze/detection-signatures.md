# Technology detection signatures

Inspect `project/` as **any** source tree: app dump, repo, web app, backend, engine extract, or mixed. One strong hit can be enough for **high** confidence; weak/mixed hits stay **medium/low**.

Record **original** technology in `detected_technology`. Stage 2 always copies Kotlin or Unity from **app category**, not from the original engine.

## Stage 2 template rule (strict)

| Condition | `recommended_template` |
|-----------|------------------------|
| `app_category` = **game** | `unity_app_template` |
| `app_category` = **non_game** | `kotlin_app_template` |

---

## Game detection — `app_category`: **game**

High-confidence markers (any engine or repo layout):

- Unity: `libunity.so`, `libil2cpp.so`, `Assets/`, `ProjectSettings/`, `global-metadata.dat`, `data.unity3d`
- Unreal: `libUE4.so`, `UnrealEngine`, `.pak`, `.uproject`
- Godot: `libgodot.so`, `.pck` / `.gdc`, `project.godot`
- Gameplay loop: levels, score, lives, scenes as game worlds, heavy OpenGL/Vulkan without a utility UI shell

If multiple game engines appear, pick the **runtime** engine for `detected_technology` and still recommend `unity_app_template`.

---

## Non-game — `app_category`: **non_game** → `kotlin_app_template`

Utilities, finance, social, productivity, tools, browsers, dashboards, APIs, etc. Original stack is recorded only.

### Web (React / Vue / Next / plain)

- `package.json`, `next.config.*`, `vite.config.*`, `src/App.tsx`, `pages/`, `app/`
- HTML/CSS/JS SPA with routes rather than a game loop

### iOS (Swift / ObjC)

- `*.xcodeproj`, `*.xcworkspace`, `Podfile`, `Package.swift`, `Info.plist`
- `*.swift`, storyboards, SwiftUI views

### Flutter (original tech only — Stage 2 still Kotlin)

- `pubspec.yaml`, `lib/*.dart`, `libflutter.so`, `assets/flutter_assets/`

### React Native (original tech only — Stage 2 still Kotlin)

- `index.android.bundle`, `libreactnative.so`, `libhermes.so`, `app.json` + React Native scripts

### Native Android (original tech — Stage 2 Kotlin)

- `AndroidManifest.xml` + `res/` + `smali/` or jadx `sources/`
- Kotlin (`kotlinx`, `.kt`) or Java patterns

### Ionic / Capacitor / Cordova

- `capacitor.config.json`, `cordova.js`, `www/` web assets

### .NET MAUI / Xamarin

- `libmonodroid.so`, `assemblies/*.dll`, `*.csproj`

### Backend / other (Node, Python, Go, etc.)

- `package.json` with Express/Fastify, `pyproject.toml`, `go.mod`, Dockerfiles
- Treat as **non_game** unless it is clearly a game server paired with a game client already classified as game

---

## Architecture, models, network (any stack)

Use these clues to fill `rebuild-metadata.json`:

| Look for | Typical paths / names |
|----------|------------------------|
| Entry points | `main`, `index`, `App`, `Application`, `UnityPlayerActivity` |
| UI / screens | `pages/`, `screens/`, `views/`, `Activities`, `Scenes/` |
| Domain models | `models/`, `entities/`, `dto/`, `types/`, serializers, Room/CoreData |
| Network | `api/`, `services/`, Retrofit/Alamofire/axios/fetch, OpenAPI, `.proto` |
| Storage | SQLite, Room, Realm, SharedPreferences, localStorage, UserDefaults |
| Auth | token interceptors, OAuth, session cookies, Firebase Auth |
| Navigation | routers, NavGraph, UINavigationController, scene flow |

Prefer **CONFIRMED** shapes from types and call sites. Mark guessed fields **INFERRED**.

---

## Branding, styling, and copyright observations

Document paths and descriptions; do **not** copy binary assets into `analysis/`.

| What to look for | Where (examples) |
|------------------|------------------|
| Launcher / adaptive icons | `res/mipmap-*`, `public/favicon`, `Assets/Icons` |
| In-app logos, splash | `res/drawable*`, `assets/`, `public/` |
| App name & brand strings | `strings.xml`, `Info.plist`, `package.json` name |
| Color palettes & themes | `colors.xml`, `themes.xml`, CSS variables, Unity themes |
| Custom fonts | `assets/fonts/`, `res/font/`, `public/fonts/` |
| Licensed media | `assets/audio/`, `assets/video/` |
| Third-party marks | Google/Facebook/ad SDK branding |

For each item, note: path, description, and **owned | third_party | unknown**.

---

## Conflicting signals

- Game + utility shell → prefer **game** if a game runtime/loop is the product.
- Flutter/RN/web + native UI → record both; `recommended_template` still follows game vs non-game.
- Unknown with low confidence → finish Stage 1; default `recommended_template` to `kotlin_app_template` and mark confidence **low**.
