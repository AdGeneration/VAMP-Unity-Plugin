using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SDKTestUtil
{
    public static string GetAppVersion()
    {
        string ver = "nothing";

        #if UNITY_IPHONE
        ver = VAMPUnitySDK.DeviceUtil.GetInfo("AppVer");
        #elif UNITY_ANDROID
        try
        {
            AndroidJavaClass playerCls = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = playerCls.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject packageManager = activity.Call<AndroidJavaObject>("getPackageManager");
            AndroidJavaClass packageManagerCls = new AndroidJavaClass("android.content.pm.PackageManager");
            string packageName = activity.Call<string>("getPackageName");
            int activities = packageManagerCls.GetStatic<int>("GET_ACTIVITIES");
            AndroidJavaObject packageInfo = packageManager.Call<AndroidJavaObject>("getPackageInfo", packageName, activities);
            ver = packageInfo.Get<string>("versionName");
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
        List<string> infos = new List<string>();

        infos.Add("--------------------");

        #if UNITY_IPHONE
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            infos.Add("サポートOSバージョン：" + VAMPUnitySDK.SupportedOSVersion());
            infos.Add("サポート対象OS：" + VAMPUnitySDK.isSupportedOSVersion());
            infos.Add("--------------------");
            infos.Add("SDK_Ver(VAMP)：" + VAMPUnitySDK.SDKVersion());
            infos.Add("SDK_Ver(Admob)：" + GetAdnwSDKVersion("Admob"));
            infos.Add("SDK_Ver(AppLovin)：" + GetAdnwSDKVersion("AppLovin"));
            infos.Add("SDK_Ver(FAN)：" + GetAdnwSDKVersion("FAN"));
            infos.Add("SDK_Ver(Maio)：" + GetAdnwSDKVersion("Maio"));
            infos.Add("SDK_Ver(Nend)：" + GetAdnwSDKVersion("Nend"));
            infos.Add("SDK_Ver(Tapjoy)：" + GetAdnwSDKVersion("Tapjoy"));
            infos.Add("SDK_Ver(UnityAds)：" + GetAdnwSDKVersion("UnityAds"));
            infos.Add("SDK_Ver(Vungle)：" + GetAdnwSDKVersion("Vungle"));
            infos.Add("--------------------");
            infos.Add("BundleID：" + VAMPUnitySDK.DeviceUtil.GetInfo("BundleID"));
            infos.Add("バージョン名：" + VAMPUnitySDK.DeviceUtil.GetInfo("AppVer"));
            infos.Add("--------------------");
            infos.Add("デバイス名：" + VAMPUnitySDK.DeviceUtil.GetInfo("DeviceName"));
            infos.Add("OS名：" + VAMPUnitySDK.DeviceUtil.GetInfo("OSName"));
            infos.Add("OSバージョン：" + VAMPUnitySDK.DeviceUtil.GetInfo("OSVersion"));
            infos.Add("OSモデル：" + VAMPUnitySDK.DeviceUtil.GetInfo("OSModel"));
            infos.Add("キャリア情報：" + VAMPUnitySDK.DeviceUtil.GetInfo("Carrier"));
            infos.Add("国コード：" + VAMPUnitySDK.DeviceUtil.GetInfo("CountryCode"));
            infos.Add("IDFA：" + VAMPUnitySDK.DeviceUtil.GetInfo("IDFA"));
        }
        #elif UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            string appName = "";
            string packageName = "";
            string versionName = "";
            string androidVersion = "";
            string manufacturer = "";
            string model = "";
            string brand = "";
            int versionCode = 0;
            int apiLevel = -1;

            try
            {
                AndroidJavaClass playerCls = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject activity = playerCls.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaObject packageManager = activity.Call<AndroidJavaObject>("getPackageManager");
                AndroidJavaClass packageManagerCls = new AndroidJavaClass("android.content.pm.PackageManager");
                packageName = activity.Call<string>("getPackageName");
                int activities = packageManagerCls.GetStatic<int>("GET_ACTIVITIES");
                AndroidJavaObject packageInfo = packageManager.Call<AndroidJavaObject>("getPackageInfo", packageName, activities);
                appName = packageManager.Call<string>("getApplicationLabel", packageInfo.Get<AndroidJavaObject>("applicationInfo"));
                versionCode = packageInfo.Get<int>("versionCode");
                versionName = packageInfo.Get<string>("versionName");
                AndroidJavaClass buildCls = new AndroidJavaClass("android.os.Build");
                AndroidJavaClass versionCls = new AndroidJavaClass("android.os.Build$VERSION");
                androidVersion = versionCls.GetStatic<string>("RELEASE");
                apiLevel = versionCls.GetStatic<int>("SDK_INT");
                manufacturer = buildCls.GetStatic<string>("MANUFACTURER");
                model = buildCls.GetStatic<string>("MODEL");
                brand = buildCls.GetStatic<string>("BRAND");
            }
            catch (AndroidJavaException ex)
            {
                Debug.Log(ex.Message);
            }

            infos.Add("サポートAPI Level：" + VAMPUnitySDK.SupportedOSVersion());
            infos.Add("サポート対象OS：" + VAMPUnitySDK.isSupportedOSVersion());
            infos.Add("--------------------");
            infos.Add("SDK_Ver(VAMP)：" + VAMPUnitySDK.SDKVersion());
            infos.Add("SDK_Ver(Admob)：" + GetAdnwSDKVersion("Admob"));
            infos.Add("SDK_Ver(AppLovin)：" + GetAdnwSDKVersion("AppLovin"));
            infos.Add("SDK_Ver(FAN)：" + GetAdnwSDKVersion("FAN"));
            infos.Add("SDK_Ver(Maio)：" + GetAdnwSDKVersion("Maio"));
            infos.Add("SDK_Ver(Nend)：" + GetAdnwSDKVersion("Nend"));
            infos.Add("SDK_Ver(Tapjoy)：" + GetAdnwSDKVersion("Tapjoy"));
            infos.Add("SDK_Ver(UnityAds)：" + GetAdnwSDKVersion("UnityAds"));
            infos.Add("SDK_Ver(Vungle)：" + GetAdnwSDKVersion("Vungle"));
            infos.Add("--------------------");
            infos.Add("アプリ名：" + appName);
            infos.Add("パッケージ名：" + packageName);
            infos.Add("バージョンコード：" + versionCode);
            infos.Add("バージョン名：" + versionName);
            infos.Add("--------------------");
            infos.Add("Androidバージョン：" + androidVersion);
            infos.Add("API Level：" + apiLevel);
            infos.Add("メーカー名：" + manufacturer);
            infos.Add("モデル番号：" + model);
            infos.Add("ブランド名：" + brand);
        }
        #endif

        infos.Add("--------------------");

        return infos;
    }

    private static string GetAdnwSDKVersion(string adnw)
    {
        string version = "nothing";

        #if UNITY_IPHONE
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            version = VAMPUnitySDK.SDKUtil.GetAdnwSDKVersion(adnw);
        }
        #elif UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidJavaClass cls;
            try
            {
                switch (adnw)
                {
                    case "Admob":
                        AndroidJavaClass playerCls = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                        AndroidJavaObject activity = playerCls.GetStatic<AndroidJavaObject>("currentActivity");
                        AndroidJavaObject res = activity.Call<AndroidJavaObject>("getResources");
                        string packageName = activity.Call<string>("getPackageName");
                        int versionId = res.Call<int>("getIdentifier", "google_play_services_version", "integer", packageName);

                        if (versionId != 0)
                        {
                            int versionInt = res.Call<int>("getInteger", versionId);
                            version = (versionInt).ToString();
                        }

                        break;
                    case "AppLovin":
                        cls = new AndroidJavaClass("com.applovin.sdk.AppLovinSdk");
                        version = cls.GetStatic<string>("VERSION");
                        break;
                    case "FAN":
                        cls = new AndroidJavaClass("com.facebook.ads.internal.AdSdkVersion");
                        version = cls.GetStatic<string>("BUILD");
                        break;
                    case "Maio":
                        cls = new AndroidJavaClass("jp.maio.sdk.android.MaioAds");
                        version = cls.CallStatic<string>("getSdkVersion");
                        break;
                    case "Nend":
                        version = "Unknown";
                        break;
                    case "Tapjoy":
                        cls = new AndroidJavaClass("com.tapjoy.Tapjoy");
                        version = cls.CallStatic<string>("getVersion");
                        break;
                    case "UnityAds":
                        cls = new AndroidJavaClass("com.unity3d.ads.UnityAds");
                        version = cls.CallStatic<string>("getVersion");
                        break;
                    case "Vungle":
                        cls = new AndroidJavaClass("com.vungle.publisher.VunglePub");
                        version = cls.GetStatic<string>("VERSION");
                        break;
                }
            }
            catch (AndroidJavaException ex)
            {
                Debug.Log(ex.Message);
            }
        }
        #endif

        return version;
    }

    public static void AddFANTestDevice(string deviceIdHash)
    {
        #if UNITY_ANDROID
        try
        {
            AndroidJavaClass cls = new AndroidJavaClass("com.facebook.ads.AdSettings");
            cls.CallStatic("addTestDevice", new string[] { deviceIdHash });
        }
        catch (AndroidJavaException ex)
        {
            Debug.Log(ex.Message);
        }
        #endif
    }

    public static void ClearFANTestDevices()
    {
        #if UNITY_ANDROID
        try
        {
            AndroidJavaClass cls = new AndroidJavaClass("com.facebook.ads.AdSettings");
            cls.CallStatic("clearTestDevices");
        }
        catch (AndroidJavaException ex)
        {
            Debug.Log(ex.Message);
        }
        #endif
    }
}

