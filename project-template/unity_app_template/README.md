# Unity App Template (read-only base)

**Do not build games in this folder.** This is the permanent starter copied into each new game project.

## How new Unity projects are created

1. You tell the agent the **game name** (e.g. `2d-racer`, `puzzle-game`).
2. Agent copies this folder to `{game-name}/` at the repo root.
3. All Stage 3 work happens in `{game-name}/` — never here.

```powershell
powershell -File .cursor/skills/project-create/scripts/copy-template.ps1 -Technology unity -ProjectName 2d-racer
```

## What this base includes

- `Assets/Scripts/Starter.cs` — on-screen “Unity App Template” message
- `Assets/Editor/BuildAndroid.cs` — batch Android APK build (SDK/NDK/JDK auto-detect)
- `Packages/manifest.json`, `ProjectSettings/` — Unity 6000.x compatible

## Android build (from a copied project, not this template folder)

From `{game-name}/` after copy:

```powershell
$env:GRADLE_USER_HOME = "D:\gradle"
$env:TEMP = "D:\tmp"
$env:TMP = "D:\tmp"
& "C:\Program Files\Unity\Hub\Editor\6000.5.8f1\Editor\Unity.exe" -batchmode -quit -nographics `
  -projectPath "D:\{game-name}" -buildTarget Android `
  -executeMethod BuildAndroid.BuildDebugApk `
  -logFile "D:\{game-name}\Builds\Android\unity-build.log"
```

Install to phone:

```powershell
adb install -r Builds\Android\unity-template-debug.apk
adb shell am start -n com.example.unity_app_template/com.unity3d.player.UnityPlayerGameActivity
```

## Requirements

- Unity Hub + Editor **6000.5.8f1** (or compatible) with **Android Build Support**
- Android SDK; NDK **27.2.12479018** (Unity 6000.5 requirement)
- Short paths recommended on Windows (`D:\gradle`, `D:\tmp` for Gradle cache)

**Troubleshooting:** see **[UNITY_ANDROID_RUNBOOK.md](UNITY_ANDROID_RUNBOOK.md)** — all issues, fixes, and tricks from first device deploy.

## Stage 3

In the **copied** project: replace `Starter.cs`, add scenes under `Assets/Scenes/`, build gameplay on top. Do not paste IL2CPP dumps from `project/`.
