# Technology detection signatures

Inspect `project/` (apktool, jadx, Flutter/RN extract, or mixed). One strong hit can be enough for **high** confidence; weak/mixed hits stay **medium/low**.

## Flutter — `recommended_template`: `flutter_app_template`

High-confidence files:

- `libflutter.so`, `libapp.so`
- `assets/flutter_assets/`
- `kernel_blob.bin`, `isolate_snapshot_data`, `vm_snapshot_data`
- `AssetManifest.json`, `FontManifest.json` under flutter assets
- `NOTICES.Z` / `NOTICES` in flutter assets

Notes: release Flutter almost never yields recoverable Dart. Plan UI reconstruction from assets, strings, and observable structure — not Dart recovery.

## React Native — `recommended_template`: `react_native_app_template`

High-confidence files:

- `index.android.bundle` / `assets/index.android.bundle`
- `libreactnativejni.so`, `libhermes.so`, `libjsc.so`, `libreactnative.so`
- `AndroidManifest` activity extending React Activity
- Readable JS/TS with `react-native` imports (jadx sometimes extracts the bundle)

Notes: Hermes bytecode is often not human-readable. If only bytecode exists, reconstruct screens from native resources + inferred navigation, and mark JS as lost.

## Native Android (Kotlin/Java) — `recommended_template`: `scaffold_native`

- `AndroidManifest.xml` + `res/` + `smali/` or jadx `sources/`
- No Flutter/RN/engine libs
- Kotlin (`kotlinx`, `.kt`) vs Java from source/smali patterns

There is no native template in `project-template/` yet. Stage 2 must scaffold a Gradle Kotlin (or Java) app.

## Ionic / Capacitor / Cordova — `recommended_template`: `none` (CLI scaffold)

- `capacitor.config.json` / `capacitor.config.ts`
- `cordova.js`, `cordova_plugins.js`
- `www/` or `public/` with Ionic/Angular/Vue web assets
- `IonicModule` / `ion-` tags in HTML

## Unity — `recommended_template`: `none`

- `libil2cpp.so`, `libunity.so`
- `assets/bin/Data/`, `global-metadata.dat`, `data.unity3d`, `unity_builtin_extra`

Do not copy IL2CPP/game data into the new project. Flag as engine content that needs a real Unity project and licensed assets.

## Unreal — `recommended_template`: `none`

- `libUE4.so`, `UnrealEngine`, `.pak` assets

## Godot — `recommended_template`: `none`

- `libgodot.so`, `.pck` / `.zip` game pack

## .NET MAUI / Xamarin — `recommended_template`: `none`

- `libmonodroid.so`, `assemblies/*.dll`, `Xamarin.*`

## Conflicting signals

If both Flutter and RN markers exist, list both, pick the **runtime** engine (the `.so` that actually loads), and keep confidence **medium**.
