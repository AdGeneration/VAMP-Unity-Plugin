using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class SDKTest : MonoBehaviour {
	// iOS
	public string iosPlacementID;
	// Android
	public string androidPlacementID;

    // FAN's device ID hash to enable test mode on Android
    public string androidFANHashedID = "HASHED ID";

	public bool testMode;
	public bool debugMode;

	public Texture logoTexture;
	public Texture btnOnTexture;
	public Texture btnOffTexture;

	public enum InitializeState {
		AUTO = 0,
		WEIGHT,
		ALL,
		WIFIONLY
	};
	public InitializeState initializeState;	// デフォルトはAUTO
	public int initializeDuration;	// 秒単位で指定する。最小4秒、最大60秒。デフォルトは10秒。

	private string[] state = new string[] {
		InitializeState.AUTO.ToString(),
		InitializeState.WEIGHT.ToString(),
		InitializeState.ALL.ToString(),
		InitializeState.WIFIONLY.ToString()
	};

	private const float SCREEN_WIDTH = 540f;
	private const float SCREEN_HEIGHT = 960f;

	public enum Block {
		Title = 0,
		Ad1,
		Info
	};

	private string appVersion;
	private List<string> messages = new List<string>();
	private List<string> infos = new List<string>();
	private int blk;
	private string placementID;
	private Vector3 scaleV3;
	private float width;
	private float height;
	private Matrix4x4 matrix;
	private bool isInitVamp;
	private GameObject logoCube;
	private bool loading;
	private Vector3 angle = new Vector3(0.0f, -90.0f, 0.0f);
	private Vector2 infoPosition = Vector2.zero;
	private Vector2 logPosition = Vector2.zero;

	// Use this for initialization
	void Start() {
		#if UNITY_IPHONE
		placementID = iosPlacementID;
		#elif UNITY_ANDROID
		placementID = androidPlacementID;
		#endif

		blk = (int)Block.Title;
		isInitVamp = false;
		logoCube = GameObject.Find("LogoCube");
		logoCube.SetActive(false);
		loading = false;

		float scaleX = Screen.width / SCREEN_WIDTH;
		float scaleY = Screen.height / SCREEN_HEIGHT;
		float scale = scaleX < scaleY ? scaleX : scaleY;
		scaleV3 = new Vector3(scale, scale, 1.0f);
		matrix = Matrix4x4.TRS(
			new Vector2((Screen.width - (SCREEN_WIDTH * scale)) / 2, (Screen.height - (SCREEN_HEIGHT * scale)) / 2), Quaternion.identity, scaleV3);
		width = Screen.width / scale;
		height = Screen.height / scale;

		appVersion = SDKTestUtil.GetInfo(infos);

        #if UNITY_ANDROID
        // Android FANのテストデバイスIDを登録
        if (androidFANHashedID != null && androidFANHashedID.Length > 0) {
            SDKTestUtil.AddFANTestDevice(androidFANHashedID);
        }
        #endif
	}

	// Update is called once per frame
	void Update() {
		if (loading) {
			logoCube.transform.Rotate(angle * Time.deltaTime, Space.World);
		}
	}

	void OnGUI() {
		// Buttonのスタイル
		GUIStyle btnStyle = GUI.skin.GetStyle("Button");
		btnStyle.alignment = TextAnchor.MiddleCenter;
		btnStyle.fontSize = 30;

		// Labelのスタイル
		GUIStyle labelStyle = GUI.skin.GetStyle("Label");
		labelStyle.alignment = TextAnchor.MiddleCenter;
		labelStyle.fontSize = 30;

		// TextFieldのスタイル
		GUIStyle textFStyle = GUI.skin.GetStyle("TextField");
		textFStyle.alignment = TextAnchor.MiddleCenter;
		textFStyle.fontSize = 30;

		GUI.matrix = Matrix4x4.Scale(scaleV3);

		if (blk == (int)Block.Title) {
			// キューブ
			logoCube.SetActive(false);

			// タイトル
			GUI.Label(new Rect(0, 0, width, 60), "VAMP-Unity-Plugin");

			labelStyle.fontSize = 25;

			// アプリバージョン、SDKバージョン
			GUI.Label(new Rect(0, height-60, width, 50), "APP " + appVersion + " / SDK " + VAMPUnitySDK.SDKVersion());

			labelStyle.fontSize = 30;

			GUI.matrix = matrix;

			// ロゴ
			GUI.DrawTexture(new Rect(190, 490, 160, 160), logoTexture);

			labelStyle.alignment = TextAnchor.MiddleRight;

			GUI.enabled = !isInitVamp;

			// PlacementID
			GUI.Label(new Rect(100, 90, 80, 60), "ID:");
			placementID = GUI.TextField(new Rect(200, 90, 240, 60), placementID);
			// TestMode
			GUI.Label(new Rect(100, 170, 240, 60), "TestMode:");
			if (GUI.Button(new Rect(350, 180, 40, 40), testMode == true ? btnOnTexture : btnOffTexture)) {
				testMode = !testMode;
			}
			// DebugMode
			GUI.Label(new Rect(100, 250, 240, 60), "DebugMode:");
			if (GUI.Button(new Rect(350, 260, 40, 40), debugMode == true ? btnOnTexture : btnOffTexture)) {
				debugMode = !debugMode;
			}

			labelStyle.fontSize = 20;
			labelStyle.alignment = TextAnchor.MiddleCenter;

			// 各アドネットワークのSDK初期化
			GUI.Box(new Rect(65, 670, 410, 215), "");
			GUILayout.BeginArea(new Rect(75, 680, 400, 195));
			GUI.Label(new Rect(10, 0, 380, 30), "ADNW SDK Initialize State");

			labelStyle.fontSize = 18;

			if (GUI.Button(new Rect(35, 40, 40, 40), initializeState == InitializeState.AUTO ? btnOnTexture : btnOffTexture)) {
				initializeState = InitializeState.AUTO;
			}
			GUI.Label(new Rect(10, 85, 90, 30), state[(int)InitializeState.AUTO]);
			if (GUI.Button(new Rect(125, 40, 40, 40), initializeState == InitializeState.WEIGHT ? btnOnTexture : btnOffTexture)) {
				initializeState = InitializeState.WEIGHT;
			}
			GUI.Label(new Rect(100, 85, 90, 30), state[(int)InitializeState.WEIGHT]);
			if (GUI.Button(new Rect(225, 40, 40, 40), initializeState == InitializeState.ALL ? btnOnTexture : btnOffTexture)) {
				initializeState = InitializeState.ALL;
			}
			GUI.Label(new Rect(200, 85, 90, 30), state[(int)InitializeState.ALL]);
			if (GUI.Button(new Rect(325, 40, 40, 40), initializeState == InitializeState.WIFIONLY ? btnOnTexture : btnOffTexture)) {
				initializeState = InitializeState.WIFIONLY;
			}
			GUI.Label(new Rect(300, 85, 90, 30), state[(int)InitializeState.WIFIONLY]);

			if (GUI.Button(new Rect(30, 125, 340, 60), "ADNW SDK INIT")) {
				isInitVamp = true;
				VAMPUnitySDK.setTestMode(testMode);
				VAMPUnitySDK.setDebugMode(debugMode);
				VAMPUnitySDK.initializeAdnwSDK(placementID, initializeState.ToString(), initializeDuration);
			}
			GUILayout.EndArea();

			GUI.enabled = true;

			// AD1ボタン
			if (GUI.Button(new Rect(100, 330, 340, 60), "AD1")) {
				blk = (int)Block.Ad1;
				isInitVamp = true;
				// VAMP初期化
				VAMPUnitySDK.setTestMode(testMode);
				VAMPUnitySDK.setDebugMode(debugMode);
				VAMPUnitySDK.initVAMP(GameObject.Find("SDKTest"), placementID);
			}
			// Infoボタン
			if (GUI.Button(new Rect(100, 410, 340, 60), "INFO")) {
				blk = (int)Block.Info;
			}

		} else if (blk == (int)Block.Ad1) {
			// キューブ
			logoCube.SetActive(true);

			// 戻るボタン
			if (GUI.Button(new Rect(0, 0, 120, 60), "＜戻る")) {
				blk = (int)Block.Title;
				VAMPUnitySDK.clearLoaded();
				loading = false;
				messages.Clear();
				logPosition = Vector2.zero;
			}
			// LOADボタン
			if (GUI.Button(new Rect(140, 0, 120, 60), "LOAD")) {
                AddMessage("click load button.");
				if (!VAMPUnitySDK.isReady()) {
					VAMPUnitySDK.load();
					loading = true;
				}
			}
			// SHOWボタン
			if (GUI.Button(new Rect(280, 0, 120, 60), "SHOW")) {
                AddMessage("click show button.");
				if (VAMPUnitySDK.isReady()) {
				    VAMPUnitySDK.show();
				}

			}
			#if UNITY_ANDROID
			// CLEARボタン（※現状Androidのみの機能なのでAndroidの時のみ表示）
			if (GUI.Button(new Rect(420, 0, 120, 60), "CLEAR")) {
                AddMessage("click clear button.");
				VAMPUnitySDK.clearLoaded();
				loading = false;
			}
			#endif

			labelStyle.fontSize = 25;

			// 設定されているtestMode、debugMode表示
			GUI.Label(new Rect(0, 70, width, 50), "[Test:" + testMode + "] [Debug:" + debugMode + "]");
			// 設定しているPlacementID表示
			GUI.Label(new Rect(0, 120, width, 50), "ID:" + placementID);
			// ログ
			GUILayout.BeginArea(new Rect(20, 170, width-40, height-190));
			logPosition = GUILayout.BeginScrollView(logPosition);
			CreateLogs();
			GUILayout.EndScrollView();
			GUILayout.EndArea();

		} else if (blk == (int)Block.Info) {
			// キューブ
			logoCube.SetActive(false);

			// 戻るボタン
			if (GUI.Button(new Rect(0, 0, 120, 60), "＜戻る")) {
				blk = (int)Block.Title;
			}

			// 端末情報
			GUILayout.BeginArea(new Rect(20, 70, width-40, height-90));
			infoPosition = GUILayout.BeginScrollView(infoPosition);
			CreateInfos();
			GUILayout.EndScrollView();
			GUILayout.EndArea();

		}

		// GUIの設定を元に戻す
		GUI.matrix = Matrix4x4.identity;
		GUI.skin = null;
	}

	// ログ表示を行う
	private void CreateLogs() {
		// 入力されたメッセージを逆順に200表示
		GUIStyle style = GUI.skin.GetStyle("Label");
		style.alignment = TextAnchor.MiddleLeft;
		style.fontSize = 20;

		int count = 1;
		for (int i = messages.Count - 1; i >= 0; i--) {
			GUILayout.Label(messages[i], style);
			count ++;
			if (count > 200) break;
		}
	}

	// 端末情報表示を行う
	private void CreateInfos() {
		GUIStyle style = GUI.skin.GetStyle("Label");
		style.alignment = TextAnchor.MiddleLeft;
		style.fontSize = 20;

		for (int i = 0; i < infos.Count; i++) {
			GUILayout.Label(infos[i], style);
		}
	}

	/**
	 * 動画表示の準備完了
	 */
	void VAMPDidReceive(string str) {
		#if UNITY_IPHONE
        AddMessage(str);
		#elif UNITY_ANDROID
		string[] param = VampParam(str);
		// param[0]:placementId
		// param[1]:adnwName
        AddMessage("onReceive(" + param[1] + ")");
		#endif
		loading = false;
	}

	/**
	 * 動画準備or表示失敗
	 */
	void VAMPDidFail(string str) {
		#if UNITY_IPHONE
        AddMessage(str);
		#elif UNITY_ANDROID
		string[] param = VampParam(str);
		// param[0]:placementId
		// param[1]:error
        AddMessage("onFail() " + param[1]);
		#endif
		loading = false;
	}

	/**
	 * 動画再生正常終了（インセンティブ付与可能）
	 */
	void VAMPDidComplete(string str) {
		#if UNITY_IPHONE
        AddMessage(str);
		#elif UNITY_ANDROID
		string[] param = VampParam(str);
		// param[0]:placementId
		// param[1]:adnwName
        AddMessage("onComplete(" + param[1] + ")");
		#endif
	}

	/**
	 * 動画プレーヤーやエンドカードが表示終了
	 * ＜注意：ユーザキャンセルなども含むので、インセンティブ付与はonCompleteで判定すること＞
	 */
	void VAMPDidClose(string str) {
		#if UNITY_IPHONE
        AddMessage(str);
		#elif UNITY_ANDROID
		string[] param = VampParam(str);
		// param[0]:placementId
		// param[1]:adnwName
        AddMessage("onClose(" + param[1] + ")");
		#endif
	}

	/**
	 * 有効期限オーバー
	 * ＜注意：onReceiveを受けてからの有効期限が切れました。showするには再度loadを行う必要が有ります＞
	 */
	void VAMPDidExpired(string str) {
		#if UNITY_IPHONE
        AddMessage(str);
		#elif UNITY_ANDROID
		// param[0]:placementId
        AddMessage("onExpired()");
		#endif
		loading = false;
	}

	/**
	 * 優先順位順にアドネットワークごとの広告取得を開始
	 */
	void VAMPLoadStart(string str) {
		#if UNITY_IPHONE
        AddMessage(str);
		#elif UNITY_ANDROID
		string[] param = VampParam(str);
		// param[0]:placementId
		// param[1]:adnwName
        AddMessage("onLoadStart(" + param[1] + ")");
		#endif
	}

	/**
	 * アドネットワークごとの広告取得結果
	 */
	void VAMPLoadResult(string str) {
		#if UNITY_IPHONE
        AddMessage(str);
		#elif UNITY_ANDROID
		string[] param = VampParam(str);
		// param[0]:placementId
		// param[1]:success
		// param[2]:adnwName
		// param[3]:message
        AddMessage("onLoadResult(" + param[2] + ") " + param[3]);
		#endif
	}

	#if UNITY_ANDROID
	private string[] VampParam(string str) {
		if (str != null && str.Length > 0) {
			string[] paramList = str.Split(',');
			return paramList;
		}
		return null;
	}
	#endif

	private void AddMessage(string str) {
		messages.Add(System.DateTime.Now.ToString("MM/dd HH:mm:ss ") + str);
	}
}
