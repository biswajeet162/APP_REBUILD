# kotlin_app_template

Pure Kotlin Android starter for the rebuild pipeline (Stage 2). Uses Jetpack Compose and Material 3.

## Prerequisites

- Android SDK (set `sdk.dir` in `local.properties`)
- JDK 11+

Example `local.properties`:

```properties
sdk.dir=C\:\\Users\\YOUR_USERNAME\\AppData\\Local\\Android\\Sdk
```

## Build and run

From this directory:

```powershell
.\gradlew.bat assembleDebug
.\gradlew.bat installDebug
```

Or open the project in Android Studio and run on a device/emulator.

## Project layout

```
app/src/main/kotlin/com/example/kotlin_app_template/
  MainActivity.kt          # Compose entry screen
  ui/theme/                # Material theme
app/src/main/res/          # Strings, drawables, themes
```

After Stage 1 analysis, Stage 2 copies this template to `new-project/` and adjusts `applicationId`, app label, and package as needed. Stage 3 reconstructs owned screens and behavior here.

## Customize

- `app/build.gradle.kts` — `applicationId`, SDK levels, dependencies
- `app/src/main/res/values/strings.xml` — display name and copy
- `app/src/main/kotlin/.../MainActivity.kt` — replace starter UI in Stage 3

Do not copy keystores or another publisher's signing material into this template.
