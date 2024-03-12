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

    public static string CountryCode = "";

    public static List<string> GetDeviceInfo() {
        return new List<string>
               {
                   "--------------------",
                   "VAMPUnityPlugin: " + VAMP.SDK.VAMPUnityPluginVersion,
                   "VAMP SDK: " + VAMP.SDK.SDKVersion,
                   "AdMob: " + GetAdnwSDKVersion("AdMob"),
                   "ironSource: " + GetAdnwSDKVersion("IronSource"),
                   "LINEAds: " + GetAdnwSDKVersion("LINEAds"),
                   "maio: " + GetAdnwSDKVersion("Maio"),
                   "Pangle: " + GetAdnwSDKVersion("Pangle"),
                   "UnityAds: " + GetAdnwSDKVersion("UnityAds"),
                   "--------------------",
                   "Unity: " + Application.unityVersion,
                   "ビルド: " + Application.buildGUID,
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
                   "Country Code: " + CountryCode,
                   "--------------------",
                   "IsSupported: " + VAMP.SDK.IsSupported,
                   "UseHyperID: " + VAMP.SDK.UseHyperID,
                   "--------------------",
               };
    }

    private static string GetAdnwSDKVersion(string adnw) {
#if UNITY_IOS && !UNITY_EDITOR
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
            return VAMPUnityTestAdnwSDKVersion(adnw) + " | " + VAMPUnityTestAdapterVersion(adnw);
        }
#elif UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android) {
            switch (adnw) {
                case "AdMob":
                {
                    const string name = "jp.supership.vamp.mediation.admob.AdMobAdapter";
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
                case "Pangle":
                {
                    const string name = "jp.supership.vamp.mediation.pangle.PangleAdapter";
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
    private static string AndroidGetAdnwVersion(string className) {
        if (Application.platform != RuntimePlatform.Android) {
            return "nothing";
        }

        try
        {
            using (var adapter = new AndroidJavaObject(className))
            {
                return adapter.Call<string>("getAdNetworkVersion");
            }
        }
        catch (Exception)
        {
            return "nothing";
        }
    }

    private static string AndroidGetAdapterVersion(string className) {
        if (Application.platform != RuntimePlatform.Android) {
            return "nothing";
        }

        try
        {
            using (var adapter = new AndroidJavaObject(className))
            {
                return adapter.Call<string>("getAdapterVersion");
            }
        }
        catch (Exception)
        {
            return "nothing";
        }
    }
#endif
}