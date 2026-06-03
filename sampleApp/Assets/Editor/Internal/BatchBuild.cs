using UnityEditor;
using UnityEngine;

public class BatchBuild
{
    [MenuItem("VAMP/Build Android Sample")]
    public static void BuildAndroid() {
        EditorUserBuildSettings.buildAppBundle = false;
        EditorUserBuildSettings.exportAsGoogleAndroidProject = false;

        PerformBuild("../temp/VAMPUnityPluginSample.apk", BuildTarget.Android);
    }

    [MenuItem("VAMP/Build iOS Sample")]
    public static void BuildiOS() {
        PerformBuild("../temp", BuildTarget.iOS);
    }

    [MenuItem("VAMP/Export VAMP Unity Package")]
    public static void ExportUnityPackage() {
        Debug.Log("Exporting unitypackage...");
        AssetDatabase.ExportPackage(
            new[] { "Assets/VAMP" },
            "vamp_for_unity.unitypackage",
            ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies
            );
    }

    private static void PerformBuild(string locationPathName, BuildTarget target) {
        Debug.Log($"Starting {target} batch build...");

        var scenes = new[] {
            "Assets/Scenes/Main.unity",
            "Assets/Scenes/AdSample.unity",
            "Assets/Scenes/Info.unity",
            "Assets/Scenes/AppOpenAdSample.unity"
        };

        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions {
            scenes = scenes,
            locationPathName = locationPathName,
            target = target,
            options = BuildOptions.Development | BuildOptions.AllowDebugging
        });

        if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded) {
            Debug.Log($"{target} build succeeded: {report.summary.totalSize} bytes");
        }
        else {
            Debug.LogError($"{target} build failed");
            EditorApplication.Exit(1);
        }
    }
}
