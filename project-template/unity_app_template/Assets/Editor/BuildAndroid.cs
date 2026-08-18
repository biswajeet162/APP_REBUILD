using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class BuildAndroid
{
    public static void BuildDebugApk()
    {
        const string scenePath = "Assets/Scenes/SampleScene.unity";
        EnsureScene(scenePath);

        PlayerSettings.companyName = "Example";
        PlayerSettings.productName = "Unity App Template";
        PlayerSettings.SetApplicationIdentifier(
            BuildTargetGroup.Android,
            "com.example.unity_app_template");

        ConfigureAndroidToolchain();

        var outputDir = "Builds/Android";
        Directory.CreateDirectory(outputDir);
        var apkPath = Path.Combine(outputDir, "unity-template-debug.apk");

        EditorUserBuildSettings.development = true;
        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
        EditorUserBuildSettings.buildAppBundle = false;

        var options = new BuildPlayerOptions
        {
            scenes = new[] { scenePath },
            locationPathName = apkPath,
            target = BuildTarget.Android,
            options = BuildOptions.Development
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result != BuildResult.Succeeded)
        {
            throw new Exception("Android build failed: " + report.summary.result);
        }

        Debug.Log("Android APK built at: " + apkPath);
    }

    private static void EnsureScene(string scenePath)
    {
        if (File.Exists(scenePath))
        {
            return;
        }

        Directory.CreateDirectory(Path.GetDirectoryName(scenePath) ?? "Assets/Scenes");
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        var go = new GameObject("Starter");
        go.AddComponent<Starter>();

        EditorSceneManager.SaveScene(scene, scenePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        SceneManager.SetActiveScene(scene);
    }

    private static void ConfigureAndroidToolchain()
    {
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var sdkRoot = Path.Combine(localAppData, "Android", "Sdk");

        if (Directory.Exists(sdkRoot))
        {
            UnityEditor.Android.AndroidExternalToolsSettings.sdkRootPath = sdkRoot;
        }

        var ndkRoot = Path.Combine(sdkRoot, "ndk", "27.2.12479018");
        if (!Directory.Exists(ndkRoot))
        {
            ndkRoot = Directory.Exists(Path.Combine(sdkRoot, "ndk"))
                ? Directory.GetDirectories(Path.Combine(sdkRoot, "ndk"))
                    .OrderByDescending(Directory.GetCreationTimeUtc)
                    .FirstOrDefault()
                : null;
        }

        if (!string.IsNullOrEmpty(ndkRoot) && Directory.Exists(ndkRoot))
        {
            UnityEditor.Android.AndroidExternalToolsSettings.ndkRootPath = ndkRoot;
        }

        var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        var javaCandidates = new[]
        {
            Path.Combine(programFiles, "Java"),
            Path.Combine(programFiles, "Android", "Android Studio", "jbr"),
            Path.Combine(programFiles, "Unity", "Hub", "Editor", "6000.5.8f1", "Editor", "Data", "PlaybackEngines", "AndroidPlayer", "OpenJDK"),
        };

        foreach (var candidate in javaCandidates)
        {
            if (!Directory.Exists(candidate))
            {
                continue;
            }

            if (File.Exists(Path.Combine(candidate, "bin", "java.exe")))
            {
                UnityEditor.Android.AndroidExternalToolsSettings.jdkRootPath = candidate;
                break;
            }

            var nestedJdk = Directory.GetDirectories(candidate)
                .FirstOrDefault(d => File.Exists(Path.Combine(d, "bin", "java.exe")));

            if (!string.IsNullOrEmpty(nestedJdk))
            {
                UnityEditor.Android.AndroidExternalToolsSettings.jdkRootPath = nestedJdk;
                break;
            }
        }

        Debug.Log(
            "Android toolchain: SDK=" + UnityEditor.Android.AndroidExternalToolsSettings.sdkRootPath +
            " NDK=" + UnityEditor.Android.AndroidExternalToolsSettings.ndkRootPath +
            " JDK=" + UnityEditor.Android.AndroidExternalToolsSettings.jdkRootPath);
    }
}
