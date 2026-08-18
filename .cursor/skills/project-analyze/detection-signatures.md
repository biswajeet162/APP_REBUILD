# Technology detection signatures

Inspect `project/` (apktool, jadx, engine extract, or mixed). One strong hit can be enough for **high** confidence; weak/mixed hits stay **medium/low**.

## Stage 2 template rule (strict)

Only two templates exist. Stage 1 must set `recommended_template` using **app category**, not the original engine alone:

| Condition | `recommended_template` |
|-----------|------------------------|
| `app_category` = **game** | `unity_app_template` |
| `app_category` = **non_game** | `kotlin_app_template` |

Record the **original** technology in `detected_technology` for the report. Stage 2 always copies Kotlin or Unity as above.

---

## Game detection — sets `app_category`: **game**

High-confidence markers (any engine):

- `libunity.so`, `libil2cpp.so`, `libmain.so` (Unity IL2CPP)
- `assets/bin/Data/`, `global-metadata.dat`, `data.unity3d`, `unity_builtin_extra`
- `libUE4.so`, `UnrealEngine`, `.pak` assets (Unreal)
- `libgodot.so`, `.pck` / `.gdc` game packs (Godot)
- Play Store category clues, game-style activities, score/level/progress strings
- Heavy OpenGL/Vulkan game rendering libs without a non-game UI shell

If multiple game engines appear, pick the **runtime** engine for `detected_technology` and still recommend `unity_app_template` for Stage 2.

---

## Non-game — sets `app_category`: **non_game** → `kotlin_app_template`

Utilities, finance, social, productivity, tools, browsers, etc. Even when the original stack was:

### Flutter (original tech only — Stage 2 still Kotlin)

- `libflutter.so`, `libapp.so`, `assets/flutter_assets/`
- `AssetManifest.json`, `FontManifest.json`

### React Native (original tech only — Stage 2 still Kotlin)

- `index.android.bundle`, `libreactnative.so`, `libhermes.so`
- Manifest activity extending React Activity

### Native Android (original tech — Stage 2 Kotlin)

- `AndroidManifest.xml` + `res/` + `smali/` or jadx `sources/`
- Kotlin (`kotlinx`, `.kt`) or Java patterns

### Ionic / Capacitor / Cordova (original tech — Stage 2 still Kotlin)

- `capacitor.config.json`, `cordova.js`, `www/` web assets

### .NET MAUI / Xamarin (original tech — Stage 2 still Kotlin)

- `libmonodroid.so`, `assemblies/*.dll`

---

## Branding, styling, and copyright observations

Document paths and descriptions; do **not** copy binary assets into `analysis/`.

| What to look for | Where |
|------------------|--------|
| Launcher / adaptive icons | `res/mipmap-*`, `res/drawable*` |
| In-app logos, splash | `res/drawable*`, `assets/` |
| App name & brand strings | `res/values/strings.xml`, manifest `android:label` |
| Color palettes & themes | `res/values/colors.xml`, `themes.xml`, `styles.xml` |
| Custom fonts | `assets/fonts/`, `res/font/` |
| Licensed media | `assets/audio/`, `assets/video/`, raw resources |
| Third-party marks | Google/Facebook/ad SDK branding in drawables or strings |

For each item, note: path, description, and **owned | third_party | unknown**.

---

## Conflicting signals

- Game + utility shell → prefer **game** if IL2CPP/Unity/Unreal/Godot runtime loads.
- Flutter/RN + native-only UI → record both; `recommended_template` still follows game vs non-game rule.
- Unknown with low confidence → finish Stage 1; default `recommended_template` to `kotlin_app_template` and mark confidence **low** in the report.
