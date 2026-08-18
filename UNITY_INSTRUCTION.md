# Unity instruction

How to create a Unity game project from the template, build it as an Android **APK** (share with friends), and export an **AAB** for Google Play.

**Base template (do not edit for a specific game):** `project-template/unity_app_template/`

**Example game folder:** `name-show/`

**More troubleshooting:** [`project-template/unity_app_template/UNITY_ANDROID_RUNBOOK.md`](project-template/unity_app_template/UNITY_ANDROID_RUNBOOK.md)

---

## 1. Create a new Unity game folder

Never build inside `project-template/`. Copy the template into a new folder at the repo root.

1. Choose a game name (example: `name-show`, `2d-racer`).
2. From the **main** folder, run:

```powershell
cd "D:\ZZ_APPS\APK UNLOCK\MY APPS\main"

powershell -File .cursor/skills/project-create/scripts/copy-template.ps1 -Technology unity -ProjectName name-show
```

This copies `project-template/unity_app_template` → `name-show/`.

Work only in `{game-name}/` after that.

---

## 2. One-time setup (Windows)

You need:

- Unity Hub + Editor **6000.5.8f1**
- **Android Build Support** module (Unity Hub → Installs → Add modules)
- Android SDK: `%LOCALAPPDATA%\Android\Sdk`
- NDK **27.2.12479018**
- JDK 17+
- Phone with USB debugging (for install)

Install the required NDK if missing:

```powershell
& "$env:LOCALAPPDATA\Android\Sdk\cmdline-tools\latest\bin\sdkmanager.bat" "ndk;27.2.12479018"
```

Create short cache folders (avoids Windows 260-character path errors):

```powershell
New-Item -ItemType Directory -Path "D:\gradle","D:\tmp" -Force
```

Optional but recommended — short path to the game:

```powershell
New-Item -ItemType Junction -Path "D:\name-show" -Target "D:\ZZ_APPS\APK UNLOCK\MY APPS\main\name-show"
```

---

## 3. Set variables before every build

Open PowerShell and run:

```powershell
$UNITY = "C:\Program Files\Unity\Hub\Editor\6000.5.8f1\Editor\Unity.exe"
$PROJECT = "D:\name-show"
$LOG = "$PROJECT\Builds\Android\unity-build.log"

$env:GRADLE_USER_HOME = "D:\gradle"
$env:TEMP = "D:\tmp"
$env:TMP = "D:\tmp"
$env:ANDROID_HOME = "$env:LOCALAPPDATA\Android\Sdk"
$env:ANDROID_NDK_HOME = "$env:LOCALAPPDATA\Android\Sdk\ndk\27.2.12479018"
```

Change `$PROJECT` if your game folder name is different.

---

## 4. Build a DEBUG APK (for your phone / share with friends)

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

**Success:** exit code `0` and this file exists:

```
{game-folder}\Builds\Android\unity-template-debug.apk
```

First build can take several minutes.

---

## 5. Install the APK on your phone

Connect the phone (USB debugging on), then:

```powershell
adb devices
adb install -r "$PROJECT\Builds\Android\unity-template-debug.apk"
```

Launch **Name Show**:

```powershell
adb shell am start -n com.example.name_show/com.unity3d.player.UnityPlayerGameActivity
```

If the activity is unknown:

```powershell
adb shell cmd package resolve-activity --brief com.example.name_show
```

---

## 6. Share the APK with friends

1. Open the output folder:

```powershell
explorer "$PROJECT\Builds\Android"
```

2. Send `unity-template-debug.apk` (WhatsApp, Drive, USB, etc.).
3. On their phone: open the APK → allow **Install unknown apps** → Install.

This is a **debug** APK. Fine for friends and testing. For Play Store, use the AAB steps below.

---

## 7. Build a RELEASE APK (signed)

### 7a. Create a keystore (once — keep it safe)

```powershell
keytool -genkeypair -v -keystore D:\my-release-key.jks -keyalg RSA -keysize 2048 -validity 10000 -alias my-key-alias
```

Do **not** commit the `.jks` file or passwords. If you lose the keystore, you cannot update the same Play Store app.

### 7b. Set signing variables

```powershell
$env:ANDROID_KEYSTORE_PATH = "D:\my-release-key.jks"
$env:ANDROID_KEYSTORE_PASS = "your-keystore-password"
$env:ANDROID_KEY_ALIAS = "my-key-alias"
$env:ANDROID_KEY_ALIAS_PASS = "your-key-password"
```

### 7c. Build

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

Output:

```
{game-folder}\Builds\Android\app-release.apk
```

---

## 8. Build AAB for Google Play Store

Play Console wants an **.aab**, not an APK, for new apps.

Use the same keystore variables as step 7b, then:

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

Output:

```
{game-folder}\Builds\Android\app-release.aab
```

Then:

1. Open [Google Play Console](https://play.google.com/console)
2. Create the app (listing, content rating, privacy policy)
3. Create a release → upload `app-release.aab`
4. Complete Data safety and submit

Before Play Store upload, change `applicationId` from `com.example.*` to your own package name in `Assets/Editor/BuildAndroid.cs`.

---

## 9. Command cheat sheet

| Goal | Method |
|------|--------|
| Debug APK (test / friends) | `BuildAndroid.BuildDebugApk` |
| Release APK | `BuildAndroid.BuildReleaseApk` |
| Play Store AAB | `BuildAndroid.BuildAppBundle` |

Full one-liner:

```powershell
& $UNITY -batchmode -quit -nographics -projectPath $PROJECT -buildTarget Android -executeMethod BuildAndroid.BuildDebugApk -logFile $LOG
```

If the build fails:

```powershell
Select-String -Path $LOG -Pattern "error|Error|Exception|Build Finished" | Select-Object -Last 30
```

---

## 10. Unity Editor (no command line)

1. Open **Unity Hub** → add `{game-name}/` (not `project-template/`).
2. **File → Build Profiles** (or **Build Settings**).
3. Select **Android**.
4. **Build** = APK. **Build App Bundle** = AAB for Play.
5. For release signing: **Edit → Project Settings → Player → Android → Publishing Settings**.

---

## Files produced

```
{game-name}/
  Builds/Android/
    unity-template-debug.apk
    app-release.apk
    app-release.aab
    unity-build.log
```

---

## Build APK and send on WhatsApp

For `name-show`. Change the path if your game folder is different.

**1. Set paths (PowerShell)**

```powershell
$UNITY = "C:\Program Files\Unity\Hub\Editor\6000.5.8f1\Editor\Unity.exe"
$PROJECT = "D:\name-show"
$env:GRADLE_USER_HOME = "D:\gradle"
$env:TEMP = "D:\tmp"
$env:TMP = "D:\tmp"
```

**2. Build the APK**

```powershell
Start-Process $UNITY -ArgumentList "-batchmode -quit -nographics -projectPath $PROJECT -buildTarget Android -executeMethod BuildAndroid.BuildDebugApk -logFile $PROJECT\Builds\Android\unity-build.log" -Wait -NoNewWindow
```

**3. Open the APK folder**

```powershell
explorer "$PROJECT\Builds\Android"
```

**4. Send on WhatsApp**

Send this file to your friend:

`unity-template-debug.apk`

On their phone: open the file → allow **Install unknown apps** → Install → open **Name Show**.

