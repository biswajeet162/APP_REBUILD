# Unity Android build runbook (Windows)

Reference for agents and developers. Documents every issue, workaround, and fix encountered while getting the Unity template building and running on a physical Android device.

**Environment tested:** Windows 11, Unity **6000.5.8f1**, Android SDK at `%LOCALAPPDATA%\Android\Sdk`, device connected via USB (developer mode).

**Related files:**

- `Assets/Editor/BuildAndroid.cs` — batch APK build + toolchain auto-config
- `Assets/Scripts/Starter.cs` — visible on-screen test message
- `README.md` — how to copy this template into a new game folder

---

## Quick success checklist

Before building, confirm:

| Check | Command / path |
|-------|----------------|
| Phone connected | `adb devices` → shows `device` |
| Unity Editor | `C:\Program Files\Unity\Hub\Editor\6000.5.8f1\Editor\Unity.exe` |
| Android Build Support | `...\Editor\Data\PlaybackEngines\AndroidPlayer\` exists |
| Android SDK | `%LOCALAPPDATA%\Android\Sdk` |
| NDK **27.2.12479018** | `%LOCALAPPDATA%\Android\Sdk\ndk\27.2.12479018` |
| JDK 17+ | e.g. `C:\Program Files\Java\jdk-17.0.12` |
| Short Gradle cache | `D:\gradle`, `D:\tmp` (see issue #5) |
| Short project path (optional) | Junction `D:\unity-app` → project folder |

**One-liner build** (from copied project, after junction if needed):

```powershell
$env:GRADLE_USER_HOME = "D:\gradle"
$env:TEMP = "D:\tmp"
$env:TMP = "D:\tmp"
$env:ANDROID_HOME = "$env:LOCALAPPDATA\Android\Sdk"
$env:ANDROID_NDK_HOME = "$env:LOCALAPPDATA\Android\Sdk\ndk\27.2.12479018"

& "C:\Program Files\Unity\Hub\Editor\6000.5.8f1\Editor\Unity.exe" `
  -batchmode -quit -nographics `
  -projectPath "D:\unity-app" `
  -buildTarget Android `
  -executeMethod BuildAndroid.BuildDebugApk `
  -logFile "D:\unity-app\Builds\Android\unity-build.log"
```

**Install + launch:**

```powershell
adb install -r Builds\Android\unity-template-debug.apk
adb shell am start -n com.example.unity_app_template/com.unity3d.player.UnityPlayerGameActivity
```

---

## Issue 1 — Android Build Support not installed

### Symptom

```
Native extension for Android target not found
Build fails immediately; no APK
```

Or folder missing:

```
C:\Program Files\Unity\Hub\Editor\<version>\Editor\Data\PlaybackEngines\AndroidPlayer
```

### Cause

Unity Editor was installed without **Android Build Support** module.

### Fix

**Unity Hub → Installs → gear on editor → Add modules:**

- Android Build Support
- Android SDK & NDK Tools
- OpenJDK

### Verify

```powershell
Test-Path "C:\Program Files\Unity\Hub\Editor\6000.5.8f1\Editor\Data\PlaybackEngines\AndroidPlayer"
# Should be True
```

### Note

Even after “Android Build Support” is installed, Unity’s **bundled** OpenJDK/SDK/NDK paths under `AndroidPlayer\OpenJDK`, `AndroidPlayer\SDK`, `AndroidPlayer\NDK` may still be **empty**. Unity then falls back to external paths (see issues #3–#4). That is normal on this setup.

---

## Issue 2 — Unity Hub CLI / module install from terminal

### Symptom

Default Unity Hub headless commands fail silently or show cache errors:

```
Unable to move the cache: Access is denied
```

### Workaround

Use a dedicated user-data directory for Hub CLI:

```powershell
& "C:\Program Files\Unity Hub\Unity Hub.exe" `
  --user-data-dir="D:\ZZ_APPS\APK UNLOCK\MY APPS\main\.unityhub-data" `
  -- --headless install-modules --version 6000.5.8f1 --module android
```

### Tip

If download is already in progress, Hub may say `attaching to existing in-flight download`. Wait and re-run until `AndroidPlayer` folder exists.

---

## Issue 3 — Android NDK not found

### Symptom

```
UnityException: Android NDK not found
Android NDK not found or invalid. Please, fix it in Edit / Unity -> Preferences -> External Tools
```

Log also shows:

```
Android NDK was not installed with Unity at ...\AndroidPlayer\NDK
```

### Cause

- Unity’s bundled NDK folder empty
- User SDK had NDK versions Unity **rejects** (e.g. `27.1.12297006`)

### Fix — install exact NDK version Unity requires

Unity **6000.5.8f1** requires **NDK r27c (64-bit)** = **`27.2.12479018`**

```powershell
$sdkmanager = "$env:LOCALAPPDATA\Android\Sdk\cmdline-tools\latest\bin\sdkmanager.bat"
echo y | & $sdkmanager "ndk;27.2.12479018"
```

Verify:

```powershell
Test-Path "$env:LOCALAPPDATA\Android\Sdk\ndk\27.2.12479018"
```

### Wrong NDK error example

```
Provided path "...\ndk\27.1.12297006" is not valid Android NDK path.
Unity requires NDK r27c (64-bit) (27.2.12479018).
```

**Do not** point Unity at random NDK versions from React Native or other tools unless they match Unity’s required version.

### Code fix (already in template)

`BuildAndroid.ConfigureAndroidToolchain()` sets:

- SDK → `%LOCALAPPDATA%\Android\Sdk`
- NDK → `...\ndk\27.2.12479018` (fallback: newest in `ndk\`)
- JDK → scans `Program Files\Java`, Android Studio JBR, Unity OpenJDK

---

## Issue 4 — JDK / SDK “not installed with Unity” warnings

### Symptom

Warnings during build (non-fatal if fallbacks work):

```
JDK was not installed with Unity at ...\AndroidPlayer\OpenJDK
Android SDK was not installed with Unity at ...\AndroidPlayer\SDK
```

Unity then uses `%LOCALAPPDATA%\Android\Sdk` automatically.

### Fix

Ensure system JDK 17+ exists:

```powershell
java -version
# java version "17.0.12" or newer
```

`BuildAndroid.cs` picks JDK from `C:\Program Files\Java\jdk-17.*`.

Optional: set manually in Unity → **Edit → Preferences → External Tools**.

---

## Issue 5 — Windows MAX_PATH (260 characters) — Gradle / CMake / Ninja

### Symptom A (React Native / long repo path)

```
ninja: error: ... Filename longer than 260 characters
```

### Symptom B (Unity IL2CPP Gradle step)

```
ninja: error: Stat(...cursor-sandbox-cache...\gradle\caches\...): Filename longer than 260 characters
```

Happens when:

- Project path is long (`D:\ZZ_APPS\APK UNLOCK\MY APPS\main\...`)
- Gradle cache lives under deep `%TEMP%\cursor-sandbox-cache\...`

### Fixes (use together on Windows)

**1. Short Gradle + temp dirs:**

```powershell
New-Item -ItemType Directory -Path "D:\gradle","D:\tmp" -Force
$env:GRADLE_USER_HOME = "D:\gradle"
$env:TEMP = "D:\tmp"
$env:TMP = "D:\tmp"
```

Set these **before** starting Unity batch build.

**2. Short project path via junction:**

```powershell
New-Item -ItemType Junction -Path "D:\unity-app" `
  -Target "D:\ZZ_APPS\APK UNLOCK\MY APPS\main\project-template\unity_app_template"
```

Build with `-projectPath D:\unity-app`.

**3. Do not use `subst R:` for Unity** — can cause “different roots” errors between `R:\` and `D:\` paths in Gradle/codegen.

**4. Long-term:** enable Windows long paths (requires admin + reboot) or keep projects under `D:\games\my-game`.

---

## Issue 6 — Unity batchmode path with spaces

### Symptom

```
Couldn't set project path to: D:/ZZ_APPS/APK UNLOCK/MY APPS/main/d:/ZZ_APPS/APK
```

### Cause

`Start-Process` argument quoting + spaces in `APK UNLOCK\MY APPS` broke `-projectPath`.

### Fix

Use junction short path `D:\unity-app` or quote carefully:

```powershell
Start-Process -FilePath $unity -ArgumentList `
  "-batchmode -quit -nographics -projectPath `"D:\unity-app`" -buildTarget Android -executeMethod BuildAndroid.BuildDebugApk -logFile `"D:\unity-app\Builds\Android\unity-build.log`""
```

---

## Issue 7 — Unity exits in ~5 seconds but build continues in log

### Symptom

Shell returns quickly; APK not ready yet OR log still growing.

### Fix

Use `Start-Process -Wait` and check log tail:

```powershell
$p = Start-Process -FilePath $unity -ArgumentList "..." -PassThru -Wait -NoNewWindow
Write-Output "UNITY_EXIT=$($p.ExitCode)"
Test-Path "D:\unity-app\Builds\Android\unity-template-debug.apk"
```

First IL2CPP Android build can take **3–15 minutes**. Success = exit code **0** + APK file exists (~20–25 MB).

Log file: `Builds/Android/unity-build.log`

---

## Issue 8 — Wrong launcher activity (Unity 6)

### Symptom

```
Error: Activity class {com.example.unity_app_template/com.unity3d.player.UnityPlayerActivity} does not exist.
```

Install succeeds; launch fails.

### Cause

Unity **6** uses `UnityPlayerGameActivity`, not `UnityPlayerActivity`.

### Fix

```powershell
adb shell am start -n com.example.unity_app_template/com.unity3d.player.UnityPlayerGameActivity
```

Find launcher if package changes:

```powershell
adb shell cmd package resolve-activity --brief com.example.unity_app_template
```

---

## Issue 9 — SDK platform-tools version warnings

### Symptom

```
Detected outdated SDK Platform Tools version 35.0.2 when the min version is 36.0.0
This version only understands SDK XML versions up to 3 but an SDK XML file of version 4...
Observed package id 'platform-tools' in inconsistent location '...\platform-tools.backup'
```

### Impact

Usually **warnings only**; build succeeded after NDK + Gradle path fixes.

### Optional cleanup

- Remove duplicate `platform-tools.backup` if safe
- Update platform-tools via Android Studio SDK Manager or `sdkmanager "platform-tools"`

---

## Issue 10 — Build log location and reading failures

### Log paths

| Build | Log |
|-------|-----|
| Batch build | `{project}/Builds/Android/unity-build.log` |
| Unity Editor GUI | `%LOCALAPPDATA%\Unity\Editor\Editor.log` |

### Search for failure

```powershell
Select-String -Path "Builds\Android\unity-build.log" -Pattern "error|failed|Exception|Build Finished"
```

Common final lines:

- Success: `Build Finished, Result: Succeeded` + `Android APK built at: ...`
- Failure: `Build Finished, Result: Failure` + `executeMethod ... threw exception`

---

## Issue 11 — Template vs game project (workflow)

### Rule

**Never develop a specific game inside `project-template/unity_app_template/`.**

Always:

1. Ask user for game name
2. Copy template → `{game-name}/` at repo root
3. Build/run from `{game-name}/`

```powershell
powershell -File .cursor/skills/project-create/scripts/copy-template.ps1 -Technology unity -ProjectName my-game
```

Copy script excludes: `Library`, `Temp`, `Logs`, `Builds`, `UserSettings` (each copy starts clean).

---

## Issue 12 — adb / device connection

### Verify

```powershell
adb devices
# L7ZTR8L7SGPNWS9D    device
```

### If unauthorized

Enable USB debugging on phone; accept RSA prompt.

### React Native note (same machine)

If Metro runs on 8081, Unity is unaffected. For RN only: `adb reverse tcp:8081 tcp:8081`.

---

## Package / identifiers (template defaults)

| Setting | Value |
|---------|--------|
| applicationId | `com.example.unity_app_template` |
| productName | `Unity App Template` |
| APK output | `Builds/Android/unity-template-debug.apk` |
| Launcher activity | `com.unity3d.player.UnityPlayerGameActivity` |

Change in `BuildAndroid.cs` / Unity **Player Settings** when shipping a real game (use owned package name).

---

## Troubleshooting decision tree

```
Build fails?
├─ AndroidPlayer folder missing → Install Android Build Support (Issue #1)
├─ NDK not found → Install ndk;27.2.12479018 (Issue #3)
├─ Filename longer than 260 characters → D:\gradle + D:\tmp + D:\unity-app (Issue #5)
├─ Path / spaces error → Use junction D:\unity-app (Issue #6)
├─ Gradle failed in IL2CPP step → Check unity-build.log; apply Issue #5
└─ APK exists but won't open → Use UnityPlayerGameActivity (Issue #8)
```

---

## Version pin reference (this repo)

| Component | Version |
|-----------|---------|
| Unity Editor | 6000.5.8f1 |
| Required NDK | 27.2.12479018 (r27c) |
| Gradle (Unity bundled) | 9.1.0 |
| Android Gradle Plugin (Unity) | 9.0.0 |
| JDK | 17+ |
| Template Editor version file | `ProjectSettings/ProjectVersion.txt` → 2022.3.21f1 (Unity upgrades on open) |

When upgrading Unity Editor, re-check NDK requirements in External Tools or first batch build log.

---

## Changelog

| Date | Notes |
|------|--------|
| 2026-08-18 | Initial runbook after first successful device deploy. Fixed NDK version, Gradle path length, junction path, GameActivity launch. |
