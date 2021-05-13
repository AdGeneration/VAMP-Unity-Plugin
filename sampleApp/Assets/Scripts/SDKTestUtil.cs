using System.Collections.Generic;
using UnityEngine;

public static class SDKTestUtil
{
#if UNITY_IOS
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern string VAMPUnityTestAdnwSDKVersion(string adnwName);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    public static extern string VAMPUnityTestDeviceInfo(string infoName);
#endif
    public static string GetAppVersion()
    {
        string ver = Application.version;

#if UNITY_IOS && !UNITY_EDITOR
        ver = VAMPUnityTestDeviceInfo("AppVer");
#elif UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            using (var playerCls = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (var activity = playerCls.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    using (var packageManager = activity.Call<AndroidJavaObject>("getPackageManager"))
                    {
                        using (var packageManagerCls = new AndroidJavaClass("android.content.pm.PackageManager"))
                        {
                            string packageName = activity.Call<string>("getPackageName");
                            int activities = packageManagerCls.GetStatic<int>("GET_ACTIVITIES");
                            using (var packageInfo = packageManager.Call<AndroidJavaObject>("getPackageInfo", packageName, activities))
                            {
                                ver = packageInfo.Get<string>("versionName");
                            }
                        }
                    }
                }
            }
        }
        catch (AndroidJavaException ex)
        {
            Debug.Log(ex.Message);
        }
#endif

        return ver;
    }

    public static List<string> GetDeviceInfo()
    {
        return new List<string>
        {
            "--------------------",
            "IsSupported：" + VAMP.SDK.IsSupported,
            "--------------------",
            "UnityPlugin_Ver：" + VAMP.SDK.VAMPUnityPluginVersion,
            "SDK_Ver(VAMP)：" + VAMP.SDK.SDKVersion,
            "SDK_Ver(AdMob)：" + GetAdnwSDKVersion("AdMob"),
            "SDK_Ver(AppLovin)：" + GetAdnwSDKVersion("AppLovin"),
            "SDK_Ver(FAN)：" + GetAdnwSDKVersion("FAN"),
            "SDK_Ver(Maio)：" + GetAdnwSDKVersion("Maio"),
            "SDK_Ver(Nend)：" + GetAdnwSDKVersion("Nend"),
            "SDK_Ver(Tapjoy)：" + GetAdnwSDKVersion("Tapjoy"),
            "SDK_Ver(UnityAds)：" + GetAdnwSDKVersion("UnityAds"),
            "SDK_Ver(Pangle)：" + GetAdnwSDKVersion("Pangle"),
            "SDK_Ver(LINEAds)：" + GetAdnwSDKVersion("LINEAds"),
            "--------------------",
            "プロダクト名：" + Application.productName,
            "アプリID：" + Application.identifier,
            "バージョン名：" + Application.version,
            "--------------------",
            "デバイス名：" + SystemInfo.deviceName,
            "OS：" + SystemInfo.operatingSystem,
            "デバイスモデル：" + SystemInfo.deviceModel,
#if UNITY_IOS && !UNITY_EDITOR
            "キャリア情報：" + VAMPUnityTestDeviceInfo("Carrier"),
            "国コード：" + VAMPUnityTestDeviceInfo("CountryCode"),
            "IDFA：" + VAMPUnityTestDeviceInfo("IDFA"),
#endif
            "--------------------",
            "Unity：" + Application.unityVersion,
            "ビルド：" + Application.buildGUID,
            "--------------------",
        };
    }

    public static string GetAdnwSDKVersion(string adnw)
    {
        string version = "nothing";
#if UNITY_IOS && !UNITY_EDITOR
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            version = VAMPUnityTestAdnwSDKVersion(adnw);
        }
#elif UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            try
            {
                switch (adnw)
                {
                    case "AdMob":
                        using (var playerCls = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                        {
                            using (var activity = playerCls.GetStatic<AndroidJavaObject>("currentActivity"))
                            {
                                using (var res = activity.Call<AndroidJavaObject>("getResources"))
                                {
                                    var packageName = activity.Call<string>("getPackageName");
                                    var versionId = res.Call<int>("getIdentifier", "google_play_services_version", "integer", packageName);

                                    if (versionId != 0)
                                    {
                                        int versionInt = res.Call<int>("getInteger", versionId);
                                        version = (versionInt).ToString();
                                    }
                                }
                            }
                        }

                        break;
                    case "AppLovin":
                        using (var cls = new AndroidJavaClass("com.applovin.sdk.AppLovinSdk"))
                        {
                            version = cls.CallStatic<string>("getVersion");
                        }
                        break;
                    case "FAN":
                        using (var obj = new AndroidJavaObject("com.facebook.ads.BuildConfig"))
                        {
                            version = obj.GetStatic<string>("VERSION_NAME");
                        }
                        break;
                    case "Maio":
                        using (var cls = new AndroidJavaClass("jp.maio.sdk.android.MaioAds"))
                        {
                            version = cls.CallStatic<string>("getSdkVersion");
                        }
                        break;
                    case "Nend":
                        using (var obj = new AndroidJavaObject("net.nend.android.BuildConfig"))
                        {
                            version = obj.GetStatic<string>("NEND_SDK_VERSION");
                        }
                        break;
                    case "Tapjoy":
                        using (var cls = new AndroidJavaClass("com.tapjoy.Tapjoy"))
                        {
                            version = cls.CallStatic<string>("getVersion");
                        }
                        break;
                    case "UnityAds":
                        using (var cls = new AndroidJavaClass("com.unity3d.ads.UnityAds"))
                        {
                            version = cls.CallStatic<string>("getVersion");
                        }
                        break;
                    case "LINEAds":
                        using (var cls = new AndroidJavaClass("com.five_corp.ad.FiveAd"))
                        {
                            if (cls.CallStatic<bool>("isInitialized"))
                            {
                                var fiveAd = cls.CallStatic<AndroidJavaObject>("getSingleton");
                                version = fiveAd.Call<string>("getVersion");
                            }
                        }
                        break;
                    case "Pangle":
                        using (var cls = new AndroidJavaClass("com.bytedance.sdk.openadsdk.TTAdSdk"))
                        {
                            var adManager = cls.CallStatic<AndroidJavaObject>("getAdManager");
                            version = adManager.Call<string>("getSDKVersion");
                        }
                        break;
                }
            }
            catch (AndroidJavaException ex)
            {
                Debug.Log(adnw + ":" + ex.Message);
            }
        }
#endif
        Debug.Log(adnw + ":" + version);
        return version;
    }

    public static void AddFANTestDevice(string deviceIdHash)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            using(var cls = new AndroidJavaClass("com.facebook.ads.AdSettings"))
            {
                cls.CallStatic("addTestDevice", new string[] { deviceIdHash });
            }
        }
        catch (AndroidJavaException ex)
        {
            Debug.Log(ex.Message);
        }
#endif
    }

    public static void ClearFANTestDevices()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            using(var cls = new AndroidJavaClass("com.facebook.ads.AdSettings"))
            {
                cls.CallStatic("clearTestDevices");
            }
        }
        catch (AndroidJavaException ex)
        {
            Debug.Log(ex.Message);
        }
#endif
    }
}