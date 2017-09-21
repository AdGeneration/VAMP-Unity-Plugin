using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.Collections;
using System.IO;

public class PostBuildProcess
{
	#if UNITY_IPHONE
	[PostProcessBuild]
	public static void OnPostProcessBuild (BuildTarget buildTarget, string path)
	{
		if (buildTarget == BuildTarget.iOS) {
			string projPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";
			PBXProject proj = new PBXProject ();
			proj.ReadFromFile (projPath);

			string target = proj.TargetGuidByName ("Unity-iPhone");

			// Other Linker Flagsに-ObjCを追加
			proj.AddBuildProperty (target, "OTHER_LDFLAGS", "-ObjC");

			// Linked Frameworks and Librariesにフレームワークを追加
			proj.AddFrameworkToProject (target, "WebKit.framework", false);
			proj.AddFrameworkToProject (target, "GLKit.framework", false);
			proj.AddFrameworkToProject (target, "MessageUI.framework", false);
			proj.AddFrameworkToProject (target, "ImageIO.framework", false);
			proj.AddFrameworkToProject (target, "libz.tbd", false);
			proj.AddFrameworkToProject (target, "libxml2.tbd", false);
            proj.AddFrameworkToProject (target, "libc++.tbd", false);

			File.WriteAllText (projPath, proj.WriteToString ());
		}
	}
	#endif
}

