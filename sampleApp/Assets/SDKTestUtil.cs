using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SDKTestUtil {
    
	// 端末情報取得
	public static string GetInfo(List<string> infos) {
		infos.Add("--------------------");
		#if UNITY_IPHONE
		infos.Add("サポートOSバージョン：" + VAMPUnitySDK.SupportedOSVersion());
		infos.Add("サポート対象OS：" + VAMPUnitySDK.isSupportedOSVersion());
		#elif UNITY_ANDROID
		infos.Add("サポートAPI Level：" + VAMPUnitySDK.SupportedOSVersion());
		infos.Add("サポート対象OS：" + VAMPUnitySDK.isSupportedOSVersion());
		#endif
		infos.Add("--------------------");
        infos.Add("SDK_Ver(VAMP)：" + GetVersion("VAMP"));
        infos.Add("SDK_Ver(Admob)：" + GetVersion("Admob"));
        infos.Add("SDK_Ver(AppLovin)：" + GetVersion("AppLovin"));
        infos.Add("SDK_Ver(FAN)：" + GetVersion("FAN"));
        infos.Add("SDK_Ver(Maio)：" + GetVersion("Maio"));
        infos.Add("SDK_Ver(Nend)：" + GetVersion("Nend"));
        infos.Add("SDK_Ver(Tapjoy)：" + GetVersion("Tapjoy"));
        infos.Add("SDK_Ver(UnityAds)：" + GetVersion("UnityAds"));
		infos.Add("SDK_Ver(Vungle)：" + GetVersion("Vungle"));
		infos.Add("--------------------");
		string versionName = GetAppInfo(infos);
		GetTerminalInfo(infos);
		return versionName;
	}

	private static void GetTerminalInfo(List<string> infos) {
		#if UNITY_IPHONE
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			infos.Add("デバイス名：" + VAMPUnitySDK.SDKInfo("DeviceName"));
			infos.Add("OS名：" + VAMPUnitySDK.SDKInfo("OSName"));
			infos.Add("OSバージョン：" + VAMPUnitySDK.SDKInfo("OSVersion"));
			infos.Add("OSモデル：" + VAMPUnitySDK.SDKInfo("OSModel"));
			infos.Add("キャリア情報：" + VAMPUnitySDK.SDKInfo("Carrier"));
			infos.Add("国コード：" + VAMPUnitySDK.SDKInfo("CountryCode"));
			infos.Add("IDFA：" + VAMPUnitySDK.SDKInfo("IDFA"));
			infos.Add("--------------------");


		}
		#elif UNITY_ANDROID
		if (Application.platform == RuntimePlatform.Android) {
			string androidVersion = "";
			int apiLevel = -1;
			string manufacturer = "";
			string model = "";
			string brand = "";
			try {
				AndroidJavaClass build = new AndroidJavaClass("android.os.Build");
				AndroidJavaClass version = new AndroidJavaClass("android.os.Build$VERSION");
				androidVersion = version.GetStatic<string>("RELEASE");
				apiLevel = version.GetStatic<int>("SDK_INT");
				manufacturer = build.GetStatic<string>("MANUFACTURER");
				model = build.GetStatic<string>("MODEL");
				brand = build.GetStatic<string>("BRAND");
			} catch (AndroidJavaException ex) {
				Debug.Log("Exception:"+ex.Message);
			}
			infos.Add("Androidバージョン：" + androidVersion);
			infos.Add("API Level：" + apiLevel);
			infos.Add("メーカー名：" + manufacturer);
			infos.Add("モデル番号：" + model);
			infos.Add("ブランド名：" + brand);
			infos.Add("--------------------");
		}
		#endif
	}

	private static string GetAppInfo(List<string> infos) {
		string versionName = "1.0";
		#if UNITY_IPHONE
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			infos.Add("BundleID：" + VAMPUnitySDK.SDKInfo("BundleID"));
			infos.Add("--------------------");

		}
		#elif UNITY_ANDROID
		if (Application.platform == RuntimePlatform.Android) {
			string packageName = "";
			string appName = "";
			int versionCode = 0;
			try {
				AndroidJavaClass player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				AndroidJavaObject activity = player.GetStatic<AndroidJavaObject>("currentActivity");
				AndroidJavaObject packageManager = activity.Call<AndroidJavaObject>("getPackageManager");
				AndroidJavaClass packageManagerClass = new AndroidJavaClass("android.content.pm.PackageManager");
				packageName = activity.Call<string>("getPackageName");
				int activities = packageManagerClass.GetStatic<int>("GET_ACTIVITIES");
				AndroidJavaObject packageInfo = packageManager.Call<AndroidJavaObject>("getPackageInfo", packageName, activities);
				appName = packageManager.Call<string>("getApplicationLabel", packageInfo.Get<AndroidJavaObject>("applicationInfo"));
				versionCode = packageInfo.Get<int>("versionCode");
				versionName = packageInfo.Get<string>("versionName");
			} catch (AndroidJavaException ex) {
				Debug.Log(ex.Message);
			}
			infos.Add("アプリ名：" + appName);
			infos.Add("パッケージ名：" + packageName);
			infos.Add("バージョンコード：" + versionCode);
			infos.Add("バージョン名：" + versionName);
			infos.Add("--------------------");
		}
		#endif
		return versionName;
	}

	private static string GetVersion(string adnw) {
		string version = "nothing";
		#if UNITY_IPHONE
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			version = VAMPUnitySDK.ADNWSDKVersion(adnw);
		}
		#elif UNITY_ANDROID
		if (Application.platform == RuntimePlatform.Android) {
			AndroidJavaClass cls;
			try {
				switch(adnw) {
				case "VAMP":
					version = VAMPUnitySDK.SDKVersion();
					break;
				case "ADGPlayer":
					cls = new AndroidJavaClass("jp.supership.adgplayer.ADGPlayer");
					version = cls.CallStatic<string>("getVersion");
					break;
				case "Admob":
					AndroidJavaClass player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
					AndroidJavaObject activity = player.GetStatic<AndroidJavaObject>("currentActivity");
					AndroidJavaObject res = activity.Call<AndroidJavaObject>("getResources");
					string packageName = activity.Call<string>("getPackageName");
					int versionId = res.Call<int>("getIdentifier", "google_play_services_version", "integer", packageName);
					if (versionId != 0) {
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
			} catch (AndroidJavaException ex) {
				Debug.Log(ex.Message);
			}
		}
		#endif
		return version;
	}

    public static void AddFANTestDevice(string deviceIdHash) {
        #if UNITY_ANDROID
        try {
            AndroidJavaClass cls = new AndroidJavaClass("com.facebook.ads.AdSettings");
            cls.CallStatic("addTestDevice", new string[] { deviceIdHash });
        } catch (AndroidJavaException ex) {
            Debug.Log(ex.Message);
        }
        #endif
    }

    public static void ClearFANTestDevices() {
        #if UNITY_ANDROID
        try {
            AndroidJavaClass cls = new AndroidJavaClass("com.facebook.ads.AdSettings");
            cls.CallStatic("clearTestDevices");
        } catch (AndroidJavaException ex) {
            Debug.Log(ex.Message);
        }
        #endif
    }
}
