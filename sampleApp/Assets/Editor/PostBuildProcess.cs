#if UNITY_EDITOR_OSX
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;
#if UNITY_2017_1_OR_NEWER
using UnityEditor.iOS.Xcode.Extensions;
#endif

public class PostBuildProcess
{
#if UNITY_IOS

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
            proj.AddFrameworkToProject(target, "CoreFoundation.framework", true);

#region MoPubの設定
#if UNITY_2017_1_OR_NEWER
            var mopubFileGuid = proj.FindFileGuidByProjectPath("Frameworks/Plugins/iOS/sdk/MoPubSDKFramework.framework");
            proj.AddFileToEmbedFrameworks(target, mopubFileGuid);
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
        }
    }

#endif
}
#endif