using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

public class  VAMPUnitySDK : VAMPMonoBehaviour {

    public enum InitializeState {
        AUTO = 0,
        WEIGHT,
        ALL,
        WIFIONLY
    }

	//パラメータ
	private static GameObject messageObj = null;
    private static GameObject countryCodeObj = null;
	private static bool isEnableTest = false;
	private static bool isEnableDebug = false;
	private static string placementID = "";
	//パラメータ

	#if UNITY_IPHONE
	private static IntPtr vampni = IntPtr.Zero;
	#elif UNITY_ANDROID
	private const string VAMP_CLASS = "jp.supership.vamp.VAMP";
	private const string UNITYPLAYER_CLASS = "com.unity3d.player.UnityPlayer";

	private static AndroidJavaObject vampObj = null;
	#endif

	private static VAMPUnitySDK myInstance;

	private static bool noInstance {
		get {
            return myInstance == null || Application.isEditor;
		}
	}

	public static void initVAMP(GameObject obj, string _placementID) {
		messageObj = obj;
		placementID = _placementID;
		initVAMPCommon();
		#if UNITY_IPHONE
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			vampni = _initVAMP(vampni, placementID, isEnableTest, isEnableDebug, messageObj.name);
		}
		#elif UNITY_ANDROID
		if (Application.platform == RuntimePlatform.Android) {
			if (placementID != null && placementID.Length > 0) {
				if (vampObj == null) {
					AndroidJavaClass player = new AndroidJavaClass(UNITYPLAYER_CLASS);
					AndroidJavaObject activity = player.GetStatic<AndroidJavaObject>("currentActivity");
					AndroidJavaClass vampCls = new AndroidJavaClass(VAMP_CLASS);
					vampObj = vampCls.CallStatic<AndroidJavaObject>("getVampInstance", activity, placementID);
					vampObj.Call("setVAMPListener", new AdListener());
					vampObj.Call("setAdvancedListner", new AdvListener());
				}
			} else {
				Debug.LogError("PlacementID is not set.");
			}
		}
		#endif
	}

	private static void initVAMPCommon() {
		if (myInstance == null) {
			GameObject gameObject = new GameObject("VAMPForUnity");
			DontDestroyOnLoad(gameObject);//Makes the object target not be destroyed automatically when loading a new scene.
			gameObject.hideFlags = HideFlags.HideAndDontSave;//A combination of not shown in the hierarchy and not saved to to scenes.
			myInstance = gameObject.AddComponent<VAMPUnitySDK>();
		}
	}

	//ここからVAMPメソッド--------------
	public static void setTestMode(bool testMode) {
		#if UNITY_IPHONE
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			_setTestModeVAMP(testMode);
		}
		#elif UNITY_ANDROID
		if (Application.platform == RuntimePlatform.Android) {
			AndroidJavaClass vampCls = new AndroidJavaClass(VAMP_CLASS);
			vampCls.CallStatic("setTestMode",testMode);
		}
		#endif
		isEnableTest = testMode;
	}

	public static bool isTestMode() {
		#if UNITY_IPHONE
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			isEnableTest = _isTestModeVAMP();
		}
		#elif UNITY_ANDROID
		if (Application.platform == RuntimePlatform.Android) {
			AndroidJavaClass vampCls = new AndroidJavaClass(VAMP_CLASS);
			isEnableTest = vampCls.CallStatic<bool>("isTestMode");
		}
		#endif
		return isEnableTest;
	}

	public static void setDebugMode(bool debugMode) {
		#if UNITY_IPHONE
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			_setDebugModeVAMP(debugMode);
		}
		#elif UNITY_ANDROID
		if (Application.platform == RuntimePlatform.Android) {
			AndroidJavaClass vampCls = new AndroidJavaClass(VAMP_CLASS);
			vampCls.CallStatic("setDebugMode",debugMode);
		}
		#endif
		isEnableDebug = debugMode;
	}

	public static bool isDebugMode() {
		#if UNITY_IPHONE
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			isEnableDebug = _isDebugModeVAMP();
		}
		#elif UNITY_ANDROID
		if (Application.platform == RuntimePlatform.Android) {
			AndroidJavaClass vampCls = new AndroidJavaClass(VAMP_CLASS);
			isEnableDebug = vampCls.CallStatic<bool>("isDebugMode");
		}
		#endif
		return isEnableDebug;
	}

	public static string SDKVersion() {
		string ret = "unknown";
		#if UNITY_IPHONE
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			ret = _SDKVersionVAMP();
		}
		#elif UNITY_ANDROID
		if (Application.platform == RuntimePlatform.Android) {
			AndroidJavaClass vampCls = new AndroidJavaClass(VAMP_CLASS);
			ret = vampCls.CallStatic<string>("SDKVersion");
		}
		#endif
		return ret;
	}

	public static float SupportedOSVersion() {
		float ret = 0;
		#if UNITY_IPHONE
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			// iOSの場合float
			ret =  _supportedOSVersionVAMP();
		}
		#elif UNITY_ANDROID
		if (Application.platform == RuntimePlatform.Android) {
			AndroidJavaClass vampCls = new AndroidJavaClass(VAMP_CLASS);
			ret = vampCls.CallStatic<int>("SupportedOSVersion");
		}
		#endif
		return ret;
	}

	public static bool isSupportedOSVersion() {
		bool ret = false;
		#if UNITY_IPHONE
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			ret =  _isSupportedOSVersionVAMP();
		}
		#elif UNITY_ANDROID
		if (Application.platform == RuntimePlatform.Android) {
			AndroidJavaClass vampCls = new AndroidJavaClass(VAMP_CLASS);
			ret = vampCls.CallStatic<bool>("isSupportedOSVersion");
		}
		#endif
		return ret;
	}

	public static void initializeAdnwSDK(string _placementID) {
		#if UNITY_IPHONE
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			_initializeAdnwSDK(vampni, _placementID);
		}
		#elif UNITY_ANDROID
		if (Application.platform == RuntimePlatform.Android) {
			if (_placementID != null && _placementID.Length > 0) {
				AndroidJavaClass player = new AndroidJavaClass(UNITYPLAYER_CLASS);
				AndroidJavaObject activity = player.GetStatic<AndroidJavaObject>("currentActivity");
				AndroidJavaClass vampCls = new AndroidJavaClass(VAMP_CLASS);
				vampCls.CallStatic("initializeAdnwSDK", activity, _placementID);
			} else {
				Debug.LogError("PlacementID is not set.");
			}
		}
		#endif
	}

	public static void initializeAdnwSDK(string _placementID, string state, int duration) {
		#if UNITY_IPHONE
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			_initializeAdnwSDKState(vampni, _placementID, state, duration);
		}
		#elif UNITY_ANDROID
		if (Application.platform == RuntimePlatform.Android) {
			if (_placementID != null && _placementID.Length > 0) {
				AndroidJavaClass player = new AndroidJavaClass(UNITYPLAYER_CLASS);
				AndroidJavaObject activity = player.GetStatic<AndroidJavaObject>("currentActivity");
				AndroidJavaClass vampCls = new AndroidJavaClass(VAMP_CLASS);
				AndroidJavaClass initStateCls = new AndroidJavaClass("jp.supership.vamp.VAMP$VAMPInitializeState");
				vampCls.CallStatic("initializeAdnwSDK", activity, _placementID, initStateCls.GetStatic<AndroidJavaObject>(state), duration);
			} else {
				Debug.LogError("PlacementID is not set.");
			}
		}
		#endif
	}

	public static void load() {
		if (noInstance) return;

		#if UNITY_IPHONE
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			_loadVAMP(vampni);
		}
		#elif UNITY_ANDROID
		if (Application.platform == RuntimePlatform.Android) {
			if (vampObj != null) {
				vampObj.Call("load");
			}
		}
		#endif
	}

	public static bool isReady() {
		if (noInstance) return false;

		bool ret = false;
		#if UNITY_IPHONE
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			// isReady に該当するものがないため
			ret = _isReadyVAMP(vampni);
		}
		#elif UNITY_ANDROID
		if (Application.platform == RuntimePlatform.Android) {
			if (vampObj != null) {
				ret = vampObj.Call<bool>("isReady");
			}
		}
		#endif
		return ret;
	}

	public static bool show() {
		if (noInstance) return false;

		bool ret = false;
		#if UNITY_IPHONE
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			ret = _showVAMP(vampni);
		}
		#elif UNITY_ANDROID
		if (Application.platform == RuntimePlatform.Android) {
			if (vampObj != null) {
				ret = vampObj.Call<bool>("show");
			}
		}
		#endif
		return ret;
	}

	public static void clearLoaded() {
		if (noInstance) return;

		#if UNITY_IPHONE
        // Nothing to do
		#elif UNITY_ANDROID
		if (Application.platform == RuntimePlatform.Android) {
			if (vampObj != null) {
				vampObj.Call("clearLoaded");
			}
		}
		#endif
	}

	public static void setMediationTimeout(int timeout) {
		#if UNITY_IPHONE
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			_setMediationTimeoutVAMP(vampni, timeout);
		}
		#elif UNITY_ANDROID
		if (Application.platform == RuntimePlatform.Android) {
			AndroidJavaClass vampCls = new AndroidJavaClass(VAMP_CLASS);
			vampCls.CallStatic("setMediationTimeout",timeout);
		}
		#endif
	}

    public static void getCountryCode(GameObject obj) {
        countryCodeObj = obj;
        #if UNITY_IPHONE
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
            _getCountryCodeVAMP(countryCodeObj.name);
        }
        #elif UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android) {
            AndroidJavaClass player = new AndroidJavaClass(UNITYPLAYER_CLASS);
            AndroidJavaObject activity = player.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaClass vampCls = new AndroidJavaClass(VAMP_CLASS);
            vampCls.CallStatic("getCountryCode", activity, new GetCountryCodeListener());
        }
        #endif
    }
	//-----------------ここまでVAMPメソッド

	#if UNITY_IPHONE
    // Nothing to do
	#elif UNITY_ANDROID
    class GetCountryCodeListener : AndroidJavaProxy {
        
        public GetCountryCodeListener() : base("jp.supership.vamp.VAMPGetCountryCodeListener") {
        }

        // null引数エラー回避の為のoverride
        public override AndroidJavaObject Invoke(string methodName, object[] javaArgs) {
            switch (methodName) {
                case "onCountryCode":
                    // javaArgs[0]:isoCode
                    countryCodeObj.SendMessage("VAMPCountryCode", joinParam((string)javaArgs[0]));
                    break;
                default:
                    return base.Invoke(methodName, javaArgs);
            }
            return null;
        }
    }

	class AdListener : AndroidJavaProxy {
        
		public AdListener() : base("jp.supership.vamp.VAMPListener") {
		}

		// null引数エラー回避の為のoverride
		public override AndroidJavaObject Invoke(string methodName, object[] javaArgs) {
			switch (methodName) {
			case "onReceive":
				// javaArgs[0]:placementId
				// javaArgs[1]:adnwName
				messageObj.SendMessage("VAMPDidReceive", joinParam((string)javaArgs[0], (string)javaArgs[1]));
				break;
			case "onFail":
				// javaArgs[0]:placementId
				// javaArgs[1]:error
				messageObj.SendMessage("VAMPDidFail", joinParam((string)javaArgs[0], ((AndroidJavaObject)javaArgs[1]).Call<string>("name")));
				break;
			case "onComplete":
				// javaArgs[0]:placementId
				// javaArgs[1]:adnwName
				messageObj.SendMessage("VAMPDidComplete", joinParam((string)javaArgs[0], (string)javaArgs[1]));
				break;
			case "onClose":
				// javaArgs[0]:placementId
				// javaArgs[1]:adnwName
				messageObj.SendMessage("VAMPDidClose", joinParam((string)javaArgs[0], (string)javaArgs[1]));
				break;
			case "onExpired":
				// javaArgs[0]:placementId
				messageObj.SendMessage("VAMPDidExpired", joinParam((string)javaArgs[0]));
				break;
			default:
				return base.Invoke(methodName, javaArgs);
			}
			return null;
		}
	}

	class AdvListener : AndroidJavaProxy {
        
		public AdvListener() : base("jp.supership.vamp.AdvancedListener") {
		}

		// null引数エラー回避の為のoverride
		public override AndroidJavaObject Invoke(string methodName, object[] javaArgs) {
			switch (methodName) {
			case "onLoadStart":
				// javaArgs[0]:placementId
				// javaArgs[1]:adnwName
				messageObj.SendMessage("VAMPLoadStart", joinParam((string)javaArgs[0], (string)javaArgs[1]));
				break;
			case "onLoadResult":
				// javaArgs[0]:placementId
				// javaArgs[1]:success
				// javaArgs[2]:adnwName
				// javaArgs[3]:message
				messageObj.SendMessage("VAMPLoadResult", joinParam((string)javaArgs[0], ((bool)javaArgs[1]).ToString(), (string)javaArgs[2], (string)javaArgs[3]));
				break;
			default:
				return base.Invoke(methodName, javaArgs);
			}
			return null;
		}
	}

	private static string joinParam(params string[] arg) {
		System.Text.StringBuilder buffer = new System.Text.StringBuilder();
		for (int i = 0; i < arg.Length; i++) {
			if (i > 0) {
				buffer.Append(",");
			}
			buffer.Append(arg[i]);
		}
		return buffer.ToString();
	}
    #endif  // end UNITY_ANDROID

	// Use this for initialization
	void Start() {
	}

	// Update is called once per frame
	void Update() {
	}

	public static string ADNWSDKVersion(string adnw) {
		string ret = "unknown";
		#if UNITY_IPHONE
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			ret = _ADNWSDKVersionVAMP(adnw);
		}
		#elif UNITY_ANDROID
        // Nothing to do
		#endif
		return ret;
	}

	public static string SDKInfo(string infoname) {
		string ret = "unknown";
		#if UNITY_IPHONE
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			ret = _SDKInfoVAMP(infoname);
		}
		#elif UNITY_ANDROID
        // Nothing to do
		#endif
		return ret;
	}
}
