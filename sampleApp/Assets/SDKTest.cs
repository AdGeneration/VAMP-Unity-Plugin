using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class SDKTest : MonoBehaviour
{
    /**
     * テスト用広告枠IDを使用して広告表示を確認することができます
     * iOS: 59755
     * Android: 59756
     * (テストIDのままリリースしないでください)
     */

    // iOS 広告枠ID
    public string iosPlacementID = "59755";
    // Android 広告枠ID
    public string androidPlacementID = "59756";
    // FAN's device ID hash to enable test mode on Android
    public string androidFANHashedID = "HASHED ID";
    // Test mode flag
    public bool testMode;
    // Debug mode flag
    public bool debugMode;
    // アドネットワークSDKの初期化モード
    public VAMPUnitySDK.InitializeState initializeState;
    // アドネットワークSDKの初期化実行間隔 (単位:秒)
    public int initializeDuration;
    // ターゲティング属性 ユーザの性別
    public VAMPUnitySDK.Gender userGender = VAMPUnitySDK.Gender.UNKNOWN;
    // ターゲティング属性 ユーザの誕生日
    public int userBirthYear = 1980;
    public int userBirthMonth = 2;
    public int userBirthDay = 20;

    public Texture logoTexture;
    public Texture btnOnTexture;
    public Texture btnOffTexture;

    private enum Block
    {
        Title = 0,
        Ad1,
        Ad2,
        Info
    }

    private const float SCREEN_WIDTH = 540f;
    private const float SCREEN_HEIGHT = 960f;

    private string placementID;
    private bool isVampInitialized;
    private bool isLoading;
    private string sdkVersion;
    private string appVersion;
    private string countryCode = "";
    private List<string> messages = new List<string>();
    private List<string> infos;
    private Block blk;
    private Vector3 scaleV3;
    private float width;
    private float height;
    private Matrix4x4 matrix;
    private GameObject logoCube;
    private Vector3 angle = new Vector3(0.0f, -90.0f, 0.0f);
    private Vector2 infoPosition = Vector2.zero;
    private Vector2 logPosition = Vector2.zero;

    private static EdgeInsets safeAreaInsets
    {
        get
        {
#if UNITY_IOS
            // for iPhone X
            if (Mathf.Min(Screen.width, Screen.height) == 1125 && Mathf.Max(Screen.width, Screen.height) == 2436)
            {
                if (Screen.width > Screen.height)
                {
                    // Landscape
                    return new EdgeInsets(0, 44, 21, 44);
                }
                else
                {
                    // Portrait
                    return new EdgeInsets(44, 0, 34, 0);
                }
            }
#endif
            // for others
            return new EdgeInsets(0, 0, 0, 0);
        }
    }

    void Start()
    {
#if UNITY_IPHONE
        placementID = iosPlacementID;
#elif UNITY_ANDROID
        placementID = androidPlacementID;
#endif

#if UNITY_ANDROID
        // Android FANのテストデバイスIDを登録
        if (androidFANHashedID != null && androidFANHashedID.Length > 0)
        {
            SDKTestUtil.AddFANTestDevice(androidFANHashedID);
        }
#endif

        // ユーザ属性をセットします
        //var targeting = new VAMPUnitySDK.Targeting();
        //targeting.Gender = userGender;
        //targeting.Birthday = new VAMPUnitySDK.Birthday(userBirthYear, userBirthMonth, userBirthDay);
        //VAMPUnitySDK.setTargeting(targeting);

        blk = Block.Title;
        isVampInitialized = false;
        logoCube = GameObject.Find("LogoCube");
        logoCube.SetActive(false);
        isLoading = false;

        float scaleX = Screen.width / SCREEN_WIDTH;
        float scaleY = Screen.height / SCREEN_HEIGHT;
        float scale = scaleX < scaleY ? scaleX : scaleY;
        scaleV3 = new Vector3(scale, scale, 1.0f);
        matrix = Matrix4x4.TRS(
            new Vector2(
                (Screen.width - SCREEN_WIDTH * scale) / 2,
                (Screen.height - SCREEN_HEIGHT * scale) / 2
            ),
            Quaternion.identity, scaleV3
        );
        width = Screen.width / scale;
        height = Screen.height / scale;

        sdkVersion = VAMPUnitySDK.SDKVersion();
        appVersion = SDKTestUtil.GetAppVersion();
        infos = SDKTestUtil.GetDeviceInfo();

        // EU圏内からのアクセスか判定します
        //VAMPUnitySDK.isEUAccess(gameObject);
    }

    void Update()
    {
        if (isLoading)
        {
            logoCube.transform.Rotate(angle * Time.deltaTime, Space.World);
        }
    }

    void OnGUI()
    {
        GUIStyle btnStyle = GUI.skin.GetStyle("Button");
        btnStyle.alignment = TextAnchor.MiddleCenter;
        btnStyle.fontSize = 30;

        GUIStyle labelStyle = GUI.skin.GetStyle("Label");
        labelStyle.alignment = TextAnchor.MiddleCenter;
        labelStyle.fontSize = 30;
        labelStyle.clipping = TextClipping.Overflow;

        GUIStyle textFStyle = GUI.skin.GetStyle("TextField");
        textFStyle.alignment = TextAnchor.MiddleCenter;
        textFStyle.fontSize = 30;

        GUI.matrix = Matrix4x4.Scale(scaleV3);

        EdgeInsets safeAreaInsets = SDKTest.safeAreaInsets;

        if (blk == Block.Title)
        {
            logoCube.SetActive(false);

            GUI.Label(new Rect(0, safeAreaInsets.Top, width, 60), "VAMP-Unity-Plugin");

            labelStyle.fontSize = 25;

            GUI.Label(new Rect(0, height - 60 - safeAreaInsets.Bottom, width, 50),
                      "APP " + appVersion + " / SDK " + sdkVersion);

            labelStyle.fontSize = 30;

            GUI.matrix = matrix;

            GUI.DrawTexture(new Rect(190, 490, 160, 160), logoTexture);

            labelStyle.alignment = TextAnchor.MiddleRight;

            GUI.enabled = !isVampInitialized;

            GUI.Label(new Rect(100, 90, 80, 60), "ID:");
            placementID = GUI.TextField(new Rect(200, 90, 240, 60), placementID);

            GUI.Label(new Rect(100, 170, 240, 60), "TestMode:");

            if (GUI.Button(new Rect(350, 180, 40, 40), testMode ? btnOnTexture : btnOffTexture))
            {
                testMode = !testMode;
            }

            GUI.Label(new Rect(100, 250, 240, 60), "DebugMode:");

            if (GUI.Button(new Rect(350, 260, 40, 40), debugMode ? btnOnTexture : btnOffTexture))
            {
                debugMode = !debugMode;
            }

            labelStyle.fontSize = 20;
            labelStyle.alignment = TextAnchor.MiddleCenter;

            GUI.Box(new Rect(65, 670, 410, 215), "");
            GUILayout.BeginArea(new Rect(75, 680, 400, 195));
            GUI.Label(new Rect(10, 0, 380, 30), "ADNW SDK Initialize State");

            labelStyle.fontSize = 18;

            if (GUI.Button(new Rect(35, 40, 40, 40), initializeState == VAMPUnitySDK.InitializeState.AUTO ? btnOnTexture : btnOffTexture))
            {
                initializeState = VAMPUnitySDK.InitializeState.AUTO;
            }

            GUI.Label(new Rect(10, 85, 90, 30), VAMPUnitySDK.InitializeState.AUTO.ToString());

            if (GUI.Button(new Rect(125, 40, 40, 40), initializeState == VAMPUnitySDK.InitializeState.WEIGHT ? btnOnTexture : btnOffTexture))
            {
                initializeState = VAMPUnitySDK.InitializeState.WEIGHT;
            }

            GUI.Label(new Rect(100, 85, 90, 30), VAMPUnitySDK.InitializeState.WEIGHT.ToString());

            if (GUI.Button(new Rect(225, 40, 40, 40), initializeState == VAMPUnitySDK.InitializeState.ALL ? btnOnTexture : btnOffTexture))
            {
                initializeState = VAMPUnitySDK.InitializeState.ALL;
            }

            GUI.Label(new Rect(200, 85, 90, 30), VAMPUnitySDK.InitializeState.ALL.ToString());

            if (GUI.Button(new Rect(325, 40, 40, 40), initializeState == VAMPUnitySDK.InitializeState.WIFIONLY ? btnOnTexture : btnOffTexture))
            {
                initializeState = VAMPUnitySDK.InitializeState.WIFIONLY;
            }

            GUI.Label(new Rect(300, 85, 90, 30), VAMPUnitySDK.InitializeState.WIFIONLY.ToString());

            if (GUI.Button(new Rect(30, 125, 340, 60), "ADNW SDK INIT"))
            {
                isVampInitialized = true;

                VAMPUnitySDK.setTestMode(testMode);
                VAMPUnitySDK.setDebugMode(debugMode);

                // アドネットワークSDKを事前に初期化しておくことができます。
                // アプリ起動時などのタイミングで1度だけ使用してください
                VAMPUnitySDK.initializeAdnwSDK(placementID, initializeState.ToString(), initializeDuration);
            }

            GUILayout.EndArea();

            GUI.enabled = true;

            if (GUI.Button(new Rect(100, 330, 340, 60), "AD1"))
            {
                blk = Block.Ad1;

                isVampInitialized = true;

                // trueを指定すると収益が発生しないテスト広告が配信されるようになります。
                // ストアに申請する際は必ずfalseを設定してください
                VAMPUnitySDK.setTestMode(testMode);

                // trueを指定するとログを詳細に出力するデバッグモードになります
                VAMPUnitySDK.setDebugMode(debugMode);

                // VAMPを初期化します。必ずLoadより先に実行してください
                VAMPUnitySDK.initVAMP(gameObject, placementID);
            }

            if (GUI.Button(new Rect(100, 410, 340, 60), "AD2"))
            {
                blk = Block.Ad2;

                isVampInitialized = true;

                // trueを指定すると収益が発生しないテスト広告が配信されるようになります。
                // ストアに申請する際は必ずfalseを設定してください
                VAMPUnitySDK.setTestMode(testMode);

                // trueを指定するとログを詳細に出力するデバッグモードになります
                VAMPUnitySDK.setDebugMode(debugMode);

                // VAMPを初期化します。必ずLoadより先に実行してください
                VAMPUnitySDK.initVAMP(gameObject, placementID);

                // 広告のプリロードを開始します
                VAMPUnitySDK.preload();
            }

            if (GUI.Button(new Rect(100, 490, 340, 60), "INFO"))
            {
                blk = Block.Info;

                // 国コード取得サンプル
                VAMPUnitySDK.getCountryCode(gameObject);
            }
        }
        else if (blk == Block.Ad1)
        {
            logoCube.SetActive(true);

            if (GUI.Button(new Rect(0, safeAreaInsets.Top, 120, 60), "＜戻る"))
            {
                blk = Block.Title;

                VAMPUnitySDK.clearLoaded();

                isLoading = false;

                messages.Clear();

                logPosition = Vector2.zero;
            }

            if (GUI.Button(new Rect(140, safeAreaInsets.Top, 120, 60), "LOAD"))
            {
                AddMessage("click load button.");

                // 動画広告ロードを開始します
                Debug.Log("[VAMPUnitySDK] VAMPUnitySDK.load()");

                VAMPUnitySDK.load();

                isLoading = true;
            }

            if (GUI.Button(new Rect(280, safeAreaInsets.Top, 120, 60), "SHOW"))
            {
                AddMessage("click show button.");

                // 動画広告が準備できているときは広告を表示します
                if (VAMPUnitySDK.isReady())
                {
                    Debug.Log("[VAMPUnitySDK] VAMPUnitySDK.show()");

                    VAMPUnitySDK.show();
                }
            }

            labelStyle.fontSize = 25;

            GUI.Label(new Rect(0, 70 + safeAreaInsets.Top, width, 50),
                "[Test:" + testMode + "] [Debug:" + debugMode + "]");

            GUI.Label(new Rect(0, 120 + safeAreaInsets.Top, width, 50),
                "ID:" + placementID);

            GUILayout.BeginArea(new Rect(20, 170 + safeAreaInsets.Top, width - 40, height - 190 - safeAreaInsets.Bottom));

            logPosition = GUILayout.BeginScrollView(logPosition);

            labelStyle.alignment = TextAnchor.MiddleLeft;
            labelStyle.fontSize = 20;

            int count = 0;

            for (int i = messages.Count - 1; i >= 0; i--)
            {
                GUILayout.Label(messages[i], labelStyle);
                count++;

                if (count >= 200)
                    break;
            }

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }
        else if (blk == Block.Ad2)
        {
            logoCube.SetActive(true);

            if (GUI.Button(new Rect(0, safeAreaInsets.Top, 120, 60), "＜戻る"))
            {
                blk = Block.Title;

                VAMPUnitySDK.clearLoaded();

                isLoading = false;

                messages.Clear();

                logPosition = Vector2.zero;
            }

            if (GUI.Button(new Rect(140, safeAreaInsets.Top, 240, 60), "LOAD & SHOW"))
            {
                AddMessage("click load & show button.");

                if (!VAMPUnitySDK.isReady())
                {
                    // 動画広告ロードを開始します
                    Debug.Log("[VAMPUnitySDK] VAMPUnitySDK.load()");

                    VAMPUnitySDK.load();

                    isLoading = true;
                }
                else
                {
                    // 動画広告が準備できているときは広告を表示します
                    Debug.Log("[VAMPUnitySDK] VAMPUnitySDK.show()");

                    VAMPUnitySDK.show();
                }
            }

            labelStyle.fontSize = 25;

            GUI.Label(new Rect(0, 70 + safeAreaInsets.Top, width, 50),
                "[Test:" + testMode + "] [Debug:" + debugMode + "]");

            GUI.Label(new Rect(0, 120 + safeAreaInsets.Top, width, 50),
                "ID:" + placementID);

            GUILayout.BeginArea(new Rect(20, 170 + safeAreaInsets.Top, width - 40, height - 190 - safeAreaInsets.Bottom));

            logPosition = GUILayout.BeginScrollView(logPosition);

            labelStyle.alignment = TextAnchor.MiddleLeft;
            labelStyle.fontSize = 20;

            int count = 0;

            for (int i = messages.Count - 1; i >= 0; i--)
            {
                GUILayout.Label(messages[i], labelStyle);
                count++;

                if (count >= 200)
                    break;
            }

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }
        else if (blk == Block.Info)
        {
            logoCube.SetActive(false);

            if (GUI.Button(new Rect(0, safeAreaInsets.Top, 120, 60), "＜戻る"))
            {
                blk = Block.Title;
            }

            GUILayout.BeginArea(new Rect(20, 70 + safeAreaInsets.Top, width - 40, height - 90 - safeAreaInsets.Bottom));

            infoPosition = GUILayout.BeginScrollView(infoPosition);

            labelStyle.alignment = TextAnchor.MiddleLeft;
            labelStyle.fontSize = 20;

            for (int i = 0; i < infos.Count; i++)
            {
                GUILayout.Label(infos[i], labelStyle);
            }

            GUILayout.Label("Country Code：" + countryCode, labelStyle);

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        GUI.matrix = Matrix4x4.identity;
        GUI.skin = null;
    }

    private void AddMessage(string str)
    {
        messages.Add(System.DateTime.Now.ToString("MM/dd HH:mm:ss ") + str);
    }

    //
    // VAMPからのメッセージを受け取る場合は以下のメソッドを実装してください
    //

    // ロードが完了し、広告表示できる状態になった時に通知されます
    void VAMPDidReceive(string msg)
    {
        string[] param = VAMPUnitySDK.MessageUtil.ParseMessage(msg);
        // param[0]:placementId
        // param[1]:adnwName
        AddMessage(string.Format("Receive {0} {1}", param[0], param[1]));

        isLoading = false;

        Debug.Log("[VAMPUnitySDK] VAMPDidReceive: " + msg);

        if (blk == Block.Ad2)
        {
            VAMPUnitySDK.show();
        }
    }

    // 広告のロード時にエラーが発生した時に通知されます
    void VAMPDidFailToLoad(string msg)
    {
        string[] param = VAMPUnitySDK.MessageUtil.ParseMessage(msg);
        // param[0]:error
        // param[1]:placementId
        AddMessage(string.Format("FailToLoad {0} {1}", param[0], param[1]));

        isLoading = false;

        Debug.Log("[VAMPUnitySDK] VAMPDidFailToLoad: " + msg);
    }

    // 広告の表示時にエラーが発生した時に通知されます
    void VAMPDidFailToShow(string msg)
    {
        string[] param = VAMPUnitySDK.MessageUtil.ParseMessage(msg);
        // param[0]:error
        // param[1]:placementId
        AddMessage(string.Format("FailToShow {0} {1}", param[0], param[1]));

        Debug.Log("[VAMPUnitySDK] VAMPDidFailToShow: " + msg);
    }


    // 広告のロード時や表示時にエラーが発生した時に通知されます。
    // v3.0.0からVAMPDidFailはdeprecatedです。代わりにVAMPDidFailToLoadおよびVAMPDidFailToShowを使用してください
    void VAMPDidFail(string msg)
    {
        Debug.Log("[VAMPUnitySDK] VAMPDidFail: " + msg);
    }

    // インセンティブ付与可能になったタイミングで通知されます
    void VAMPDidComplete(string msg)
    {
        string[] param = VAMPUnitySDK.MessageUtil.ParseMessage(msg);
        // param[0]:placementId
        // param[1]:adnwName
        AddMessage(string.Format("Complete {0} {1}", param[0], param[1]));

        Debug.Log("[VAMPUnitySDK] VAMPDidComplete: " + msg);
    }

    // 広告が閉じられた時に通知されます。
    // ユーザキャンセルなども含まれるのため、インセンティブ付与はVAMPDidCompleteで判定してください
    void VAMPDidClose(string msg)
    {
        string[] param = VAMPUnitySDK.MessageUtil.ParseMessage(msg);
        // param[0]:placementId
        // param[1]:adnwName
        AddMessage(string.Format("Close {0} {1}", param[0], param[1]));

        Debug.Log("[VAMPUnitySDK] VAMPDidClose: " + msg);
    }

    // 広告準備完了から55分経つと取得した広告の表示はできてもRTBの収益は発生しません。
    // この通知を受け取ったら、もう一度Loadからやり直す必要があります
    void VAMPDidExpired(string msg)
    {
        string[] param = VAMPUnitySDK.MessageUtil.ParseMessage(msg);
        // param[0]:placementId
        AddMessage(string.Format("Expire {0}", param[0]));

        isLoading = false;

        Debug.Log("[VAMPUnitySDK] VAMPDidExpired: " + msg);
    }

    // アドネットワークごとの広告取得が開始された時に通知されます
    void VAMPLoadStart(string msg)
    {
        string[] param = VAMPUnitySDK.MessageUtil.ParseMessage(msg);
        // param[0]:placementId
        AddMessage(string.Format("LoadStart {0} {1}", param[0], param[1]));

        Debug.Log("[VAMPUnitySDK] VAMPLoadStart: " + msg);
    }

    // アドネットワークごとの広告取得結果が通知されます(成功/失敗どちらも通知)。
    void VAMPLoadResult(string msg)
    {
        string[] param = VAMPUnitySDK.MessageUtil.ParseMessage(msg);
        // param[0]:placementId
        // param[1]:success
        // param[2]:adnwName
        // param[3]:message
        AddMessage(string.Format("LoadResult {0} {2} success={1} message={3}",
                param[0], param[1], param[2], param[3]));

        Debug.Log("[VAMPUnitySDK] VAMPLoadResult: " + msg);
    }

    // VAMPUnitySDK.getCountryCodeメソッドの取得結果が通知されます
    void VAMPCountryCode(string msg)
    {
        string[] param = VAMPUnitySDK.MessageUtil.ParseMessage(msg);
        // param[0]:isoCode
        countryCode = param[0];

        AddMessage(string.Format("CountryCode {0}", param[0]));

        Debug.Log("[VAMPUnitySDK] VAMPCountryCode: " + msg);
    }

    // VAMPUnitySDK.isEUAccessメソッドの取得結果が通知されます
    //void VAMPIsEUAccess(string msg)
    //{
    //    string[] param = VAMPUnitySDK.MessageUtil.ParseMessage(msg);
    //    // param[0]:access ("True"/"False")
    //    AddMessage(string.Format("IsEUAccess {0}", param[0]));

    //    Debug.Log("[VAMPUnitySDK] VAMPIsEUAccess: " + msg);

    //    if (param[0] == "True")
    //    {
    //        // TODO: ユーザに広告が個人に関連する情報を取得することの同意を求めます

    //        // ユーザの入力を受け付けACCEPTEDまたはDENIEDをセットします
    //        VAMPUnitySDK.setUserConsent(VAMPUnitySDK.ConsentStatus.ACCEPTED);
    //    }
    //}
}

public struct EdgeInsets
{

    public float Top { get; }

    public float Left { get; }

    public float Bottom { get; }

    public float Right { get; }

    public EdgeInsets(float top, float left, float bottom, float right)
    {
        Top = top;
        Left = left;
        Bottom = bottom;
        Right = right;
    }
}
