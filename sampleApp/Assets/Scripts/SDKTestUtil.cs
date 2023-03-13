using System;
using System.Collections.Generic;
using UnityEngine;

public static class SDKTestUtil
{
#if UNITY_IOS
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern string VAMPUnityTestAdnwSDKVersion(string adnwName);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern string VAMPUnityTestAdapterVersion(string adnwName);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    public static extern string VAMPUnityTestDeviceInfo(string infoName);
#endif

    public static List<string> GetDeviceInfo()
    {
        return new List<string>
        {
            "--------------------",
            "IsSupported: " + VAMP.SDK.IsSupported,
            "UseMetaAudienceNetworkBidding: " + VAMP.SDK.UseMetaAudienceNetworkBidding,
            "IsMetaAudienceNetworkBiddingTestMode: " +
            VAMP.SDK.IsMetaAudienceNetworkBiddingTestMode,
            "UseHyperID: " + VAMP.SDK.UseHyperID,
            "--------------------",
            "VAMPUnityPlugin: " + VAMP.SDK.VAMPUnityPluginVersion,
            "VAMP SDK: " + VAMP.SDK.SDKVersion,
            "AdMob: " + GetAdnwSDKVersion("AdMob"),
            "FAN: " + GetAdnwSDKVersion("FAN"),
            "ironSource: " + GetAdnwSDKVersion("IronSource"),
            "LINEAds: " + GetAdnwSDKVersion("LINEAds"),
            "maio: " + GetAdnwSDKVersion("Maio"),
            "nend: " + GetAdnwSDKVersion("Nend"),
            "Pangle: " + GetAdnwSDKVersion("Pangle"),
            "Tapjoy: " + GetAdnwSDKVersion("Tapjoy"),
            "UnityAds: " + GetAdnwSDKVersion("UnityAds"),
            "--------------------",
            "プロダクト名: " + Application.productName,
            "アプリID: " + Application.identifier,
            "バージョン名: " + Application.version,
            "--------------------",
            "デバイス名: " + SystemInfo.deviceName,
            "OS: " + SystemInfo.operatingSystem,
            "デバイスモデル: " + SystemInfo.deviceModel,
#if UNITY_IOS && !UNITY_EDITOR
            "キャリア情報: " + VAMPUnityTestDeviceInfo("Carrier"),
            "国コード: " + VAMPUnityTestDeviceInfo("CountryCode"),
            "IDFA: " + VAMPUnityTestDeviceInfo("IDFA"),
#endif
            "--------------------",
            "Unity: " + Application.unityVersion,
            "ビルド: " + Application.buildGUID,
            "--------------------",
        };
    }

    private static string GetAdnwSDKVersion(string adnw)
    {
#if UNITY_IOS && !UNITY_EDITOR
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            return VAMPUnityTestAdnwSDKVersion(adnw) + " | " + VAMPUnityTestAdapterVersion(adnw);
        }
#elif UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            switch (adnw)
            {
                case "AdMob":
                {
                    const string name = "jp.supership.vamp.mediation.admob.AdMobAdapter";
                    return AndroidGetAdnwVersion(name) + " | " + AndroidGetAdapterVersion(name);
                }
                case "FAN":
                {
                    const string name = "jp.supership.vamp.mediation.fan.FANAdapter";
                    return AndroidGetAdnwVersion(name) + " | " + AndroidGetAdapterVersion(name);
                }
                case "IronSource":
                {
                    const string name = "jp.supership.vamp.mediation.ironsource.IronSourceAdapter";
                    return AndroidGetAdnwVersion(name) + " | " + AndroidGetAdapterVersion(name);
                }
                case "LINEAds":
                {
                    const string name = "jp.supership.vamp.mediation.lineads.LINEAdsAdapter";
                    return AndroidGetAdnwVersion(name) + " | " + AndroidGetAdapterVersion(name);
                }
                case "Maio":
                {
                    const string name = "jp.supership.vamp.mediation.maio.MaioAdapter";
                    return AndroidGetAdnwVersion(name) + " | " + AndroidGetAdapterVersion(name);
                }
                case "Nend":
                {
                    const string name = "jp.supership.vamp.mediation.nend.NendAdapter";
                    return AndroidGetAdnwVersion(name) + " | " + AndroidGetAdapterVersion(name);
                }
                case "Pangle":
                {
                    const string name = "jp.supership.vamp.mediation.pangle.PangleAdapter";
                    return AndroidGetAdnwVersion(name) + " | " + AndroidGetAdapterVersion(name);
                }
                case "Tapjoy":
                {
                    const string name = "jp.supership.vamp.mediation.tapjoy.TapjoyAdapter";
                    return AndroidGetAdnwVersion(name) + " | " + AndroidGetAdapterVersion(name);
                }
                case "UnityAds":
                {
                    const string name = "jp.supership.vamp.mediation.unityads.UnityAdsAdapter";
                    return AndroidGetAdnwVersion(name) + " | " + AndroidGetAdapterVersion(name);
                }
            }
        }
#endif

        return "nothing";
    }

#if UNITY_ANDROID
    private static string AndroidGetAdnwVersion(string className)
    {
        if (Application.platform != RuntimePlatform.Android) return "nothing";

        try
        {
            using (var adapter = new AndroidJavaObject(className))
            {
                return adapter.Call<string>("getAdNetworkVersion");
            }
        }
        catch (Exception e)
        {
            return "nothing";
        }
    }

    private static string AndroidGetAdapterVersion(string className)
    {
        if (Application.platform != RuntimePlatform.Android) return "nothing";

        try
        {
            using (var adapter = new AndroidJavaObject(className))
            {
                return adapter.Call<string>("getAdapterVersion");
            }
        }
        catch (Exception e)
        {
            return "nothing";
        }
    }
#endif

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