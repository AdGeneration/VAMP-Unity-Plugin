/// <summary>
///
/// VAMP-Unity-Plugin
///
/// Created by AdGeneratioin.
/// Copyright 2018 Supership Inc. All rights reserved.
///
/// </summary>

#if UNITY_EDITOR_OSX
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

#if UNITY_IOS
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;

public class PostBuildProcess
{
    private static readonly string[] privateIosPodsForMainTarget = {
        "  pod 'AppLovinSDK', '13.5.0'",
        "  pod 'MaioSDK-v2', '2.2.1'",
    };

    // AdMob AppID
    private static readonly string plistKeyadMobAppId = "GADApplicationIdentifier";
    private static readonly string adMobAppId = "ca-app-pub-3940256099942544~3347511713";

    // ATT
    private static readonly string plistKeyTrackingUsageDescription =
        "NSUserTrackingUsageDescription";

    private static readonly string trackingUsageDescription =
        "App would like to access IDFA for tracking purpose";

    // SKAdNetwork
    private static readonly string plistKeySKAdNetworkItems = "SKAdNetworkItems";
    private static readonly string plistKeySKAdNetworkIdentifier = "SKAdNetworkIdentifier";

    // Canonical source: VAMP-iOS-SDK/ProvisionSample/VAMPAppOpenAdsSwiftUISample/
    //                   VAMPAppOpenAdsSwiftUISample/Info.plist (Issue #51 §11 案 B)
    // VAMP-iOS-SDK の release ごとに canonical source を再確認し、本配列を更新する責務が
    // plugin 保守側に発生する。
    private static readonly string[] VAMP_SKADNETWORK_IDS = {
        "22mmun2rn5.skadnetwork",
        "238da6jt44.skadnetwork",
        "24t9a8vw3c.skadnetwork",
        "252b5q8x7y.skadnetwork",
        "275upjj5gd.skadnetwork",
        "294l99pt4k.skadnetwork",
        "2fnua5tdw4.skadnetwork",
        "2u9pt9hc89.skadnetwork",
        "32z4fx6l9h.skadnetwork",
        "348l86zlvx.skadnetwork",
        "3l6bd9hu43.skadnetwork",
        "3qcr597p9d.skadnetwork",
        "3qy4746246.skadnetwork",
        "3rd42ekr43.skadnetwork",
        "3sh42y64q3.skadnetwork",
        "424m5254lk.skadnetwork",
        "4468km3ulz.skadnetwork",
        "44jx6755aq.skadnetwork",
        "44n7hlldy6.skadnetwork",
        "47vhws6wlr.skadnetwork",
        "488r3q3dtq.skadnetwork",
        "4dzt52r2t5.skadnetwork",
        "4fzdc2evr5.skadnetwork",
        "4mn522wn87.skadnetwork",
        "4pfyvq9l8r.skadnetwork",
        "4w7y6s5ca2.skadnetwork",
        "523jb4fst2.skadnetwork",
        "52fl2v3hgk.skadnetwork",
        "54nzkqm89y.skadnetwork",
        "5594blyghf.skadnetwork",
        "578prtvx9j.skadnetwork",
        "5a6flpkh64.skadnetwork",
        "5l3tpt7t6e.skadnetwork",
        "5lm9lj6jb7.skadnetwork",
        "5tjdwbrq8w.skadnetwork",
        "6g9af3uyq4.skadnetwork",
        "6xzpu9s2p8.skadnetwork",
        "6yxyv74ff7.skadnetwork",
        "737z793b9f.skadnetwork",
        "74b6s63p6l.skadnetwork",
        "79pbpufp6p.skadnetwork",
        "7fmhfwg9en.skadnetwork",
        "7rz58n8ntl.skadnetwork",
        "7ug5zh24hu.skadnetwork",
        "866k9ut3g3.skadnetwork",
        "8c4e2ghe7u.skadnetwork",
        "8r8llnkz5a.skadnetwork",
        "8s468mfl3y.skadnetwork",
        "97r2b46745.skadnetwork",
        "9b89h5y424.skadnetwork",
        "9nlqeag3gk.skadnetwork",
        "9rd848q2bz.skadnetwork",
        "9t245vhmpl.skadnetwork",
        "9yg77x724h.skadnetwork",
        "a2p9lx4jpn.skadnetwork",
        "a8cz6cu7e5.skadnetwork",
        "av6w8kgt66.skadnetwork",
        "c3frkrj4fj.skadnetwork",
        "c6k4g5qg8m.skadnetwork",
        "cg4yq2srnc.skadnetwork",
        "cj5566h2ga.skadnetwork",
        "cp8zw746q7.skadnetwork",
        "cstr6suwn9.skadnetwork",
        "dbu4b84rxf.skadnetwork",
        "dkc879ngq3.skadnetwork",
        "dzg6xy7pwj.skadnetwork",
        "e5fvkxwrpn.skadnetwork",
        "ecpz2srf59.skadnetwork",
        "eh6m2bh4zr.skadnetwork",
        "ejvt5qm6ak.skadnetwork",
        "f38h382jlk.skadnetwork",
        "f73kdq92p3.skadnetwork",
        "f7s53z58qe.skadnetwork",
        "feyaarzu9v.skadnetwork",
        "g28c52eehv.skadnetwork",
        "g6gcrrvk4p.skadnetwork",
        "ggvn48r87g.skadnetwork",
        "glqzh8vgby.skadnetwork",
        "gta9lk7p23.skadnetwork",
        "gvmwg8q7h5.skadnetwork",
        "hdw39hrw9y.skadnetwork",
        "hs6bdukanm.skadnetwork",
        "k674qkevps.skadnetwork",
        "kbd757ywx3.skadnetwork",
        "kbmxgpxpgc.skadnetwork",
        "klf5c3l5u5.skadnetwork",
        "lr83yxwka7.skadnetwork",
        "ludvb6z3bs.skadnetwork",
        "m5mvw97r93.skadnetwork",
        "m8dbw4sv7c.skadnetwork",
        "mlmmfzh3r3.skadnetwork",
        "mls7yz5dvl.skadnetwork",
        "mp6xlyr22a.skadnetwork",
        "mqn7fxpca7.skadnetwork",
        "mtkv5xtk9e.skadnetwork",
        "n38lu8286q.skadnetwork",
        "n66cz3y3bx.skadnetwork",
        "n6fk4nfna4.skadnetwork",
        "n9x2a789qt.skadnetwork",
        "nzq8sh4pbs.skadnetwork",
        "p78axxw29g.skadnetwork",
        "ppxm28t8ap.skadnetwork",
        "prcb7njmu6.skadnetwork",
        "pu4na253f3.skadnetwork",
        "pwa73g5rt2.skadnetwork",
        "qqp299437r.skadnetwork",
        "r45fhb6rf7.skadnetwork",
        "rvh3l7un93.skadnetwork",
        "s39g8k73mm.skadnetwork",
        "su67r6k2v3.skadnetwork",
        "t38b2kh725.skadnetwork",
        "tl55sbb4fm.skadnetwork",
        "u679fj5vs4.skadnetwork",
        "uw77j35x4d.skadnetwork",
        "v4nxqhlyqp.skadnetwork",
        "v72qych5uu.skadnetwork",
        "v79kvwwj4g.skadnetwork",
        "v9wttpbfk9.skadnetwork",
        "vcra2ehyfk.skadnetwork",
        "vutu7akeur.skadnetwork",
        "w9q455wk68.skadnetwork",
        "wg4vff78zm.skadnetwork",
        "wzmmz9fp6w.skadnetwork",
        "x44k69ngh6.skadnetwork",
        "x5l83yy675.skadnetwork",
        "x8jxxk4ff5.skadnetwork",
        "x8uqf25wch.skadnetwork",
        "xy9t38ct57.skadnetwork",
        "y45688jllp.skadnetwork",
        "y5ghdn5j9k.skadnetwork",
        "yclnxrl5pm.skadnetwork",
        "ydx93a7ass.skadnetwork",
        "yrqqpx2mcb.skadnetwork",
        "z4gj7hsk7h.skadnetwork",
        "zmvfpc5aq8.skadnetwork",
        "zq492l623r.skadnetwork",
    };

    // AR
    private static readonly string plistKeyCameraUsageDescription = "NSCameraUsageDescription";
    private static readonly string cameraUsageDescription = "For AR";

    private static readonly string plistKeyPhotoLibraryAddUsageDescription =
        "NSPhotoLibraryAddUsageDescription";

    private static readonly string
        photoLibraryAddUsageDescription = "Take a screenshot for AR demo";

    // Appearance
    private static readonly string plistKeyAppearance = "UIUserInterfaceStyle";
    private static readonly string appearance = "Dark";

    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string path) {
        if (buildTarget == BuildTarget.iOS) {
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
            proj.AddFrameworkToProject(target, "AppTrackingTransparency.framework", false);
            proj.AddFrameworkToProject(target, "CoreFoundation.framework", true);

            File.WriteAllText(projPath, proj.WriteToString());

            var plistPath = path + "/Info.plist";
            var plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));

            var rootDict = plist.root;
            rootDict.SetString(plistKeyadMobAppId, adMobAppId);
            rootDict.SetString(plistKeyTrackingUsageDescription, trackingUsageDescription);
            rootDict.SetString(plistKeyPhotoLibraryAddUsageDescription,
                               photoLibraryAddUsageDescription);
            rootDict.SetString(plistKeyCameraUsageDescription, cameraUsageDescription);
            MergeSKAdNetworkItems(rootDict);
            rootDict.SetString(plistKeyAppearance, appearance);
            File.WriteAllText(plistPath, plist.WriteToString());
        }
    }

    [PostProcessBuild(101)]
    public static void OnPostProcessBuildEmbedSwiftLibraries(BuildTarget target,
                                                             string      pathToBuiltProject) {
        // 「Always Embed Swift Standard Libraries」をYESに設定する
        var projPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
        var proj = new PBXProject();

        proj.ReadFromFile(projPath);
        var mainTarget = proj.GetUnityMainTargetGuid();
        proj.SetBuildProperty(mainTarget, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");
        var frameworkTarget = proj.GetUnityFrameworkTargetGuid();
        proj.SetBuildProperty(frameworkTarget, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");
        File.WriteAllText(projPath, proj.WriteToString());
    }

    // When using `use_frameworks! :linkage => :static`, CocoaPods converts most pods to static
    // linkage and does not generate an "Embed Pods Frameworks" build phase. However, some SDKs
    // (e.g. AppLovinSDK, Maio) ship as prebuilt dynamic XCFrameworks/frameworks that cannot be
    // made static. These must be explicitly embedded in the main app target so that they are
    // copied into the app bundle at build time and can be found by the dynamic linker at runtime.
    [PostProcessBuild(103)]
    public static void OnPostProcessBuildEmbedDynamicFrameworks(BuildTarget buildTarget,
                                                                string      path) {
        if (buildTarget != BuildTarget.iOS) {
            return;
        }

        var projPath = PBXProject.GetPBXProjectPath(path);
        var proj = new PBXProject();
        proj.ReadFromFile(projPath);

        var mainTarget = proj.GetUnityMainTargetGuid();
        var podsDir = Path.Combine(path, "Pods");

        if (!Directory.Exists(podsDir)) {
            Debug.LogWarning("Pods directory not found. Skipping framework embedding.");
            return;
        }

        // These are prebuilt dynamic frameworks that cannot be made static by CocoaPods
        // and must be embedded in the main app target.
        var dynamicFrameworks = new[] {
            "AppLovinSDK.xcframework",
            "Maio.xcframework",
        };

        foreach (var frameworkName in dynamicFrameworks) {
            var dirs = Directory.GetDirectories(podsDir, frameworkName,
                                                SearchOption.AllDirectories);
            if (dirs.Length == 0) {
                Debug.LogWarning($"{frameworkName} not found under Pods. Skipping.");
                continue;
            }

            var relativePath = dirs[0].Substring(path.Length + 1);
            var fileGuid = proj.AddFile(relativePath, $"Frameworks/{frameworkName}",
                                        PBXSourceTree.Source);
            PBXProjectExtensions.AddFileToEmbedFrameworks(proj, mainTarget, fileGuid);
        }

        File.WriteAllText(projPath, proj.WriteToString());
    }

    [PostProcessBuild(102)]
    public static void OnPostProcessBuildDisableBitcode(BuildTarget buildTarget, string path) {
        // 「Enable Bitcode」をNOに設定する
        var projPath = PBXProject.GetPBXProjectPath(path);
        var proj = new PBXProject();

        proj.ReadFromFile(projPath);
        var mainTarget = proj.GetUnityMainTargetGuid();
        proj.SetBuildProperty(mainTarget, "ENABLE_BITCODE", "NO");
        var frameworkTarget = proj.GetUnityFrameworkTargetGuid();
        proj.SetBuildProperty(frameworkTarget, "ENABLE_BITCODE", "NO");
        File.WriteAllText(projPath, proj.WriteToString());
    }

    // Must be between 40 and 50 to ensure that it's not overriden by Podfile generation (40) and
    // that it's added before "pod install" (50).
    [PostProcessBuildAttribute(45)]
    public static void OnPostProcessBuildPodfile(BuildTarget target, string buildPath) {
        if (target != BuildTarget.iOS) {
            return;
        }

        var podfilePath = Path.Combine(buildPath, "Podfile");
        if (!File.Exists(podfilePath)) {
            return;
        }

        var lines = File.ReadAllLines(podfilePath);
        using (var writer = new StringWriter()) {
            var insertedMainTargetPods = false;
            foreach (var line in lines) {
                writer.WriteLine(line);
                if (!insertedMainTargetPods && line.Contains("target 'Unity-iPhone' do")) {
                    foreach (var podLine in privateIosPodsForMainTarget) {
                        if (!ContainsPod(lines, podLine)) {
                            writer.WriteLine(podLine);
                        }
                    }
                    insertedMainTargetPods = true;
                }
            }
            File.WriteAllText(podfilePath, writer.ToString());
        }
    }

    private static bool ContainsPod(string[] lines, string podLine) {
        var podName = ExtractPodName(podLine);

        if (string.IsNullOrEmpty(podName)) {
            return false;
        }

        foreach (var line in lines) {
            if (string.Equals(ExtractPodName(line), podName, StringComparison.Ordinal)) {
                return true;
            }
        }

        return false;
    }

    // Issue #51 §11 案 B: 既存 SKAdNetworkItems を保持しつつ canonical 136 個を dedupe merge する。
    // メディア個別渡しサンプルに ADG SDK バナーも含むため Pangle 単体ではなく VAMP-iOS-SDK
    // 公式 sample の全 SKAdNetworkID をマージする。case-insensitive で比較する。
    private static void MergeSKAdNetworkItems(PlistElementDict rootDict) {
        PlistElementArray skanArray;

        if (rootDict.values.TryGetValue(plistKeySKAdNetworkItems, out var existingElement) &&
            existingElement.AsArray() is PlistElementArray existing) {
            skanArray = existing;
        }
        else {
            skanArray = rootDict.CreateArray(plistKeySKAdNetworkItems);
        }

        var existingIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var item in skanArray.values) {
            var dict = item.AsDict();
            if (dict == null) {
                continue;
            }

            if (!dict.values.TryGetValue(plistKeySKAdNetworkIdentifier, out var idElement)) {
                continue;
            }

            var id = idElement.AsString();
            if (!string.IsNullOrEmpty(id)) {
                existingIds.Add(id);
            }
        }

        foreach (var skanId in VAMP_SKADNETWORK_IDS) {
            // HashSet.Add() returns true only when the id is newly added; existing ids are skipped.
            if (existingIds.Add(skanId)) {
                skanArray.AddDict().SetString(plistKeySKAdNetworkIdentifier, skanId);
            }
        }
    }

    private static string ExtractPodName(string line) {
        var trimmed = line.Trim();

        if (!trimmed.StartsWith("pod '", StringComparison.Ordinal)) {
            return null;
        }

        var start = "pod '".Length;
        var end = trimmed.IndexOf('\'', start);
        if (end < 0) {
            return null;
        }

        return trimmed.Substring(start, end - start);
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

    void UnityEditor.Android.IPostGenerateGradleAndroidProject.OnPostGenerateGradleAndroidProject(string path) {
        var gradlePropertiesFile = path + "/gradle.properties";

        if (File.Exists(gradlePropertiesFile)) {
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
