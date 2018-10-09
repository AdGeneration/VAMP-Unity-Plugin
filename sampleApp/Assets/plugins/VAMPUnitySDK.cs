using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

/// <summary>
///
/// VAMP-Unity-Plugin
///
/// Created by AdGeneratioin.
/// Copyright 2018 Supership Inc. All rights reserved.
///
/// </summary>
public class VAMPUnitySDK : MonoBehaviour
{
#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern IntPtr VAMPUnityInitialize(string placementId);

    [DllImport("__Internal")]
    private static extern IntPtr VAMPUnityInit(IntPtr vampni, string placementId, string gameObjName);

    [DllImport("__Internal")]
    private static extern void VAMPUnityPreload(IntPtr vampni);

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
    private static extern void VAMPUnityGetCountryCode2(GetCountryCodeCallback callback);

    [DllImport("__Internal")]
    private static extern void VAMPUnityIsEUAccess(string gameObjName);

    [DllImport("__Internal")]
    private static extern void VAMPUnityIsEUAccess2(IsEUAccessCallback callback);

    [DllImport("__Internal")]
    private static extern void VAMPUnitySetUserConsent(int consentStatus);

    [DllImport("__Internal")]
    private static extern void VAMPUnitySetTargeting(int gender, int birthYear, int birthMonth, int birthDay);

    [DllImport("__Internal")]
    private static extern string VAMPUnityDeviceInfo(string infoName);

    [DllImport("__Internal")]
    private static extern void VAMPUnitySetCallbacks(IntPtr vampni,
                                                     InternalReceiveCallback receiveCallback,
                                                     InternalFailToLoadCallback failToLoadCallback,
                                                     InternalFailToShowCallback failToShowCallback,
                                                     InternalCompleteCallback completeCallback,
                                                     InternalCloseCallback closeCallback,
                                                     InternalExpireCallback expireCallback,
                                                     InternalLoadStartCallback loadStartCallback,
                                                     InternalLoadResultCallback loadResultCallback
    );

    [DllImport("__Internal")]
    private static extern void VAMPUnitySetFrequencyCap(string placementId, uint impressions, uint minutes);

    [DllImport("__Internal")]
    private static extern void VAMPUnityClearFrequencyCap(string placementId);

    [DllImport("__Internal")]
    private static extern bool VAMPUnityIsFrequencyCapped(string placementId);

    [DllImport("__Internal")]
    private static extern IntPtr VAMPUnityGetFrequencyCappedStatus(string placementId);

    [DllImport("__Internal")]
    private static extern void VAMPUnityResetFrequencyCap(string placementId);

    [DllImport("__Internal")]
    private static extern void VAMPUnityResetFrequencyCapAll();

    [DllImport("__Internal")]
    private static extern IntPtr VAMPUnityVAMPConfigurationDefaultConfiguration();

    [DllImport("__Internal")]
    private static extern bool VAMPUnityVAMPConfigurationIsPlayerCancelable(HandleRef config);

    [DllImport("__Internal")]
    private static extern void VAMPUnityVAMPConfigurationSetPlayerCancelable(HandleRef config, bool playerCancelable);

    [DllImport("__Internal")]
    private static extern string VAMPUnityVAMPConfigurationGetPlayerAlertTitleText(HandleRef config);

    [DllImport("__Internal")]
    private static extern void VAMPUnityVAMPConfigurationSetPlayerAlertTitleText(HandleRef config, string title);

    [DllImport("__Internal")]
    private static extern string VAMPUnityVAMPConfigurationGetPlayerAlertBodyText(HandleRef config);

    [DllImport("__Internal")]
    private static extern void VAMPUnityVAMPConfigurationSetPlayerAlertBodyText(HandleRef config, string body);

    [DllImport("__Internal")]
    private static extern string VAMPUnityVAMPConfigurationGetPlayerAlertCloseButtonText(HandleRef config);

    [DllImport("__Internal")]
    private static extern void VAMPUnityVAMPConfigurationSetPlayerAlertCloseButtonText(HandleRef config, string buttonText);

    [DllImport("__Internal")]
    private static extern string VAMPUnityVAMPConfigurationGetPlayerAlertContinueButtonText(HandleRef config);

    [DllImport("__Internal")]
    private static extern void VAMPUnityVAMPConfigurationSetPlayerAlertContinueButtonText(HandleRef config, string buttonText);

    [DllImport("__Internal")]
    private static extern bool VAMPUnityVAMPFrequencyCappedStatusIsCapped(HandleRef frequencyCappedStatus);

    [DllImport("__Internal")]
    private static extern uint VAMPUnityVAMPFrequencyCappedStatusImpressionLimit(HandleRef frequencyCappedStatus);

    [DllImport("__Internal")]
    private static extern uint VAMPUnityVAMPFrequencyCappedStatusTimeLimit(HandleRef frequencyCappedStatus);

    [DllImport("__Internal")]
    private static extern uint VAMPUnityVAMPFrequencyCappedStatusImpressions(HandleRef frequencyCappedStatus);

    [DllImport("__Internal")]
    private static extern uint VAMPUnityVAMPFrequencyCappedStatusRemainingTime(HandleRef frequencyCappedStatus);

    [DllImport("__Internal")]
    private static extern uint VAMPUnityDeleteVAMPFrequencyCappedStatus(HandleRef handle);
#endif

    public enum InitializeState
    {
        /// <summary>
        /// 接続環境によって、WEIGHTとALL設定を自動的に切り替えます
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

    public enum ConsentStatus
    {
        /// <summary>
        /// ユーザの同意が不明です(デフォルト)
        /// </summary>
        UNKNOWN = 0,
        /// <summary>
        /// ユーザは同意しています
        /// </summary>
        ACCEPTED,
        /// <summary>
        /// ユーザは拒否しています
        /// </summary>
        DENIED
    }

#if UNITY_ANDROID
    private const string VampClass = "jp.supership.vamp.VAMP";
    private const string UnityPlayerClass = "com.unity3d.player.UnityPlayer";
#endif

#if UNITY_IOS
    private static IntPtr vampni = IntPtr.Zero;


#elif UNITY_ANDROID
    private static AndroidJavaObject vampObj = null;
#endif

    private static GameObject messageObj = null;
    private static GameObject countryCodeObj = null;
    private static GameObject userConsentObj = null;

    internal delegate void InternalReceiveCallback(string placementId,string adnwName);

    internal delegate void InternalCompleteCallback(string placementId,string adnwName);

    internal delegate void InternalCloseCallback(string placementId,string adnwName);

    internal delegate void InternalLoadStartCallback(string placementId,string adnwName);

    internal delegate void InternalLoadResultCallback(string placementId,bool success,string adnwName,string message);

    internal delegate void InternalExpireCallback(string placementId);

    internal delegate void InternalFailToLoadCallback(int errorCode,string placementId);

    internal delegate void InternalFailToShowCallback(int errorCode,string placementId);

    /// <summary>
    /// getCountryCode コールバック.
    /// </summary>
    public delegate void GetCountryCodeCallback(string countryCode);

    /// <summary>
    /// isEUAccess コールバック.
    /// </summary>
    public delegate void IsEUAccessCallback(bool access);

    private static IVAMPListener listener;
    private static IVAMPAdvancedListener advancedListener;
    private static GetCountryCodeCallback getCountryCodeCallback;
    private static IsEUAccessCallback isEUAccessCallback;
    private static readonly object lockObject = new object();

    public static string VAMPUnityPluginVersion
    {
        get
        {
            return "3.0.5";
        }
    }

    /// <summary>
    /// VAMPUnitySDKクラスの準備を行います
    /// </summary>
    /// <param name="placementID">広告枠ID</param>
    public static void initialize(string placementID)
    {
        Debug.Log("VAMP-Unity-Plugin version: " + VAMPUnityPluginVersion);

        if (string.IsNullOrEmpty(placementID))
        {
            Debug.LogError("PlacementID is not set.");
            return;
        }
#if UNITY_IOS
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            vampni = VAMPUnityInitialize(placementID);
            VAMPUnitySetCallbacks(vampni,
                                  VAMPDidReceive,
                                  VAMPDidFailToLoad,
                                  VAMPDidFailToShow,
                                  VAMPDidComplete,
                                  VAMPDidClose,
                                  VAMPDidExpire,
                                  VAMPDidLoadStart,
                                  VAMPDidLoadResult);
        }
#elif UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            if (vampObj != null) {
                vampObj.Dispose();
            }

            using(var player = new AndroidJavaClass(UnityPlayerClass))
            {
                using(var activity = player.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    using(var vampCls = new AndroidJavaClass(VampClass))
                    {
                        vampObj = vampCls.CallStatic<AndroidJavaObject>("getVampInstance", activity, placementID);
                        vampObj.Call("setVAMPListener", new AdListener());
                        vampObj.Call("setAdvancedListener", new AdvListener());
                    }
                }
            }
        }
#endif
    }

    /// <summary>
    /// VAMPUnitySDKクラスの準備を行います
    /// </summary>
    /// <param name="obj">GameObject</param>
    /// <param name="placementID">広告枠ID</param>
    [Obsolete("Deprecated")]
    public static void initVAMP(GameObject obj, string placementID)
    {
        Debug.Log("VAMP-Unity-Plugin version: " + VAMPUnityPluginVersion);

        if (placementID == null || placementID.Length <= 0)
        {
            Debug.LogError("PlacementID is not set.");
            return;
        }

        messageObj = obj;

#if UNITY_IOS
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            vampni = VAMPUnityInit(vampni, placementID, messageObj.name);

        }
#elif UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            if (vampObj == null)
            {
                using(var player = new AndroidJavaClass(UnityPlayerClass))
                {
                    using(var activity = player.GetStatic<AndroidJavaObject>("currentActivity"))
                    {
                        using(var vampCls = new AndroidJavaClass(VampClass))
                        {
                            vampObj = vampCls.CallStatic<AndroidJavaObject>("getVampInstance", activity, placementID);
                            vampObj.Call("setVAMPListener", new AdListener());
                            vampObj.Call("setAdvancedListener", new AdvListener());
                        }
                    }
                }
            }
        }
#endif
    }

    /// <summary>
    /// リスナーをセットします
    /// </summary>
    /// <param name="listener">Listener.</param>
    public static void setVAMPListener(IVAMPListener listener)
    {
        lock (lockObject)
        {
            VAMPUnitySDK.listener = listener;
        }
    }

    /// <summary>
    /// リスナーをセットします
    /// </summary>
    /// <param name="listener">Listener.</param>
    public static void setAdvancedListener(IVAMPAdvancedListener listener)
    {
        lock (lockObject)
        {
            VAMPUnitySDK.advancedListener = listener;
        }
    }

    /// <summary>
    /// 広告をロードします。
    /// ロードする際にVAMPからの通知を受け取る必要がないときに使います
    /// </summary>
    public static void preload()
    {
#if UNITY_IOS
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            VAMPUnityPreload(vampni);
        }
#elif UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            if (vampObj != null)
            {
                vampObj.Call("preload");
            }
        }
#endif
    }

    /// <summary>
    /// 広告をロードします
    /// </summary>
    public static void load()
    {
#if UNITY_IOS
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
        var ret = false;

#if UNITY_IOS
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
        var ret = false;

#if UNITY_IOS
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
    [Obsolete("Deprecated")]
    public static void clearLoaded()
    {
#if UNITY_IOS
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

#if UNITY_IOS
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            VAMPUnityInitializeAdnwSDK(placementID);
        }
#elif UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            using(var player = new AndroidJavaClass(UnityPlayerClass))
            {
                using(var activity = player.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    using(var vampCls = new AndroidJavaClass(VampClass))
                    {
                        vampCls.CallStatic("initializeAdnwSDK", activity, placementID);
                    }
                }
            }
        }
#endif
    }

    /// <summary>
    /// アプリ起動時などのタイミングでアドネットワーク側のSDKを初期化しておきたいときに使います
    /// </summary>
    /// <param name="placementID">広告枠ID</param>
    /// <param name="state">InitializeState string</param>
    /// <param name="duration">アドネットワークSDKの初期化実行間隔</param>
    [Obsolete("Deprecated")]
    public static void initializeAdnwSDK(string placementID, string state, int duration)
    {
        if (placementID == null || placementID.Length <= 0)
        {
            Debug.LogError("PlacementID is not set.");
            return;
        }

#if UNITY_IOS
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            VAMPUnityInitializeAdnwSDKWithConfig(placementID, state, duration);
        }
#elif UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            using(var player = new AndroidJavaClass(UnityPlayerClass))
            {
                using(var activity = player.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    using(var vampCls = new AndroidJavaClass(VampClass))
                    {
                        using(var initStateCls = new AndroidJavaClass("jp.supership.vamp.VAMP$VAMPInitializeState"))
                        {
                            vampCls.CallStatic("initializeAdnwSDK", activity, placementID, initStateCls.GetStatic<AndroidJavaObject>(state), duration);
                        }
                    }
                }
            }
        }
#endif
    }

    /// <summary>
    /// アプリ起動時などのタイミングでアドネットワーク側のSDKを初期化しておきたいときに使います
    /// </summary>
    /// <param name="placementID">広告枠ID</param>
    /// <param name="state">InitializeState string</param>
    /// <param name="duration">アドネットワークSDKの初期化実行間隔</param>
    public static void initializeAdnwSDK(string placementID, InitializeState state, int duration)
    {
        if (string.IsNullOrEmpty(placementID))
        {
            Debug.LogError("PlacementID is not set.");
            return;
        }

#if UNITY_IOS
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            VAMPUnityInitializeAdnwSDKWithConfig(placementID, state.ToString(), duration);
        }
#elif UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            using(var player = new AndroidJavaClass(UnityPlayerClass))
            {
                using(var activity = player.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    using(var vampCls = new AndroidJavaClass(VampClass))
                    {
                        using(var initStateCls = new AndroidJavaClass("jp.supership.vamp.VAMP$VAMPInitializeState"))
                        {
                            vampCls.CallStatic("initializeAdnwSDK", activity, placementID, initStateCls.GetStatic<AndroidJavaObject>(state.ToString()), duration);
                        }
                    }
                }
            }
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
#if UNITY_IOS
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            VAMPUnitySetTestMode(testMode);
        }
#elif UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            using(var vampCls = new AndroidJavaClass(VampClass))
            {
                vampCls.CallStatic("setTestMode", testMode);
            }
        }
#endif
    }

    /// <summary>
    /// テストモードのときはtrueを返し、それ以外はfalseを返します
    /// </summary>
    public static bool isTestMode()
    {
        var ret = false;

#if UNITY_IOS
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            ret = VAMPUnityIsTestMode();
        }
#elif UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            using(var vampCls = new AndroidJavaClass(VampClass))
            {
                ret = vampCls.CallStatic<bool>("isTestMode");
            }
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
#if UNITY_IOS
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            VAMPUnitySetDebugMode(debugMode);
        }
#elif UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            using(var vampCls = new AndroidJavaClass(VampClass))
            {
                vampCls.CallStatic("setDebugMode", debugMode);
            }
        }
#endif
    }

    /// <summary>
    /// デバッグモードのときはtrueを返し、それ以外はfalseを返します
    /// </summary>
    public static bool isDebugMode()
    {
        var ret = false;

#if UNITY_IOS
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            ret = VAMPUnityIsDebugMode();
        }
#elif UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            using(var vampCls = new AndroidJavaClass(VampClass))
            {
                ret = vampCls.CallStatic<bool>("isDebugMode");
            }
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
        var ret = "unknown";

#if UNITY_IOS
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            ret = VAMPUnitySDKVersion();
        }
#elif UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            using(var vampCls = new AndroidJavaClass(VampClass))
            {
                ret = vampCls.CallStatic<string>("SDKVersion");
            }
        }
#endif

        return ret;
    }

    /// <summary>
    /// VAMP SDKがサポートするOSの最低バージョンを返します。Androidの場合はAPIレベルの返却になります
    /// </summary>
    public static float SupportedOSVersion()
    {
        var ret = 0f;

#if UNITY_IOS
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            ret = VAMPUnitySupportedOSVersion();
        }
#elif UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            using(var vampCls = new AndroidJavaClass(VampClass))
            {
                ret = vampCls.CallStatic<int>("SupportedOSVersion");
            }
        }
#endif

        return ret;
    }

    /// <summary>
    /// VAMP SDKがサポートするOSバージョンのときはtrueを返します。サポート外のときはfalseを返します
    /// </summary>
    public static bool isSupportedOSVersion()
    {
        var ret = false;

#if UNITY_IOS
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            ret = VAMPUnityIsSupportedOSVersion();
        }
#elif UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            using(var vampCls = new AndroidJavaClass(VampClass))
            {
                ret = vampCls.CallStatic<bool>("isSupportedOSVersion");
            }
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
#if UNITY_IOS
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            VAMPUnitySetMediationTimeout(timeout);
        }
#elif UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            using(var vampCls = new AndroidJavaClass(VampClass))
            {
                vampCls.CallStatic("setMediationTimeout", timeout);
            }
        }
#endif
    }

    /// <summary>
    /// 2文字の国コード(JP, USなど)を取得します。IPから国を判別できなかった、リクエストがタイムアウトしたなど、
    /// 正常に値が返せないケースは"99"が返却されます。
    /// </summary>
    /// <param name="callback">コールバック</param>
    public static void getCountryCode(GetCountryCodeCallback callback)
    {
        getCountryCodeCallback = callback;

#if UNITY_IOS
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            VAMPUnityGetCountryCode2(VAMPDidGetCountryCode);
        }
#elif UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            using(var player = new AndroidJavaClass(UnityPlayerClass))
            {
                using(var activity = player.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    var vampCls = new AndroidJavaClass(VampClass);

                    vampCls.CallStatic("getCountryCode", activity, new GetCountryCodeListener());
                }
            }
        }
#endif
    }

    /// <summary>
    /// 2文字の国コード(JP, USなど)を取得します。IPから国を判別できなかった、リクエストがタイムアウトしたなど、
    /// 正常に値が返せないケースは"99"が返却されます。
    /// 結果はVAMPCountryCodeメソッドを通じて返却されます
    /// </summary>
    /// <param name="obj">GameObject</param>
    [Obsolete("Deprecated")]
    public static void getCountryCode(GameObject obj)
    {
        countryCodeObj = obj;

#if UNITY_IOS
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            VAMPUnityGetCountryCode(countryCodeObj.name);
        }
#elif UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            using(var player = new AndroidJavaClass(UnityPlayerClass))
            {
                using(var activity = player.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    using(var vampCls = new AndroidJavaClass(VampClass))
                    {
                        vampCls.CallStatic("getCountryCode", activity, new GetCountryCodeListener());
                    }
                }
            }
        }
#endif
    }

    /// <summary>
    /// EU圏内からのアクセスか判定します。
    /// </summary>
    /// <param name="callback">コールバック</param>
    public static void isEUAccess(IsEUAccessCallback callback)
    {
        isEUAccessCallback = callback;

#if UNITY_IOS
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            VAMPUnityIsEUAccess2(VAMPIsEUAccess);
        }
#elif UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            using(var vampCls = new AndroidJavaClass(VampClass))
            {
                vampCls.CallStatic("isEUAccess", new UserConsentListener());
            }
        }
#endif
    }

    /// <summary>
    /// EU圏内からのアクセスか判定します。
    /// 結果はVAMPIsEUAccessメソッドを通じて返却されます
    /// </summary>
    /// <param name="obj">GameObject</param>
    [Obsolete("Deprecated")]
    public static void isEUAccess(GameObject obj)
    {
        userConsentObj = obj;

#if UNITY_IOS
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            VAMPUnityIsEUAccess(userConsentObj.name);
        }
#elif UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            using(var vampCls = new AndroidJavaClass(VampClass))
            {
                vampCls.CallStatic("isEUAccess", new UserConsentListener());
            }
        }
#endif
    }

    /// <summary>
    /// ユーザの同意がある場合はConsentStatus.Acceptedをセットします
    /// </summary>
    /// <param name="status">ConsentStatus</param>
    public static void setUserConsent(ConsentStatus status)
    {
#if UNITY_IOS
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            VAMPUnitySetUserConsent((int)status);
        }
#elif UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            using(var consentStatusCls = new AndroidJavaClass("jp.supership.vamp.VAMPPrivacySettings$ConsentStatus"))
            {
                using(var vampCls = new AndroidJavaClass(VampClass)) 
                {
                    vampCls.CallStatic("setUserConsent", consentStatusCls.GetStatic<AndroidJavaObject>(status.ToString()));
                }
            }
        }
#endif
    }

    /// <summary>
    /// ユーザ属性を指定します
    /// </summary>
    /// <param name="targeting"></param>
    public static void setTargeting(Targeting targeting)
    {
#if UNITY_IOS
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            VAMPUnitySetTargeting((int)targeting.Gender,
                targeting.Birthday.Year, targeting.Birthday.Month, targeting.Birthday.Day);
        }
#elif UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            using(var user = new AndroidJavaObject("jp.supership.vamp.VAMPTargeting"))
            {

                using(var genderCls = new AndroidJavaClass("jp.supership.vamp.VAMPTargeting$Gender"))
                {
                    user.Call<AndroidJavaObject>("setGender", genderCls.GetStatic<AndroidJavaObject>(targeting.Gender.ToString()));

                    using(var calendar = new AndroidJavaObject("java.util.GregorianCalendar",
                        targeting.Birthday.Year, targeting.Birthday.Month - 1, targeting.Birthday.Day))
                    {
                        
                        user.Call<AndroidJavaObject>("setBirthday", calendar.Call<AndroidJavaObject>("getTime"));

                        using(var vampCls = new AndroidJavaClass(VampClass))
                        {
                            vampCls.CallStatic("setTargeting", user);
                        }
                    }
                }
            }
        }
#endif
    }

    /// <summary>
    /// フリークエンシーキャップ機能を設定します。
    /// 指定した時間内に何回広告を表示できるかを設定します。
    /// </summary>
    /// <param name="placementId">フリークエンシーキャップを設定する広告枠ID</param>
    /// <param name="impressions">視聴制限回数</param>
    /// <param name="minutes">視聴回数がリセットされるまでの制限時間</param>
    public static void setFrequencyCap(string placementId, uint impressions, uint minutes)
    {
#if UNITY_IOS && !UNITY_EDITOR
        VAMPUnitySetFrequencyCap(placementId, impressions, minutes);
#elif UNITY_ANDROID && !UNITY_EDITOR
        using(var vampCls = new AndroidJavaClass(VampClass))
        {
            vampCls.CallStatic("setFrequencyCap", placementId, (int)impressions, (int)minutes);
        }
#endif
    }

    /// <summary>
    /// フリークエンシーキャップを解除します。
    /// </summary>
    /// <param name="placementId">フリークエンシーキャップ機能を解除する広告枠ID</param>
    public static void clearFrequencyCap(string placementId)
    {
#if UNITY_IOS && !UNITY_EDITOR
        VAMPUnityClearFrequencyCap(placementId);
#elif UNITY_ANDROID && !UNITY_EDITOR
        using (var vampCls = new AndroidJavaClass(VampClass))
        {
            vampCls.CallStatic("clearFrequencyCap", placementId);
        }
#endif
    }

    /// <summary>
    /// キャップにかかっているかどうか確認します。
    /// </summary>
    /// <returns>キャップにかかっているなら<c>true</c>、 かかっていないなら<c>false</c></returns>
    /// <param name="placementId">キャップにかかっているか確認する広告枠ID</param>
    public static bool isFrequencyCapped(string placementId)
    {
        var ret = false;
#if UNITY_IOS && !UNITY_EDITOR
        ret = VAMPUnityIsFrequencyCapped(placementId);
#elif UNITY_ANDROID && !UNITY_EDITOR
        using (var vampCls = new AndroidJavaClass(VampClass))
        {
            ret = vampCls.CallStatic<bool>("isFrequencyCapped", placementId);
        }
#endif
        return ret;
    }

    /// <summary>
    /// キャップ状況を取得します。
    /// </summary>
    /// <returns>キャップ状況</returns>
    /// <param name="placementId">キャップ状況を取得する広告枠ID</param>
    public static VAMPFrequencyCappedStatus getFrequencyCappedStatus(string placementId)
    {
        VAMPFrequencyCappedStatus status = null;
#if UNITY_IOS && !UNITY_EDITOR
        var cPtr = VAMPUnityGetFrequencyCappedStatus(placementId);
        status = new VAMPFrequencyCappedStatus(cPtr);
#elif UNITY_ANDROID && !UNITY_EDITOR
        using (var vampCls = new AndroidJavaClass(VampClass))
        {
            var fcStatus = vampCls.CallStatic<AndroidJavaObject>("getFrequencyCappedStatus", placementId);
            status = new VAMPFrequencyCappedStatus(fcStatus);
        }
#endif
        return status;
    }

    /// <summary>
    /// 指定した広告枠IDのフリークエンシーキャップの設定を解除します。
    /// </summary>
    /// <param name="placementId">フリークエンシーキャップの設定を解除する広告枠ID</param>
    public static void resetFrequencyCap(string placementId)
    {
#if UNITY_IOS && !UNITY_EDITOR
        VAMPUnityResetFrequencyCap(placementId);
#elif UNITY_ANDROID && !UNITY_EDITOR
        using (var vampCls = new AndroidJavaClass(VampClass))
        {
            using(var player = new AndroidJavaClass(UnityPlayerClass))
            {
                using(var activity = player.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    vampCls.CallStatic("resetFrequencyCap", activity, placementId);
                }
            }
        }
#endif
    }

    /// <summary>
    /// 全ての広告枠IDのフリークエンシーキャップの設定を解除します。
    /// </summary>
    public static void resetFrequencyCapAll()
    {
#if UNITY_IOS && !UNITY_EDITOR
        VAMPUnityResetFrequencyCapAll();
#elif UNITY_ANDROID && !UNITY_EDITOR
        using (var vampCls = new AndroidJavaClass(VampClass))
        {
            using(var player = new AndroidJavaClass(UnityPlayerClass))
            {
                using(var activity = player.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    vampCls.CallStatic("resetFrequencyCapAll", activity);
                }
            }
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
                    if (countryCodeObj)
                    {
                        countryCodeObj.SendMessage("VAMPCountryCode", JoinParams((string)javaArgs[0]));
                    }

                    if (getCountryCodeCallback != null)
                    {
                        getCountryCodeCallback.Invoke((string)javaArgs[0]);
                    }
                    break;
                default:
                    return base.Invoke(methodName, javaArgs);
            }

            return null;
        }
    }

    class UserConsentListener : AndroidJavaProxy
    {

        public UserConsentListener()
            : base("jp.supership.vamp.VAMPPrivacySettings$UserConsentListener")
        {
        }

        public override AndroidJavaObject Invoke(string methodName, object[] javaArgs)
        {
            switch (methodName)
            {
                case "onRequired":
                    // javaArgs[0]:isRequired
                    if (userConsentObj)
                    {
                        userConsentObj.SendMessage("VAMPIsEUAccess", JoinParams(((bool)javaArgs[0]).ToString()));
                    }

                    if (isEUAccessCallback != null)
                    {
                        isEUAccessCallback.Invoke((bool)javaArgs[0]);
                    }
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
                    if (messageObj)
                    {
                        messageObj.SendMessage("VAMPDidReceive",
                            JoinParams((string)javaArgs[0], (string)javaArgs[1]));
                    }

                    if (listener != null)
                    {
                        listener.VAMPDidReceive((string)javaArgs[0], (string)javaArgs[1]);
                    }
                    break;
                case "onFailedToLoad":
                    // javaArgs[0]:error
                    // javaArgs[1]:placementId
                    if (messageObj)
                    {
                        messageObj.SendMessage("VAMPDidFailToLoad",
                            JoinParams(((AndroidJavaObject)javaArgs[0]).Call<string>("name"), (string)javaArgs[1]));
                    }

                    if (listener != null)
                    {
                        var errorCode = (AndroidJavaObject)javaArgs[0];
                        listener.VAMPDidFailToLoad((VAMPError)errorCode.Call<int>("ordinal"), (string)javaArgs[1]);
                    }
                    break;
                case "onFailedToShow":
                    // javaArgs[0]:error
                    // javaArgs[1]:placementId
                    if (messageObj)
                    {
                        messageObj.SendMessage("VAMPDidFailToShow",
                            JoinParams(((AndroidJavaObject)javaArgs[0]).Call<string>("name"), (string)javaArgs[1]));
                    }

                    if (listener != null)
                    {
                        var errorCode = (AndroidJavaObject)javaArgs[0];
                        listener.VAMPDidFailToShow((VAMPError)errorCode.Call<int>("ordinal"), (string)javaArgs[1]);
                    }
                    break;
                case "onFail":  // Deprecated
                                // javaArgs[0]:placementId
                                // javaArgs[1]:error
                    if (messageObj)
                    {
                        messageObj.SendMessage("VAMPDidFail",
                            JoinParams((string)javaArgs[0], ((AndroidJavaObject)javaArgs[1]).Call<string>("name")));
                    }
                    break;
                case "onComplete":
                    // javaArgs[0]:placementId
                    // javaArgs[1]:adnwName
                    if (messageObj)
                    {
                        messageObj.SendMessage("VAMPDidComplete",
                            JoinParams((string)javaArgs[0], (string)javaArgs[1]));
                    }

                    if (listener != null)
                    {
                        listener.VAMPDidComplete((string)javaArgs[0], (string)javaArgs[1]);
                    }
                    break;
                case "onClose":
                    // javaArgs[0]:placementId
                    // javaArgs[1]:adnwName
                    if (messageObj)
                    {
                        messageObj.SendMessage("VAMPDidClose",
                            JoinParams((string)javaArgs[0], (string)javaArgs[1]));
                    }

                    if (listener != null)
                    {
                        listener.VAMPDidClose((string)javaArgs[0], (string)javaArgs[1]);
                    }
                    break;
                case "onExpired":
                    // javaArgs[0]:placementId
                    if (messageObj)
                    {
                        messageObj.SendMessage("VAMPDidExpired",
                            JoinParams((string)javaArgs[0]));
                    }

                    if (listener != null)
                    {
                        listener.VAMPDidExpired((string)javaArgs[0]);
                    }
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
                    if (messageObj)
                    {
                        messageObj.SendMessage("VAMPLoadStart",
                            JoinParams((string)javaArgs[0], (string)javaArgs[1]));
                    }

                    if (advancedListener != null)
                    {
                        advancedListener.VAMPLoadStart((string)javaArgs[0], (string)javaArgs[1]);
                    }
                    break;
                case "onLoadResult":
                    // javaArgs[0]:placementId
                    // javaArgs[1]:success
                    // javaArgs[2]:adnwName
                    // javaArgs[3]:message
                    if (messageObj)
                    {
                        messageObj.SendMessage("VAMPLoadResult",
                            JoinParams((string)javaArgs[0], ((bool)javaArgs[1]).ToString(), (string)javaArgs[2], (string)javaArgs[3]));
                    }
                    if (advancedListener != null)
                    {
                        advancedListener.VAMPLoadResult((string)javaArgs[0], (bool)javaArgs[1], (string)javaArgs[2], (string)javaArgs[3]);
                    }
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
        /// <param name="month">誕生日 月. int型の1~12の範囲で指定します</param>
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

    [Obsolete("Deprecated")]
    public class DeviceUtil
    {

        public static string GetInfo(string infoName)
        {
            var ret = "unknown";

#if UNITY_IOS
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

    /// <summary>
    /// VAMPConfiguration class.
    /// </summary>
    public class VAMPConfiguration
    {
        static VAMPConfiguration instance;

        /// <summary>
        /// VAMPConfigurationインスタンスを取得します
        /// </summary>
        /// <returns>VAMPConfigurationインスタンス</returns>
        public static VAMPConfiguration getInstance()
        {
            if (instance == null)
            {
                instance = new VAMPConfiguration();
            }

            return instance;
        }
            
#if UNITY_IOS && !UNITY_EDITOR
        HandleRef handleRef;
#elif UNITY_ANDROID && !UNITY_EDITOR
        const string VAMPConfigurationClass = "jp.supership.vamp.VAMPConfiguration";
        AndroidJavaObject configObject;
#endif
        private VAMPConfiguration() 
        {
#if UNITY_IOS && !UNITY_EDITOR
            var cPtr = VAMPUnityVAMPConfigurationDefaultConfiguration();
            this.handleRef = new HandleRef(this, cPtr);
#elif UNITY_ANDROID && !UNITY_EDITOR
            using(var configCls = new AndroidJavaClass(VAMPConfigurationClass))
            {
                this.configObject = configCls.CallStatic<AndroidJavaObject>("getInstance");
            }
#endif
        }

        /// <summary>
        /// 動画再生中にキャンセルが可能かどうかを設定します。
        /// この機能は一部のアドネットワークのみ有効です。
        /// </summary>
        /// <value>動画再生中にキャンセルが可能なら<c>true</c>、 そうでないなら<c>false</c>.</value>
        public bool PlayerCancelable
        {
            get {
                var ret = false;
#if UNITY_IOS && !UNITY_EDITOR
                if (handleRef.Handle != IntPtr.Zero)
                {
                    ret = VAMPUnityVAMPConfigurationIsPlayerCancelable(handleRef);
                }
#elif UNITY_ANDROID && !UNITY_EDITOR
                if (configObject != null)
                {
                    ret = configObject.Call<bool>("isPlayerCancelable");
                }
#endif
                return ret;
            }
                
            set {
#if UNITY_IOS && !UNITY_EDITOR
                if (handleRef.Handle != IntPtr.Zero)
                {
                    VAMPUnityVAMPConfigurationSetPlayerCancelable(handleRef, value);
                }
#elif UNITY_ANDROID && !UNITY_EDITOR
                if (configObject != null)
                {
                    configObject.Call("setPlayerCancelable", value);
                }
#endif
            }
        }

        /// <summary>
        /// キャンセル機能が有効の時に表示するアラートダイアログのタイトルを設定します。
        /// この設定は一部のアドネットワークのみ有効です。
        /// </summary>
        /// <value>アラートダイアログのタイトル</value>
        public string PlayerAlertTitleText
        {
            get {
                var ret = string.Empty;
#if UNITY_IOS && !UNITY_EDITOR
                if (handleRef.Handle != IntPtr.Zero)
                {
                    ret = VAMPUnityVAMPConfigurationGetPlayerAlertTitleText(handleRef);
                }
#elif UNITY_ANDROID && !UNITY_EDITOR
                if (configObject != null)
                {
                    ret = configObject.Call<string>("getPlayerAlertTitleText");
                }
#endif
                return ret;
            }

            set {
#if UNITY_IOS && !UNITY_EDITOR
                if (handleRef.Handle != IntPtr.Zero)
                {
                    VAMPUnityVAMPConfigurationSetPlayerAlertTitleText(handleRef, value);
                }
#elif UNITY_ANDROID && !UNITY_EDITOR
                if (configObject != null)
                {
                    configObject.Call("setPlayerAlertTitleText", value);
                }
#endif
            }
        }

        /// <summary>
        /// キャンセル機能が有効の時に表示するアラートダイアログの本文を設定します。
        /// この設定は一部のアドネットワークのみ有効です。
        /// </summary>
        /// <value>アラートダイアログの本文</value>
        public string PlayerAlertBodyText
        {
            get {
                var ret = string.Empty;
#if UNITY_IOS && !UNITY_EDITOR
                if (handleRef.Handle != IntPtr.Zero)
                {
                    ret = VAMPUnityVAMPConfigurationGetPlayerAlertBodyText(handleRef);
                }
#elif UNITY_ANDROID && !UNITY_EDITOR
                if (configObject != null)
                {
                    ret = configObject.Call<string>("getPlayerAlertBodyText");
                }
#endif
                return ret;
            }
            set {
#if UNITY_IOS && !UNITY_EDITOR
                if (handleRef.Handle != IntPtr.Zero)
                {
                    VAMPUnityVAMPConfigurationSetPlayerAlertBodyText(handleRef, value);
                }
#elif UNITY_ANDROID && !UNITY_EDITOR
                if (configObject != null)
                {
                    configObject.Call("setPlayerAlertBodyText", value);
                }
#endif
            }
        }

        /// <summary>
        /// キャンセル機能が有効の時に表示するアラートダイアログの終了ボタンのテキストを設定します。
        /// この設定は一部のアドネットワークのみ有効です。
        /// </summary>
        /// <value>アラートダイアログの終了ボタンのテキスト</value>
        public string PlayerAlertCloseButtonText
        {
            get {
                var ret = string.Empty;
#if UNITY_IOS && !UNITY_EDITOR
                if (handleRef.Handle != IntPtr.Zero)
                {
                    ret = VAMPUnityVAMPConfigurationGetPlayerAlertCloseButtonText(handleRef);
                }
#elif UNITY_ANDROID && !UNITY_EDITOR
                if (configObject != null)
                {
                    ret = configObject.Call<string>("getPlayerAlertCloseButtonText");
                }
#endif
                return ret;
            }
            set {
#if UNITY_IOS && !UNITY_EDITOR
                if (handleRef.Handle != IntPtr.Zero)
                {
                    VAMPUnityVAMPConfigurationSetPlayerAlertCloseButtonText(handleRef, value);
                }
#elif UNITY_ANDROID && !UNITY_EDITOR
                if (configObject != null)
                {
                    configObject.Call("setPlayerAlertCloseButtonText", value);
                }
#endif
            }
        }

        /// <summary>
        /// キャンセル機能が有効の時に表示するアラートダイアログの継続視聴ボタンのテキストを設定します。
        /// この設定は一部のアドネットワークのみ有効です。
        /// </summary>
        /// <value>アラートダイアログの継続視聴ボタンのテキスト</value>
        public string PlayerAlertContinueButtonText
        {
            get {
                var ret = string.Empty;
#if UNITY_IOS && !UNITY_EDITOR
                if (handleRef.Handle != IntPtr.Zero)
                {
                    ret = VAMPUnityVAMPConfigurationGetPlayerAlertContinueButtonText(handleRef);
                }
#elif UNITY_ANDROID && !UNITY_EDITOR
                if (configObject != null)
                {
                    ret = configObject.Call<string>("getPlayerAlertContinueButtonText");
                }
#endif
                return ret;
            }
            set {
#if UNITY_IOS && !UNITY_EDITOR
                if (handleRef.Handle != IntPtr.Zero)
                {
                    VAMPUnityVAMPConfigurationSetPlayerAlertContinueButtonText(handleRef, value);
                }
#elif UNITY_ANDROID && !UNITY_EDITOR
                if (configObject != null)
                {
                    configObject.Call("setPlayerAlertContinueButtonText", value);
                }
#endif
            }
        }
    }

    /// <summary>
    /// フリークエンシーキャップの状況を表すクラス
    /// </summary>
    public class VAMPFrequencyCappedStatus : IDisposable
    {
#if UNITY_IOS && !UNITY_EDITOR
        private HandleRef handleRef;
        public VAMPFrequencyCappedStatus(IntPtr cPtr) 
        {
            handleRef = new HandleRef(this, cPtr);
        }

#elif UNITY_ANDROID && !UNITY_EDITOR
        private AndroidJavaObject javaObject;
        public VAMPFrequencyCappedStatus(AndroidJavaObject javaObject)
        {
            this.javaObject = javaObject;
        }
#endif
        ~VAMPFrequencyCappedStatus()
        {
            Dispose();
        }

        public void Dispose()
        {
            lock (this)
            {
#if UNITY_IOS && !UNITY_EDITOR
                if (handleRef.Handle != IntPtr.Zero)
                {
                    VAMPUnityDeleteVAMPFrequencyCappedStatus(handleRef);
                    handleRef = new HandleRef(null, IntPtr.Zero);
                    GC.SuppressFinalize(this);
                }
#elif UNITY_ANDROID && !UNITY_EDITOR
                if (javaObject != null) 
                {
                    javaObject.Dispose();
                    javaObject = null;
                    GC.SuppressFinalize(this);
                }
#endif
            }
        }

        /// <summary>
        /// キャップにかかっているかどうかを確認します
        /// </summary>
        /// <value>キャップにかかっているなら<c>true</c>、かかっていないなら<c>false</c>.</value>
        public bool IsCapped
        {
            get {
                var ret = false;
#if UNITY_IOS && !UNITY_EDITOR
                if (handleRef.Handle != IntPtr.Zero)
                {
                    ret = VAMPUnityVAMPFrequencyCappedStatusIsCapped(handleRef); 
                }
                else
                {
                    throw new ObjectDisposedException("");
                }
#elif UNITY_ANDROID && !UNITY_EDITOR
                if (javaObject != null)
                {
                    ret = javaObject.Call<bool>("isCapped");
                }
                else
                {
                    throw new ObjectDisposedException("");
                }
#endif
                return ret;
    
            }
        }

        /// <summary>
        /// 現在設定されている視聴制限回数を取得します
        /// </summary>
        /// <value>視聴制限回数</value>
        public uint ImpressionLimit
        {
            get { 
                var ret = 0u;
#if UNITY_IOS && !UNITY_EDITOR
                if (handleRef.Handle != IntPtr.Zero)
                {
                    ret = VAMPUnityVAMPFrequencyCappedStatusImpressionLimit(handleRef);
                }
                else
                {
                    throw new ObjectDisposedException("");
                }
#elif UNITY_ANDROID && !UNITY_EDITOR
                if (javaObject != null)
                {
                    ret = (uint)Math.Max(javaObject.Call<int>("getImpressionLimit"), 0);
                }
                else
                {
                    throw new ObjectDisposedException("");
                }
#endif
                return ret;
            }
        }

        /// <summary>
        /// 現在設定されている、視聴回数がリセットされるまでの時間制限を取得します
        /// </summary>
        /// <value>視聴回数がリセットされるまでの時間制限</value>
        public uint TimeLimit
        {
            get {
                var ret = 0u;
#if UNITY_IOS && !UNITY_EDITOR
                if (handleRef.Handle != IntPtr.Zero)
                {
                    ret = VAMPUnityVAMPFrequencyCappedStatusTimeLimit(handleRef); 
                }
                else
                {
                    throw new ObjectDisposedException("");
                }
#elif UNITY_ANDROID && !UNITY_EDITOR
                if (javaObject != null)
                {
                    ret = (uint)Math.Max(javaObject.Call<int>("getTimeLimit"), 0);
                }
                else
                {
                    throw new ObjectDisposedException("");
                }
#endif
                return ret;
            }
        }

        /// <summary>
        /// 現在の視聴回数を取得します
        /// </summary>
        /// <value>視聴回数</value>
        public uint Impressions
        {
            get { 
                var ret = 0u;
#if UNITY_IOS && !UNITY_EDITOR
                if (handleRef.Handle != IntPtr.Zero)
                {
                    ret = VAMPUnityVAMPFrequencyCappedStatusImpressions(handleRef);
                }
                else
                {
                    throw new ObjectDisposedException("");
                }
#elif UNITY_ANDROID && !UNITY_EDITOR
                if (javaObject != null)
                {
                    ret = (uint)Math.Max(javaObject.Call<int>("getImpressions"), 0);
                }
                else
                {
                    throw new ObjectDisposedException("");
                }
#endif
                return ret;
            }
        }

        public uint RemainingTime
        {
            get {
                var ret = 0u;
#if UNITY_IOS && !UNITY_EDITOR
                if (handleRef.Handle != IntPtr.Zero)
                {
                    ret = VAMPUnityVAMPFrequencyCappedStatusRemainingTime(handleRef);
                }
                else
                {
                    throw new ObjectDisposedException("");
                }
#elif UNITY_ANDROID && !UNITY_EDITOR
                if (javaObject != null)
                {
                    ret = (uint)Math.Max(javaObject.Call<int>("getRemainingTime"), 0);
                }
                else
                {
                    throw new ObjectDisposedException("");
                }
#endif
                return ret;
            }
        }
    }

#if UNITY_IOS
    [AOT.MonoPInvokeCallback(typeof(InternalReceiveCallback))]
    static void VAMPDidReceive(string placementId, string adnwName)
    {
        if (listener != null)
        {
            listener.VAMPDidReceive(placementId, adnwName);
        }
    }

    [AOT.MonoPInvokeCallback(typeof(InternalFailToLoadCallback))]
    static void VAMPDidFailToLoad(int errorCode, string placementId)
    {
        if (listener != null)
        {
            listener.VAMPDidFailToLoad((VAMPError)errorCode, placementId);
        }
    }

    [AOT.MonoPInvokeCallback(typeof(InternalFailToShowCallback))]
    static void VAMPDidFailToShow(int errorCode, string placementId)
    {
        if (listener != null)
        {
            listener.VAMPDidFailToShow((VAMPError)errorCode, placementId);
        }
    }

    [AOT.MonoPInvokeCallback(typeof(InternalCompleteCallback))]
    static void VAMPDidComplete(string placementId, string adnwName)
    {
        if (listener != null)
        {
            listener.VAMPDidComplete(placementId, adnwName);
        }
    }

    [AOT.MonoPInvokeCallback(typeof(InternalCloseCallback))]
    static void VAMPDidClose(string placementId, string adnwName)
    {
        if (listener != null)
        {
            listener.VAMPDidClose(placementId, adnwName);
        }
    }

    [AOT.MonoPInvokeCallback(typeof(InternalLoadStartCallback))]
    static void VAMPDidLoadStart(string placementId, string adnwName)
    {
        if (advancedListener != null)
        {
            advancedListener.VAMPLoadStart(placementId, adnwName);
        }
    }

    [AOT.MonoPInvokeCallback(typeof(InternalLoadResultCallback))]
    static void VAMPDidLoadResult(string placementId, bool success, string adnwName, string message)
    {
        if (advancedListener != null)
        {
            advancedListener.VAMPLoadResult(placementId, success, adnwName, message);
        }
    }

    [AOT.MonoPInvokeCallback(typeof(InternalExpireCallback))]
    static void VAMPDidExpire(string placementId)
    {
        if (listener != null)
        {
            listener.VAMPDidExpired(placementId);
        }
    }

    [AOT.MonoPInvokeCallback(typeof(GetCountryCodeCallback))]
    static void VAMPDidGetCountryCode(string countryCode)
    {
        if (getCountryCodeCallback != null)
        {
            getCountryCodeCallback.Invoke(countryCode);
        }
    }

    [AOT.MonoPInvokeCallback(typeof(IsEUAccessCallback))]
    static void VAMPIsEUAccess(bool access)
    {
        if (isEUAccessCallback != null)
        {
            isEUAccessCallback.Invoke(access);
        }
    }
#endif

    /// <summary>
    /// IVAMPListener interface.
    /// </summary>
    public interface IVAMPListener
    {
        /// <summary>
        /// ロードが完了し、広告表示できる状態になった時に通知されます。
        /// </summary>
        /// <param name="placementId">広告枠ID</param>
        /// <param name="adnwName">アドネットワーク名</param>
        void VAMPDidReceive(string placementId, string adnwName);

        /// <summary>
        /// 広告のロード時にエラーが発生した時に通知されます。
        /// </summary>
        /// <param name="error">Error</param>
        /// <param name="placementId">広告枠ID</param>
        void VAMPDidFailToLoad(VAMPError error, string placementId);

        /// <summary>
        /// 広告の表示時にエラーが発生した時に通知されます。
        /// </summary>
        /// <param name="error">Error</param>
        /// <param name="placementId">広告枠ID</param>
        void VAMPDidFailToShow(VAMPError error, string placementId);

        /// <summary>
        /// インセンティブ付与可能になったタイミングで通知されます。
        /// </summary>
        /// <param name="placementId">広告枠ID</param>
        /// <param name="adnwName">アドネットワーク名</param>
        void VAMPDidComplete(string placementId, string adnwName);

        /// <summary>
        /// 広告が閉じられた時に通知されます。
        /// ユーザキャンセルなども含まれるのため、インセンティブ付与はVAMPDidCompleteで判定してください。
        /// </summary>
        /// <param name="placementId">広告枠ID</param>
        /// <param name="adnwName">アドネットワーク名</param>
        void VAMPDidClose(string placementId, string adnwName);

        /// <summary>
        /// 広告準備完了から55分経つと取得した広告の表示はできてもRTBの収益は発生しません。
        /// この通知を受け取ったら、もう一度Loadからやり直す必要があります。
        /// </summary>
        /// <param name="placementId">広告枠ID</param>
        void VAMPDidExpired(string placementId);
    }

    /// <summary>
    /// IVAMPAdvancedListener interface.
    /// </summary>
    public interface IVAMPAdvancedListener
    {
        /// <summary>
        /// アドネットワークごとの広告取得が開始された時に通知されます。
        /// </summary>
        /// <param name="placementId">広告枠ID</param>
        /// <param name="adnwName">アドネットワーク名</param>
        void VAMPLoadStart(string placementId, string adnwName);

        /// <summary>
        /// アドネットワークごとの広告取得結果が通知されます(成功/失敗どちらも通知)。
        /// </summary>
        /// <param name="placementId">広告枠ID</param>
        /// <param name="success"><c>true</c>なら成功</param>
        /// <param name="adnwName">アドネットワーク名</param>
        /// <param name="message">メッセージ</param>
        void VAMPLoadResult(string placementId, bool success, string adnwName, string message);
    }

    /// <summary>
    /// VAMP側で発生するエラーの定義
    /// </summary>
    public enum VAMPError
    {
        /// <summary>
        /// 非対応OSバージョン
        /// </summary>
        NOT_SUPPORTED_OS_VERSION,

        /// <summary>
        /// 不明なエラー
        /// </summary>
        UNKNOWN,

        /// <summary>
        /// サーバー間通信エラー
        /// </summary>
        SERVER_ERROR,

        /// <summary>
        /// 配信可能なアドネットワークがない
        /// </summary>
        NO_ADNETWORK,

        /// <summary>
        /// 通信不通
        /// </summary>
        NEED_CONNECTION,

        /// <summary>
        /// タイムアウト
        /// </summary>
        MEDIATION_TIMEOUT,

        /// <summary>
        /// ユーザ都合の途中終了
        /// </summary>
        USER_CANCEL,

        /// <summary>
        /// 広告在庫無し
        /// </summary>
        NO_ADSTOCK,

        /// <summary>
        /// アドネットワークにてエラーが発生
        /// </summary>
        ADNETWORK_ERROR,

        /// <summary>
        /// 設定エラー
        /// </summary>
        SETTING_ERROR,

        /// <summary>
        /// 広告のロードが完了していないときに表示しようとした
        /// </summary>
        NOT_LOADED,

        /// <summary>
        /// パラメータが不正
        /// </summary>
        INVALID_PARAMETER,

        /// <summary>
        /// キャップにかかっている
        /// </summary>
        FREQUENCY_CAPPED
    }
}
