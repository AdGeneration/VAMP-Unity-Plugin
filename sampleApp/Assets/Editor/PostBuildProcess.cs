#if UNITY_EDITOR_OSX
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;
#if UNITY_2017_1_OR_NEWER
using UnityEditor.iOS.Xcode.Extensions;
#endif

#if UNITY_IOS
public class PostBuildProcess
{
    private static readonly string adMobAppId = "ca-app-pub-3940256099942544~3347511713";
    private static readonly string adMobAppIdKey = "GADApplicationIdentifier";

    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            var projPath = PBXProject.GetPBXProjectPath(path);
            var proj = new PBXProject();
            proj.ReadFromFile(projPath);
            var target = proj.TargetGuidByName(PBXProject.GetUnityTargetName());

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
            proj.AddFrameworkToProject(target, "CoreFoundation.framework", true);

#region MoPubの設定
#if UNITY_2017_1_OR_NEWER
            var mopubFileGuid = proj.FindFileGuidByProjectPath("Frameworks/Plugins/iOS/sdk/MoPubSDKFramework.framework");
            if (!string.IsNullOrEmpty(mopubFileGuid))
            {
                proj.AddFileToEmbedFrameworks(target, mopubFileGuid);
            }
#endif
            var maskedFiles = Directory.GetFiles(
               path, "*.prevent_unity_compilation", SearchOption.AllDirectories);
            foreach (var maskedFile in maskedFiles)
            {
                var unmaskedFile = maskedFile.Replace(".prevent_unity_compilation", "");
                File.Move(maskedFile, unmaskedFile);
            }
#endregion
            File.WriteAllText(projPath, proj.WriteToString());


            var plistPath = path + "/Info.plist";
            var plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));

            var rootDict = plist.root;
            rootDict.SetString(adMobAppIdKey, adMobAppId);

            File.WriteAllText(plistPath, plist.WriteToString());
        }
    }
}
#elif UNITY_ANDROID
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