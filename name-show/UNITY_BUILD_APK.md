# Unity — build APK / AAB (export as Android app)

How to export a Unity project as an **APK** (install or share) or **AAB** (Google Play Store).

**Works with:** Unity **6000.5.8f1**, Windows, projects copied from `unity_app_template` (e.g. `name-show/`).

**Troubleshooting build errors:** see [UNITY_ANDROID_RUNBOOK.md](UNITY_ANDROID_RUNBOOK.md).

---

## What you get

| Output | Use case | Share with friends? | Play Store? |
|--------|----------|---------------------|-------------|
| **Debug APK** | Dev / testing on your phone | Yes (send the file) | No |
| **Release APK** | Signed installable app | Yes | Rarely (Play prefers AAB) |
| **Release AAB** | Google Play upload | No (upload to Console) | **Yes (required)** |

**Output folder (inside your game project):**

```
{your-game}/
  Builds/Android/
    unity-template-debug.apk    ← debug build
    app-release.apk             ← release APK (after Step 5)
    app-release.aab             ← Play Store bundle (after Step 6)
    unity-build.log             ← full build log
```

---

## Prerequisites (one-time)

1. **Unity Hub** with Editor **6000.5.8f1** + **Android Build Support** (SDK, NDK, OpenJDK modules).
2. **Android NDK** `27.2.12479018` installed:
   ```powershell
   sdkmanager "ndk;27.2.12479018"
   ```
3. **JDK 17+** installed.
4. Optional **short paths** on Windows (avoids build failures):
   ```powershell
   New-Item -ItemType Directory -Path "D:\gradle","D:\tmp" -Force
   New-Item -ItemType Junction -Path "D:\name-show" -Target "D:\path\to\your-game"
   ```

Replace `name-show` with your project folder name.

---

## Step 1 — Open PowerShell in the repo / set variables

```powershell
$UNITY = "C:\Program Files\Unity\Hub\Editor\6000.5.8f1\Editor\Unity.exe"
$PROJECT = "D:\name-show"   # junction or short path to your Unity game folder
$LOG = "$PROJECT\Builds\Android\unity-build.log"

$env:GRADLE_USER_HOME = "D:\gradle"
$env:TEMP = "D:\tmp"
$env:TMP = "D:\tmp"
$env:ANDROID_HOME = "$env:LOCALAPPDATA\Android\Sdk"
$env:ANDROID_NDK_HOME = "$env:LOCALAPPDATA\Android\Sdk\ndk\27.2.12479018"
```

---

## Step 2 — Build DEBUG APK (testing & sharing with friends)

Uses `BuildAndroid.BuildDebugApk` from `Assets/Editor/BuildAndroid.cs`.

```powershell
$p = Start-Process -FilePath $UNITY -ArgumentList @(
  "-batchmode","-quit","-nographics",
  "-projectPath", $PROJECT,
  "-buildTarget", "Android",
  "-executeMethod", "BuildAndroid.BuildDebugApk",
  "-logFile", $LOG
) -PassThru -Wait -NoNewWindow

Write-Host "Unity exit code:" $p.ExitCode
Test-Path "$PROJECT\Builds\Android\unity-template-debug.apk"
```

**First build:** 3–15 minutes. **Success:** exit code `0` and APK file exists (~20–30 MB).

---

## Step 3 — Install on your phone (USB)

```powershell
adb devices
adb install -r "$PROJECT\Builds\Android\unity-template-debug.apk"
```

**Launch** (change package if you changed `applicationId` in `BuildAndroid.cs`):

```powershell
adb shell am start -n com.example.name_show/com.unity3d.player.UnityPlayerGameActivity
```

Find launcher activity if unsure:

```powershell
adb shell cmd package resolve-activity --brief com.example.name_show
```

---

## Step 4 — Share APK with friends (not Play Store)

1. Copy the APK to phone storage, Google Drive, WhatsApp, etc.:
   ```powershell
   explorer "$PROJECT\Builds\Android"
   ```
2. Friend opens the APK on Android → allows **Install unknown apps** for that app (Chrome / Files).
3. Install and open.

**Note:** Debug APKs are fine for friends/testing. For wider distribution or Play Store, use release signing (Steps 5–7).

---

## Step 5 — Build RELEASE APK (signed installable app)

### 5a. Create a keystore (once — keep safe, never commit)

```powershell
keytool -genkeypair -v -keystore D:\my-release-key.jks -keyalg RSA -keysize 2048 -validity 10000 -alias my-key-alias
```

Store password, alias, and `.jks` file securely. **If you lose it, you cannot update the same Play Store app.**

### 5b. Set signing env vars (this session)

```powershell
$env:ANDROID_KEYSTORE_PATH = "D:\my-release-key.jks"
$env:ANDROID_KEYSTORE_PASS = "your-keystore-password"
$env:ANDROID_KEY_ALIAS     = "my-key-alias"
$env:ANDROID_KEY_ALIAS_PASS = "your-key-password"
```

### 5c. Build release APK

```powershell
$p = Start-Process -FilePath $UNITY -ArgumentList @(
  "-batchmode","-quit","-nographics",
  "-projectPath", $PROJECT,
  "-buildTarget", "Android",
  "-executeMethod", "BuildAndroid.BuildReleaseApk",
  "-logFile", $LOG
) -PassThru -Wait -NoNewWindow

Write-Host "Unity exit code:" $p.ExitCode
Test-Path "$PROJECT\Builds\Android\app-release.apk"
```

Output: `Builds/Android/app-release.apk` — share or sideload like Step 4.

If env vars are **not** set, Unity signs with the **debug key** (OK for local tests, **not** for Play Store).

---

## Step 6 — Build AAB for Google Play Store

Play Console requires **Android App Bundle (.aab)**, not APK, for new apps.

Same keystore env vars as Step 5b, then:

```powershell
$p = Start-Process -FilePath $UNITY -ArgumentList @(
  "-batchmode","-quit","-nographics",
  "-projectPath", $PROJECT,
  "-buildTarget", "Android",
  "-executeMethod", "BuildAndroid.BuildAppBundle",
  "-logFile", $LOG
) -PassThru -Wait -NoNewWindow

Write-Host "Unity exit code:" $p.ExitCode
Test-Path "$PROJECT\Builds\Android\app-release.aab"
```

Output: `Builds/Android/app-release.aab`

---

## Step 7 — Upload to Google Play Console

1. Go to [Google Play Console](https://play.google.com/console).
2. **Create app** → fill store listing, content rating, privacy policy.
3. **Release → Production** (or internal testing first).
4. **Create new release** → upload `app-release.aab`.
5. Complete **Data safety**, **target audience**, **app signing** (Play App Signing recommended).
6. Submit for review.

Before upload, in Unity / `BuildAndroid.cs` set:

- **Unique `applicationId`** (e.g. `com.yourname.nameshow`) — not `com.example.*`
- **Version code** / **version name** in Player Settings (increment each upload)
- **Target API level** meets [Play requirements](https://developer.android.com/google/play/requirements/target-sdk)

---

## Alternative — Unity Editor (GUI)

1. Open project in **Unity Hub** (open `{your-game}/`, not `project-template/`).
2. **File → Build Profiles** (or **Build Settings** on older layouts).
3. Select **Android** → switch platform if needed.
4. Choose:
   - **Build** → APK file
   - **Build App Bundle** → AAB for Play Store
5. For release: **Edit → Project Settings → Player → Android → Publishing Settings** → keystore.

Batch commands above do the same thing without opening the Editor UI.

---

## Quick reference — all execute methods

| Goal | Command |
|------|---------|
| Debug APK (dev) | `-executeMethod BuildAndroid.BuildDebugApk` |
| Release APK | `-executeMethod BuildAndroid.BuildReleaseApk` |
| Play Store AAB | `-executeMethod BuildAndroid.BuildAppBundle` |

Full template:

```powershell
& $UNITY -batchmode -quit -nographics `
  -projectPath $PROJECT `
  -buildTarget Android `
  -executeMethod BuildAndroid.BuildDebugApk `
  -logFile $LOG
```

---

## Check build log on failure

```powershell
Select-String -Path $LOG -Pattern "error|Error|Exception|Build Finished" | Select-Object -Last 30
```

Common fixes: [UNITY_ANDROID_RUNBOOK.md](UNITY_ANDROID_RUNBOOK.md) (NDK version, path length, font blocks, wrong launcher activity).

---

## Security reminders

- **Never commit** `.jks` keystore, passwords, or `google-services.json` from another publisher.
- **Never commit** `.unityhub-data/` or local signing secrets.
- Use your **own** `applicationId` and signing key for Play Store apps you publish.

---

## Example: full flow for `name-show`

```powershell
$UNITY = "C:\Program Files\Unity\Hub\Editor\6000.5.8f1\Editor\Unity.exe"
$PROJECT = "D:\name-show"
$env:GRADLE_USER_HOME = "D:\gradle"; $env:TEMP = "D:\tmp"; $env:TMP = "D:\tmp"

Start-Process $UNITY -ArgumentList "-batchmode -quit -nographics -projectPath $PROJECT -buildTarget Android -executeMethod BuildAndroid.BuildDebugApk -logFile $PROJECT\Builds\Android\unity-build.log" -Wait -NoNewWindow

adb install -r "$PROJECT\Builds\Android\unity-template-debug.apk"
adb shell am start -n com.example.name_show/com.unity3d.player.UnityPlayerGameActivity
```

Share with friends: send `unity-template-debug.apk` from `Builds\Android\`.

Play Store: create keystore → set env vars → `BuildAndroid.BuildAppBundle` → upload `app-release.aab`.
