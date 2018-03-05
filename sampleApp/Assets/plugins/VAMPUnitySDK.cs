using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

/// <summary>
///
/// VAMP-Unity-Plugin
///
/// Created by AdGeneratioin.
/// Copyright 2018 Supership Inc. All rights reserved.
///
/// </summary>
public class  VAMPUnitySDK : MonoBehaviour
{
    #if UNITY_IPHONE
    
    [DllImport("__Internal")]
    private static extern IntPtr VAMPUnityInit(IntPtr vampni, string placementId, string gameObjName);

    [DllImport("__Internal")]
    private static extern void VAMPUnityLoad(IntPtr vampni);

    [DllImport("__Internal")]
    private static extern bool VAMPUnityShow(IntPtr vampni);

    [DllImport("__Internal")]
    private static extern bool VAMPUnityIsReady(IntPtr vampni);

    [DllImport("__Internal")]
    private static extern void VAMPUnityClearLoaded(IntPtr vampni);

    [DllImport("__Internal")]
    private static extern void VAMPUnityInitializeAdnwSDK(string placementId);

    [DllImport("__Internal")]
    private static extern void VAMPUnityInitializeAdnwSDKWithConfig(string placementId, string state, int duration);

    [DllImport("__Internal")]
    private static extern void VAMPUnitySetTestMode(bool enableTest);

    [DllImport("__Internal")]
    private static extern bool VAMPUnityIsTestMode();

    [DllImport("__Internal")]
    private static extern void VAMPUnitySetDebugMode(bool enableDebug);

    [DllImport("__Internal")]
    private static extern bool VAMPUnityIsDebugMode();

    [DllImport("__Internal")]
    private static extern float VAMPUnitySupportedOSVersion();

    [DllImport("__Internal")]
    private static extern bool VAMPUnityIsSupportedOSVersion();

    [DllImport("__Internal")]
    private static extern string VAMPUnitySDKVersion();

    [DllImport("__Internal")]
    private static extern void VAMPUnitySetMediationTimeout(int timeout);

    [DllImport("__Internal")]
    private static extern void VAMPUnityGetCountryCode(string gameObjName);

    [DllImport("__Internal")]
    private static extern void VAMPUnitySetTargeting(int gender, int birthYear, int birthMonth, int birthDay);

    [DllImport("__Internal")]
    private static extern string VAMPUnityAdnwSDKVersion(string adnwName);

    [DllImport("__Internal")]
    private static extern string VAMPUnityDeviceInfo(string infoName);

    #endif

    public enum InitializeState
    {
        /// <summary>
        /// 接続環境によって、WEIGHTとALL設定お自動的に切り替えます
        /// Wi-Fi: ALL
        /// キャリア回線: WEIGHT
        /// </summary>
        AUTO = 0,
        /// <summary>
        /// 配信比率が高いものをひとつ初期化します
        /// </summary>
        WEIGHT,
        /// <summary>
        /// 全アドネットワークを初期化します
        /// </summary>
        ALL,
        /// <summary>
        /// Wi-Fi接続時のみ全アドネットワークを初期化します
        /// </summary>
        WIFIONLY
    }

    #if UNITY_ANDROID
    private const string VampClass = "jp.supership.vamp.VAMP";
    private const string UnityPlayerClass = "com.unity3d.player.UnityPlayer";
    #endif

    #if UNITY_IPHONE
    private static IntPtr vampni = IntPtr.Zero;

#elif UNITY_ANDROID
    private static AndroidJavaObject vampObj = null;
    #endif

    private static GameObject messageObj = null;
    private static GameObject countryCodeObj = null;

    public static string VAMPUnityPluginVersion
    {
        get
        {
            return "2.0.4";
        }
    }

    /// <summary>
    /// VAMPUnitySDKクラスの準備を行います
    /// </summary>
    /// <param name="obj">GameObject</param>
    /// <param name="placementID">広告枠ID</param>
    public static void initVAMP(GameObject obj, string placementID)
    {
        Debug.Log("VAMP-Unity-Plugin version: " + VAMPUnityPluginVersion);

        if (placementID == null || placementID.Length <= 0)
        {
            Debug.LogError("PlacementID is not set.");
            return;
        }

        messageObj = obj;

        #if UNITY_IPHONE
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            vampni = VAMPUnityInit(vampni, placementID, messageObj.name);
        }
        #elif UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            if (vampObj == null)
            {
                AndroidJavaClass player = new AndroidJavaClass(UnityPlayerClass);
                AndroidJavaObject activity = player.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaClass vampCls = new AndroidJavaClass(VampClass);
                vampObj = vampCls.CallStatic<AndroidJavaObject>("getVampInstance", activity, placementID);
                vampObj.Call("setVAMPListener", new AdListener());
                vampObj.Call("setAdvancedListner", new AdvListener());
            }
        }
        #endif
    }

    /// <summary>
    /// 広告をロードします
    /// </summary>
    public static void load()
    {
        #if UNITY_IPHONE
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            VAMPUnityLoad(vampni);
        }
        #elif UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            if (vampObj != null)
            {
                vampObj.Call("load");
            }
        }
        #endif
    }

    /// <summary>
    /// 広告を表示します
    /// </summary>
    public static bool show()
    {
        bool ret = false;

        #if UNITY_IPHONE
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            ret = VAMPUnityShow(vampni);
        }
        #elif UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            if (vampObj != null)
            {
                ret = vampObj.Call<bool>("show");
            }
        }
        #endif

        return ret;
    }

    /// <summary>
    /// 広告の表示が可能ならばtrueを返し、それ以外はfalseを返します
    /// </summary>
    public static bool isReady()
    {
        bool ret = false;

        #if UNITY_IPHONE
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            ret = VAMPUnityIsReady(vampni);
        }
        #elif UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            if (vampObj != null)
            {
                ret = vampObj.Call<bool>("isReady");
            }
        }
        #endif

        return ret;
    }

    /// <summary>
    /// ロード済みの広告を破棄します
    /// </summary>
    public static void clearLoaded()
    {
        #if UNITY_IPHONE
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            VAMPUnityClearLoaded(vampni);
        }
        #elif UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            if (vampObj != null)
            {
                vampObj.Call("clearLoaded");
            }
        }
        #endif
    }

    /// <summary>
    /// アプリ起動時などのタイミングでアドネットワーク側のSDKを初期化しておきたいときに使います
    /// </summary>
    /// <param name="placementID">広告枠ID</param>
    public static void initializeAdnwSDK(string placementID)
    {
        if (placementID == null || placementID.Length <= 0)
        {
            Debug.LogError("PlacementID is not set.");
            return;
        }

        #if UNITY_IPHONE
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            VAMPUnityInitializeAdnwSDK(placementID);
        }
        #elif UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidJavaClass player = new AndroidJavaClass(UnityPlayerClass);
            AndroidJavaObject activity = player.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaClass vampCls = new AndroidJavaClass(VampClass);
            vampCls.CallStatic("initializeAdnwSDK", activity, placementID);
        }
        #endif
    }

    /// <summary>
    /// アプリ起動時などのタイミングでアドネットワーク側のSDKを初期化しておきたいときに使います
    /// </summary>
    /// <param name="placementID">広告枠ID</param>
    /// <param name="state">InitializeState string</param>
    /// <param name="duration">アドネットワークSDKの初期化実行間隔</param>
    public static void initializeAdnwSDK(string placementID, string state, int duration)
    {
        if (placementID == null || placementID.Length <= 0)
        {
            Debug.LogError("PlacementID is not set.");
            return;
        }

        #if UNITY_IPHONE
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            VAMPUnityInitializeAdnwSDKWithConfig(placementID, state, duration);
        }
        #elif UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidJavaClass player = new AndroidJavaClass(UnityPlayerClass);
            AndroidJavaObject activity = player.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaClass vampCls = new AndroidJavaClass(VampClass);
            AndroidJavaClass initStateCls = new AndroidJavaClass("jp.supership.vamp.VAMP$VAMPInitializeState");
            vampCls.CallStatic("initializeAdnwSDK", activity, placementID, initStateCls.GetStatic<AndroidJavaObject>(state), duration);
        }
        #endif
    }

    /// <summary>
    /// trueを指定すると収益が発生しないテスト広告が配信されるようになります。
    /// ストアに申請する際は必ずfalseを設定してください。
    /// デフォルト値はfalseです
    /// </summary>
    /// <param name="testMode"></param>
    public static void setTestMode(bool testMode)
    {
        #if UNITY_IPHONE
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            VAMPUnitySetTestMode(testMode);
        }
        #elif UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidJavaClass vampCls = new AndroidJavaClass(VampClass);
            vampCls.CallStatic("setTestMode", testMode);
        }
        #endif
    }

    /// <summary>
    /// テストモードのときはtrueを返し、それ以外はfalseを返します
    /// </summary>
    public static bool isTestMode()
    {
        bool ret = false;

        #if UNITY_IPHONE
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            ret = VAMPUnityIsTestMode();
        }
        #elif UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidJavaClass vampCls = new AndroidJavaClass(VampClass);
            ret = vampCls.CallStatic<bool>("isTestMode");
        }
        #endif

        return ret;
    }

    /// <summary>
    /// trueを指定するとログを詳細に出力するデバッグモードになります。
    /// デフォルト値はfalseです
    /// </summary>
    /// <param name="debugMode"></param>
    public static void setDebugMode(bool debugMode)
    {
        #if UNITY_IPHONE
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            VAMPUnitySetDebugMode(debugMode);
        }
        #elif UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidJavaClass vampCls = new AndroidJavaClass(VampClass);
            vampCls.CallStatic("setDebugMode", debugMode);
        }
        #endif
    }

    /// <summary>
    /// デバッグモードのときはtrueを返し、それ以外はfalseを返します
    /// </summary>
    public static bool isDebugMode()
    {
        bool ret = false;

        #if UNITY_IPHONE
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            ret = VAMPUnityIsDebugMode();
        }
        #elif UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidJavaClass vampCls = new AndroidJavaClass(VampClass);
            ret = vampCls.CallStatic<bool>("isDebugMode");
        }
        #endif

        return ret;
    }

    /// <summary>
    /// VAMPのSDKバージョンを返します。この返却される値は、Androidの場合はVAMP.jarのバージョン、
    /// iOSの場合はVAMP.frameworkのバージョンになります
    /// </summary>
    public static string SDKVersion()
    {
        string ret = "unknown";

        #if UNITY_IPHONE
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            ret = VAMPUnitySDKVersion();
        }
        #elif UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidJavaClass vampCls = new AndroidJavaClass(VampClass);
            ret = vampCls.CallStatic<string>("SDKVersion");
        }
        #endif

        return ret;
    }

    /// <summary>
    /// VAMP SDKがサポートするOSの最低バージョンを返します。Androidの場合はAPIレベルの返却になります
    /// </summary>
    public static float SupportedOSVersion()
    {
        float ret = 0;

        #if UNITY_IPHONE
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            ret = VAMPUnitySupportedOSVersion();
        }
        #elif UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidJavaClass vampCls = new AndroidJavaClass(VampClass);
            ret = vampCls.CallStatic<int>("SupportedOSVersion");
        }
        #endif

        return ret;
    }

    /// <summary>
    /// VAMP SDKがサポートするOSバージョンのときはtrueを返します。サポート外のときはfalseを返します
    /// </summary>
    public static bool isSupportedOSVersion()
    {
        bool ret = false;

        #if UNITY_IPHONE
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            ret = VAMPUnityIsSupportedOSVersion();
        }
        #elif UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidJavaClass vampCls = new AndroidJavaClass(VampClass);
            ret = vampCls.CallStatic<bool>("isSupportedOSVersion");
        }
        #endif

        return ret;
    }

    /// <summary>
    /// アドネットワーク側の広告取得を待つタイムアウト時間を秒単位で指定します
    /// </summary>
    /// <param name="timeout">単位:秒</param>
    public static void setMediationTimeout(int timeout)
    {
        #if UNITY_IPHONE
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            VAMPUnitySetMediationTimeout(timeout);
        }
        #elif UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidJavaClass vampCls = new AndroidJavaClass(VampClass);
            vampCls.CallStatic("setMediationTimeout", timeout);
        }
        #endif
    }

    /// <summary>
    /// 2文字の国コード(JP, USなど)を取得します。IPから国を判別できなかった、リクエストがタイムアウトしたなど、
    /// 正常に値が返せないケースは"99"が返却されます。
    /// 結果はVAMPCountryCodeメソッドを通じて返却されます
    /// </summary>
    /// <param name="obj">GameObject</param>
    public static void getCountryCode(GameObject obj)
    {
        countryCodeObj = obj;

        #if UNITY_IPHONE
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            VAMPUnityGetCountryCode(countryCodeObj.name);
        }
        #elif UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidJavaClass player = new AndroidJavaClass(UnityPlayerClass);
            AndroidJavaObject activity = player.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaClass vampCls = new AndroidJavaClass(VampClass);
            vampCls.CallStatic("getCountryCode", activity, new GetCountryCodeListener());
        }
        #endif
    }

    /// <summary>
    /// ユーザ属性を指定します
    /// </summary>
    /// <param name="targeting"></param>
    public static void setTargeting(Targeting targeting)
    {
        #if UNITY_IPHONE
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            VAMPUnitySetTargeting((int)targeting.Gender, 
                targeting.Birthday.Year, targeting.Birthday.Month, targeting.Birthday.Day);
        }
        #elif UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidJavaObject user = new AndroidJavaObject("jp.supership.vamp.VAMPTargeting");

            AndroidJavaClass genderCls = new AndroidJavaClass("jp.supership.vamp.VAMPTargeting$Gender");
            user.Call<AndroidJavaObject>("setGender", genderCls.GetStatic<AndroidJavaObject>(targeting.Gender.ToString()));

            AndroidJavaObject calendar = new AndroidJavaObject("java.util.GregorianCalendar", 
                                             targeting.Birthday.Year, targeting.Birthday.Month, targeting.Birthday.Day);
            user.Call<AndroidJavaObject>("setBirthday", calendar.Call<AndroidJavaObject>("getTime"));

            AndroidJavaClass vampCls = new AndroidJavaClass(VampClass);
            vampCls.CallStatic("setTargeting", user);
        }
        #endif
    }

    #if UNITY_ANDROID
    
    private static string JoinParams(params string[] args)
    {
        if (args.Length <= 0)
        {
            return "";
        }

        return string.Join(",", args);
    }

    class GetCountryCodeListener : AndroidJavaProxy
    {

        public GetCountryCodeListener()
            : base("jp.supership.vamp.VAMPGetCountryCodeListener")
        {
        }

        public override AndroidJavaObject Invoke(string methodName, object[] javaArgs)
        {
            switch (methodName)
            {
                case "onCountryCode":
                    // javaArgs[0]:isoCode
                    countryCodeObj.SendMessage("VAMPCountryCode", JoinParams((string)javaArgs[0]));
                    break;
                default:
                    return base.Invoke(methodName, javaArgs);
            }

            return null;
        }
    }

    class AdListener : AndroidJavaProxy
    {

        public AdListener()
            : base("jp.supership.vamp.VAMPListener")
        {
        }

        public override AndroidJavaObject Invoke(string methodName, object[] javaArgs)
        {
            switch (methodName)
            {
                case "onReceive":
                // javaArgs[0]:placementId
                // javaArgs[1]:adnwName
                    messageObj.SendMessage("VAMPDidReceive",
                        JoinParams((string)javaArgs[0], (string)javaArgs[1]));
                    break;
                case "onFail":
                // javaArgs[0]:placementId
                // javaArgs[1]:error
                    messageObj.SendMessage("VAMPDidFail",
                        JoinParams((string)javaArgs[0], ((AndroidJavaObject)javaArgs[1]).Call<string>("name")));
                    break;
                case "onComplete":
                // javaArgs[0]:placementId
                // javaArgs[1]:adnwName
                    messageObj.SendMessage("VAMPDidComplete",
                        JoinParams((string)javaArgs[0], (string)javaArgs[1]));
                    break;
                case "onClose":
                // javaArgs[0]:placementId
                // javaArgs[1]:adnwName
                    messageObj.SendMessage("VAMPDidClose",
                        JoinParams((string)javaArgs[0], (string)javaArgs[1]));
                    break;
                case "onExpired":
                // javaArgs[0]:placementId
                    messageObj.SendMessage("VAMPDidExpired",
                        JoinParams((string)javaArgs[0]));
                    break;
                default:
                    return base.Invoke(methodName, javaArgs);
            }

            return null;
        }
    }

    class AdvListener : AndroidJavaProxy
    {

        public AdvListener()
            : base("jp.supership.vamp.AdvancedListener")
        {
        }

        public override AndroidJavaObject Invoke(string methodName, object[] javaArgs)
        {
            switch (methodName)
            {
                case "onLoadStart":
                // javaArgs[0]:placementId
                // javaArgs[1]:adnwName
                    messageObj.SendMessage("VAMPLoadStart",
                        JoinParams((string)javaArgs[0], (string)javaArgs[1]));
                    break;
                case "onLoadResult":
                // javaArgs[0]:placementId
                // javaArgs[1]:success
                // javaArgs[2]:adnwName
                // javaArgs[3]:message
                    messageObj.SendMessage("VAMPLoadResult",
                        JoinParams((string)javaArgs[0], ((bool)javaArgs[1]).ToString(), (string)javaArgs[2], (string)javaArgs[3]));
                    break;
                default:
                    return base.Invoke(methodName, javaArgs);
            }

            return null;
        }
    }

    #endif  // end UNITY_ANDROID

    public enum Gender
    {
        /// <summary>
        /// 性別不明
        /// </summary>
        UNKNOWN = 0,
        /// <summary>
        /// 男性
        /// </summary>
        MALE,
        /// <summary>
        /// 女性
        /// </summary>
        FEMALE
    }

    public class Targeting
    {

        public Gender Gender { get; set; }

        public Birthday Birthday { get; set; }

        public Targeting()
        {
            Gender = Gender.UNKNOWN;
            Birthday = new Birthday();
        }
    }

    public class Birthday
    {

        public int Year { get; }

        public int Month { get; }

        public int Day { get; }

        public Birthday()
        {
            Year = 0;
            Month = 0;
            Day = 0;
        }

        /// <summary>
        /// ユーザの誕生日を指定います
        /// </summary>
        /// <param name="year">誕生日 年(西暦)</param>
        /// <param name="month">誕生日 月</param>
        /// <param name="day">誕生日 日</param>
        public Birthday(int year, int month, int day)
        {
            Year = year;
            Month = month;
            Day = day;
        }
    }

    public class MessageUtil
    {

        public static string[] ParseMessage(string msg)
        {
            if (msg != null && msg.Length > 0)
            {
                return msg.Split(',');
            }

            return null;
        }
    }

    public class SDKUtil
    {

        public static string GetAdnwSDKVersion(string adnw)
        {
            string ret = "unknown";

            #if UNITY_IPHONE
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                ret = VAMPUnityAdnwSDKVersion(adnw);
            }
            #elif UNITY_ANDROID
            // Nothing to do
            #endif

            return ret;
        }
    }

    public class DeviceUtil
    {

        public static string GetInfo(string infoName)
        {
            string ret = "unknown";

            #if UNITY_IPHONE
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                ret = VAMPUnityDeviceInfo(infoName);
            }
            #elif UNITY_ANDROID
            // Nothing to do
            #endif

            return ret;
        }
    }
}
