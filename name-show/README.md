# Name Show

Unity game project copied from `project-template/unity_app_template`.

## What it shows

1. **Start screen** — enter your name (English letters A–Z, spaces allowed)
2. **3D view** — your name as revolving world-space text
3. **Drag text** — move it anywhere
4. **Drag empty space** — orbit camera to view from different angles

## Project layout

```
Assets/Scripts/Starter.cs   # Creates 3D TextMesh at runtime
Assets/Editor/BuildAndroid.cs
```

## Build Android APK

See `UNITY_ANDROID_RUNBOOK.md` in this folder (copied from template).

```powershell
$env:GRADLE_USER_HOME = "D:\gradle"
$env:TEMP = "D:\tmp"
$env:TMP = "D:\tmp"
& "C:\Program Files\Unity\Hub\Editor\6000.5.8f1\Editor\Unity.exe" -batchmode -quit -nographics `
  -projectPath "D:\name-show" -buildTarget Android `
  -executeMethod BuildAndroid.BuildDebugApk `
  -logFile "D:\name-show\Builds\Android\unity-build.log"
```

## Install on phone

```powershell
adb install -r Builds\Android\unity-template-debug.apk
adb shell am start -n com.example.name_show/com.unity3d.player.UnityPlayerGameActivity
```

## Next steps (Stage 3)

Replace display text, add gameplay, scenes, and assets in this folder — not in `project-template/`.
