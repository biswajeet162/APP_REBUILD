# Narrow Puzzle

Standard **APK mod and rebuild project** for Arrow Puzzle v1.8.0.

This is **not** a normal Android Studio or Unity source project. The original game is a **Unity IL2CPP** build (C# compiled to native code). Full game source code cannot be recovered from the APK. This repo uses the standard workflow for Android APK modding:

**input APK → decode → patch → rebuild → sign → install**

## Project layout

```
narrow-puzzle/
├── config/project.json      # App metadata and paths
├── input/apk/               # Original APK files (source inputs)
├── workspace/decoded/base/  # apktool decoded APK (generated)
├── scripts/                 # Patch and build automation
├── output/                  # Signed APK output (generated)
├── build.gradle.kts         # Gradle tasks (standard entry point)
└── README.md
```

## Requirements

- Java (for apktool)
- Python 3
- Android SDK build-tools (`zipalign`, `apksigner`)
- apktool JAR at `../../tools/apktool.jar` (relative to this project)
- `adb` for device install

Configure SDK path in `gradle.properties`:

```properties
android.sdk.dir=C:\\Path\\To\\Android\\Sdk
```

## Commands

First-time setup (decode original APK):

```bash
./gradlew decodeApk
```

Build patched APK:

```bash
./gradlew build
```

Install on connected phone:

```bash
./gradlew installApk
```

PowerShell equivalents:

```powershell
powershell -ExecutionPolicy Bypass -File scripts/build.ps1
powershell -ExecutionPolicy Bypass -File scripts/install.ps1
```

## What gets patched

- App name → **Narrow Puzzle**
- Skip consent / ads / cross-promo blocking screens
- Offline-friendly network and SDK bootstrap
- Default arrow color → red
- Single installable APK (arm64 libs merged)

## Important limitation

You **cannot** convert this into a normal editable Unity/Kotlin game source tree from the APK alone. To change deep game logic, you would need the original Unity project from the developer.

This standard project structure is the correct and maintainable way to keep modifying and rebuilding the APK.

## Play Store planning

See [docs/PLAY_STORE_CHECKLIST.md](docs/PLAY_STORE_CHECKLIST.md) for a detailed pre-upload checklist (ownership, package name, signing, SDKs, privacy, and policy risks).
