/// <summary>
///
/// VAMP-Unity-Plugin
///
/// Created by AdGeneratioin.
/// Copyright 2018 Supership Inc. All rights reserved.
///
/// </summary>

#if UNITY_EDITOR_OSX 
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;

#if UNITY_IOS
using UnityEditor.iOS.Xcode;

public class PostBuildProcess
{
    // AdMob AppID
    private static readonly string plistKeyadMobAppId = "GADApplicationIdentifier";
    private static readonly string adMobAppId = "ca-app-pub-3940256099942544~3347511713";

    // ATT
    private static readonly string plistKeyTrackingUsageDescription = "NSUserTrackingUsageDescription";
    private static readonly string trackingUsageDescription = "App would like to access IDFA for tracking purpose";

    // SKAdNetwork
    private static readonly string plistKeySKAdNetworkItems = "SKAdNetworkItems";
    private static readonly string plistKeySKAdNetworkIdentifier = "SKAdNetworkIdentifier";
    private static readonly string plistKeySupershipSKAdNetworkIdentifier = "348L86ZLVX.skadnetwork";

    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            var projPath = PBXProject.GetPBXProjectPath(path);
            var proj = new PBXProject();
            proj.ReadFromFile(projPath);
#if UNITY_2019_4_OR_NEWER
            var target = proj.GetUnityFrameworkTargetGuid();
#else
            var target = proj.TargetGuidByName(PBXProject.GetUnityTargetName());
#endif

            // Other Linker Flagsに-ObjCを追加
            proj.AddBuildProperty(target, "OTHER_LDFLAGS", "-ObjC");

            // Linked Frameworks and Librariesにフレームワークを追加
            proj.AddFrameworkToProject(target, "WebKit.framework", false);
            proj.AddFrameworkToProject(target, "GLKit.framework", false);
            proj.AddFrameworkToProject(target, "MessageUI.framework", false);
            proj.AddFrameworkToProject(target, "ImageIO.framework", false);
            proj.AddFrameworkToProject(target, "libz.tbd", false);
            proj.AddFrameworkToProject(target, "libxml2.tbd", false);
            proj.AddFrameworkToProject(target, "libc++.tbd", false);
            proj.AddFrameworkToProject(target, "libsqlite3.tbd", false);
            proj.AddFrameworkToProject(target, "libresolv.9.tbd", false);
            proj.AddFrameworkToProject(target, "libbz2.tbd", false);
            proj.AddFrameworkToProject(target, "AVKit.framework", false);
            proj.AddFrameworkToProject(target, "CoreFoundation.framework", true);

            File.WriteAllText(projPath, proj.WriteToString());

            var plistPath = path + "/Info.plist";
            var plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));

            var rootDict = plist.root;
            rootDict.SetString(plistKeyadMobAppId, adMobAppId);
            rootDict.SetString(plistKeyTrackingUsageDescription, trackingUsageDescription);
            rootDict.CreateArray(plistKeySKAdNetworkItems).AddDict()
                .SetString(plistKeySKAdNetworkIdentifier, plistKeySupershipSKAdNetworkIdentifier);
            File.WriteAllText(plistPath, plist.WriteToString());
        }
    }
}

#elif UNITY_ANDROID && UNITY_2018_1_OR_NEWER

public class PostBuildProcess : UnityEditor.Android.IPostGenerateGradleAndroidProject
{
    public int callbackOrder
    {
        get
        {
            return 999;
        }
    }

    void UnityEditor.Android.IPostGenerateGradleAndroidProject.OnPostGenerateGradleAndroidProject(string path)
    {
        var gradlePropertiesFile = path + "/gradle.properties";

        if (File.Exists(gradlePropertiesFile))
        {
            File.Delete(gradlePropertiesFile);
        }

        using (var writer = File.CreateText(gradlePropertiesFile))
        {
            writer.WriteLine("org.gradle.jvmargs=-Xmx4096M");
            writer.WriteLine("android.useAndroidX=true");
            writer.WriteLine("android.enableJetifier=true");
            writer.Flush();
            writer.Close();
        }
    }
}
#endif
#endif