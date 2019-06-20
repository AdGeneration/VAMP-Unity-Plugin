using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

/// <summary>
/// SDKTest class.
/// </summary>
public class SDKTest : MonoBehaviour
{
    /**
     * テスト用広告枠IDを使用して広告表示を確認することができます
     * iOS: 59755
     * Android: 59756
     * (テストIDのままリリースしないでください)
     */

    // iOS 広告枠ID
    [SerializeField]
    private string iosPlacementID = "59755";
    // Android 広告枠ID
    [SerializeField]
    private string androidPlacementID = "59756";
    // FAN's device ID hash to enable test mode on Android
    [SerializeField]
    private string androidFANHashedID = "HASHED ID";
    // Test mode flag
    [SerializeField]
    private bool testMode;
    // Debug mode flag
    [SerializeField]
    private bool debugMode;

    [SerializeField]
    private bool childDirected;

    [SerializeField]
    private VAMPUnitySDK.UnderAgeOfConsent underAgeOfConsent;
    // ターゲティング属性 ユーザの性別
    [SerializeField]
    private VAMPUnitySDK.Gender userGender = VAMPUnitySDK.Gender.UNKNOWN;

    // ターゲティング属性 ユーザの誕生日
    [SerializeField]
    private Birthday birthday;

    [SerializeField]
    private FrequencyCap frequencyCap;

    [SerializeField]
    private VAMPConfig vampConfig;

    [SerializeField]
    private Texture logoTexture;

    [SerializeField]
    private Texture btnOnTexture;

    [SerializeField]
    public Texture btnOffTexture;

    [SerializeField]
    private Texture soundOnTexture;

    [SerializeField]
    private Texture soundOffTexture;

    [SerializeField]
    private AudioSource audioSource;

    private bool isPlayingPrev;

    private enum Block
    {
        Title = 0,
        Ad1,
        Ad2,
        Ad3,
        Info
    }

    private const float SCREEN_WIDTH = 540f;
    private const float SCREEN_HEIGHT = 960f;

    private string placementID;
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

    private static EdgeInsets SafeAreaInsets
    {
        get
        {
            var safeArea = Screen.safeArea;
            return new EdgeInsets(safeArea.yMin, safeArea.xMin, Screen.height - safeArea.yMax, Screen.width - safeArea.xMax);
        }
    }

    void Start()
    {
#if UNITY_IOS
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
        //targeting.Birthday = new VAMPUnitySDK.Birthday(birthday.year, birthday.month, birthday.day);
        //VAMPUnitySDK.setTargeting(targeting);

        var vampConfiguration = VAMPUnitySDK.VAMPConfiguration.getInstance();
        vampConfiguration.PlayerCancelable = vampConfig.playerCancelable;
        vampConfiguration.PlayerAlertTitleText = vampConfig.playerAlertTitleText;
        vampConfiguration.PlayerAlertBodyText = vampConfig.playerAlertBodyText;
        vampConfiguration.PlayerAlertCloseButtonText = vampConfig.playerAlertCloseButtonText;
        vampConfiguration.PlayerAlertContinueButtonText = vampConfig.playerAlertContinueButtonText;

        // COPPA対象ユーザかどうかを設定します
        //VAMPUnitySDK.setChildDirected(childDirected);

        // GDPRの対象ユーザで特定の年齢未満であるかどうかを設定します。
        //if (underAgeOfConsent != VAMPUnitySDK.UnderAgeOfConsent.UNKNOWN)
        //{
        //    VAMPUnitySDK.setUnderAgeOfConsent(underAgeOfConsent);
        //}

        blk = Block.Title;
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
        //        VAMPUnitySDK.isEUAccess((bool access) => {
        //            AddMessage(string.Format("IsEUAccess {0}", access));

        //            Debug.Log("[VAMPUnitySDK] VAMPIsEUAccess: " + access);

        //            if (access)
        //            {
        //                // TODO: ユーザに広告が個人に関連する情報を取得することの同意を求めます

        //                // ユーザの入力を受け付けACCEPTEDまたはDENIEDをセットします
        //                VAMPUnitySDK.setUserConsent(VAMPUnitySDK.ConsentStatus.ACCEPTED);
        //            }
        //        });

        VAMPListener listener = new VAMPListener();
        listener.onReceive += VAMPDidReceive;
        listener.onComplete += VAMPDidComplete;
        listener.onFailToLoad += VAMPDidFailToLoad;
        listener.onFailToShow += VAMPDidFailToShow;
        listener.onClose += VAMPDidClose;
        listener.onExpire += VAMPDidExpired;
        listener.onLoadStart += VAMPLoadStart;
        listener.onLoadResult += VAMPLoadResult;

        VAMPUnitySDK.setVAMPListener(listener);
        VAMPUnitySDK.setAdvancedListener(listener);
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

        var safeAreaInsets = SDKTest.SafeAreaInsets;

        if (blk == Block.Title)
        {
            isPlayingPrev = false;
            audioSource.Stop();

            logoCube.SetActive(false);

            labelStyle.fontSize = 25;

            GUI.Label(new Rect(0, height - 60 - safeAreaInsets.Bottom, width, 50),
                "APP " + appVersion + " / SDK " + sdkVersion);

            labelStyle.fontSize = 30;

            GUI.matrix = matrix;

            GUI.DrawTexture(new Rect(190, 490, 160, 160), logoTexture);

            labelStyle.alignment = TextAnchor.MiddleRight;

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

            if (GUI.Button(new Rect(100, 330, 340, 60), "AD1"))
            {
                blk = Block.Ad1;

                // trueを指定すると収益が発生しないテスト広告が配信されるようになります。
                // ストアに申請する際は必ずfalseを設定してください
                VAMPUnitySDK.setTestMode(testMode);

                // trueを指定するとログを詳細に出力するデバッグモードになります
                VAMPUnitySDK.setDebugMode(debugMode);

                // VAMPを初期化します。必ずLoadより先に実行してください
                VAMPUnitySDK.initialize(placementID);
            }

            if (GUI.Button(new Rect(100, 410, 340, 60), "AD2"))
            {
                blk = Block.Ad2;

                // trueを指定すると収益が発生しないテスト広告が配信されるようになります。
                // ストアに申請する際は必ずfalseを設定してください
                VAMPUnitySDK.setTestMode(testMode);

                // trueを指定するとログを詳細に出力するデバッグモードになります
                VAMPUnitySDK.setDebugMode(debugMode);

                // VAMPを初期化します。必ずLoadより先に実行してください
                VAMPUnitySDK.initialize(placementID);
                // 広告のプリロードを開始します
                VAMPUnitySDK.preload();
            }

            if (GUI.Button(new Rect(100, 490, 340, 60), "AD3"))
            {
                blk = Block.Ad3;

                // trueを指定すると収益が発生しないテスト広告が配信されるようになります。
                // ストアに申請する際は必ずfalseを設定してください
                VAMPUnitySDK.setTestMode(testMode);

                // trueを指定するとログを詳細に出力するデバッグモードになります
                VAMPUnitySDK.setDebugMode(debugMode);

                // VAMPを初期化します。必ずLoadより先に実行してください
                VAMPUnitySDK.initialize(placementID);
            }

            if (GUI.Button(new Rect(100, 570, 340, 60), "INFO"))
            {
                blk = Block.Info;


                // 国コード取得サンプル
                VAMPUnitySDK.getCountryCode((string countryCode) =>
                    {
                        this.countryCode = countryCode;
                        AddMessage(string.Format("CountryCode {0}", countryCode));

                        Debug.Log("[VAMPUnitySDK] VAMPCountryCode: " + countryCode);

                        //if (countryCode == "US") {
                        //    // COPPA対象ユーザである場合はtrueを設定する
                        //    VAMPUnitySDK.setCoppaChildDirected(true);
                        //}
                    });
            }
        }
        else if (blk == Block.Ad1)
        {
            logoCube.SetActive(true);

            if (GUI.Button(new Rect(0, 60 + safeAreaInsets.Top, 120, 60), "＜戻る"))
            {
                blk = Block.Title;

                isLoading = false;

                messages.Clear();

                logPosition = Vector2.zero;
            }

            if (GUI.Button(new Rect(140, 60 + safeAreaInsets.Top, 120, 60), "LOAD"))
            {
                AddMessage("click load button.");

                // 動画広告ロードを開始します
                Debug.Log("[VAMPUnitySDK] VAMPUnitySDK.load()");

                VAMPUnitySDK.load();
                isLoading = true;
            }

            if (GUI.Button(new Rect(280, 60 + safeAreaInsets.Top, 120, 60), "SHOW"))
            {
                AddMessage("click show button.");

                // 動画広告が準備できているときは広告を表示します
                if (VAMPUnitySDK.isReady())
                {
                    Debug.Log("[VAMPUnitySDK] VAMPUnitySDK.show()");
                    PauseSound();
                    VAMPUnitySDK.show();
                }
            }

            if (GUI.Button(new Rect(420, 60 + safeAreaInsets.Top, 120, 60), audioSource.isPlaying ? soundOffTexture : soundOnTexture))
            {
                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
                else
                {
                    audioSource.Play();
                }
            }

            labelStyle.fontSize = 25;

            GUI.Label(new Rect(0, 130 + safeAreaInsets.Top, width, 50),
                "[Test:" + testMode + "] [Debug:" + debugMode + "]");

            GUI.Label(new Rect(0, 180 + safeAreaInsets.Top, width, 50),
                "ID:" + placementID);

            GUILayout.BeginArea(new Rect(20, 230 + safeAreaInsets.Top, width - 40, height - 190 - safeAreaInsets.Bottom));

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

            if (GUI.Button(new Rect(0, 60 + safeAreaInsets.Top, 120, 60), "＜戻る"))
            {
                blk = Block.Title;

                isLoading = false;

                messages.Clear();

                logPosition = Vector2.zero;
            }

            if (GUI.Button(new Rect(140, 60 + safeAreaInsets.Top, 240, 60), "LOAD & SHOW"))
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
                    PauseSound();
                    VAMPUnitySDK.show();
                }
            }

            if (GUI.Button(new Rect(420, 60 + safeAreaInsets.Top, 120, 60), audioSource.isPlaying ? soundOffTexture : soundOnTexture))
            {
                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
                else
                {
                    audioSource.Play();
                }
            }

            labelStyle.fontSize = 25;

            GUI.Label(new Rect(0, 130 + safeAreaInsets.Top, width, 50),
                "[Test:" + testMode + "] [Debug:" + debugMode + "]");

            GUI.Label(new Rect(0, 180 + safeAreaInsets.Top, width, 50),
                "ID:" + placementID);

            GUILayout.BeginArea(new Rect(20, 230 + safeAreaInsets.Top, width - 40, height - 190 - safeAreaInsets.Bottom));

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
        else if (blk == Block.Ad3)
        {
            logoCube.SetActive(true);

            if (GUI.Button(new Rect(0, 60 + safeAreaInsets.Top, 120, 60), "＜戻る"))
            {
                blk = Block.Title;

                isLoading = false;

                messages.Clear();

                logPosition = Vector2.zero;
            }

            if (GUI.Button(new Rect(140, 60 + safeAreaInsets.Top, 120, 60), "LOAD"))
            {
                AddMessage("click load button.");

                // 動画広告ロードを開始します
                Debug.Log("[VAMPUnitySDK] VAMPUnitySDK.load()");

                VAMPUnitySDK.load();
                isLoading = true;
            }

            if (GUI.Button(new Rect(280, 60 + safeAreaInsets.Top, 120, 60), "SHOW"))
            {
                AddMessage("click show button.");

                // 動画広告が準備できているときは広告を表示します
                if (VAMPUnitySDK.isReady())
                {
                    Debug.Log("[VAMPUnitySDK] VAMPUnitySDK.show()");

                    PauseSound();
                    VAMPUnitySDK.show();
                }
            }

            if (GUI.Button(new Rect(420, 60 + safeAreaInsets.Top, 120, 60), audioSource.isPlaying ? soundOffTexture : soundOnTexture))
            {
                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
                else
                {
                    audioSource.Play();
                }
            }


            if (GUI.Button(new Rect(70, 130 + safeAreaInsets.Top, 400, 60), "SetFrequencyCap"))
            {
                AddMessage("click SetFrequencyCap button.");

                VAMPUnitySDK.setFrequencyCap(placementID, frequencyCap.impressions, frequencyCap.timeLimit);
            }

            if (GUI.Button(new Rect(70, 210 + safeAreaInsets.Top, 400, 60), "GetFrequencyCappedStatus"))
            {
                AddMessage("click GetFrequencyCappedStatus button.");

                using (var fpStatus = VAMPUnitySDK.getFrequencyCappedStatus(placementID))
                {
                    AddMessage(string.Format("capped:{0}, impressions:{1}, remainingTime:{2}, impressionLimit:{3}, timeLimit:{4}",
                        fpStatus.IsCapped, fpStatus.Impressions, fpStatus.RemainingTime, fpStatus.ImpressionLimit, fpStatus.TimeLimit));
                }
            }

            if (GUI.Button(new Rect(70, 290 + safeAreaInsets.Top, 400, 60), "ClearFrequencyCap"))
            {
                AddMessage("click clearFrequencyCap button");
                VAMPUnitySDK.clearFrequencyCap(placementID);
            }

            if (GUI.Button(new Rect(70, 370 + safeAreaInsets.Top, 400, 60), "ResetFrequencyCap"))
            {
                AddMessage("click resetFrequencyCap button");
                VAMPUnitySDK.resetFrequencyCap(placementID);
            }

            labelStyle.fontSize = 25;

            GUI.Label(new Rect(0, 450 + safeAreaInsets.Top, width, 50),
                "[Test:" + testMode + "] [Debug:" + debugMode + "]");

            GUI.Label(new Rect(0, 500 + safeAreaInsets.Top, width, 50),
                "ID:" + placementID);

            GUILayout.BeginArea(new Rect(20, 550 + safeAreaInsets.Top, width - 40, height - 190 - safeAreaInsets.Bottom));

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

            if (GUI.Button(new Rect(0, 60 + safeAreaInsets.Top, 120, 60), "＜戻る"))
            {
                blk = Block.Title;
            }

            GUILayout.BeginArea(new Rect(20, 130 + safeAreaInsets.Top, width - 40, height - 90 - safeAreaInsets.Bottom));

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

    private void PauseSound()
    {
        isPlayingPrev = audioSource.isPlaying;
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
        }
    }

    private void ResumeSound()
    {
        if (isPlayingPrev)
        {
            audioSource.UnPause();
        }
    }

    private void AddMessage(string str)
    {
        messages.Add(System.DateTime.Now.ToString("MM/dd HH:mm:ss ") + str);
    }

    public void VAMPDidReceive(string placementId, string adnwName)
    {
        AddMessage(string.Format("Receive {0} {1}", placementId, adnwName));

        isLoading = false;

        Debug.LogFormat("[VAMPUnitySDK] VAMPDidReceive: {0} {1}", placementId, adnwName);

        if (blk == Block.Ad2)
        {
            PauseSound();
            VAMPUnitySDK.show();
        }
    }

    public void VAMPDidFailToLoad(VAMPUnitySDK.VAMPError error, string placementId)
    {
        AddMessage(string.Format("FailToLoad {0} {1}", error, placementId));

        isLoading = false;
        ResumeSound();
        Debug.LogFormat("[VAMPUnitySDK] VAMPDidFailToLoad: {0} {1}", error, placementId);
    }

    public void VAMPDidFailToShow(VAMPUnitySDK.VAMPError error, string placementId)
    {
        AddMessage(string.Format("FailToShow {0} {1}", error, placementId));

        Debug.LogFormat("[VAMPUnitySDK] VAMPDidFailToShow: {0} {1}", error, placementId);
    }

    public void VAMPDidComplete(string placementId, string adnwName)
    {
        AddMessage(string.Format("Complete {0} {1}", placementId, adnwName));

        Debug.LogFormat("[VAMPUnitySDK] VAMPDidComplete: {0} {1}", placementId, adnwName);
    }

    public void VAMPDidClose(string placementId, string adnwName)
    {
        AddMessage(string.Format("Close {0} {1}", placementId, adnwName));
        ResumeSound();
        Debug.LogFormat("[VAMPUnitySDK] VAMPDidClose: {0} {1}", placementId, adnwName);
    }

    public void VAMPDidExpired(string placementId, string adnwName)
    {
        AddMessage(string.Format("Expire {0}", placementId));

        isLoading = false;

        Debug.LogFormat("[VAMPUnitySDK] VAMPDidExpired: {0}", placementId);
    }


    // IVAMPAdvancedListener
    public void VAMPLoadStart(string placementId, string adnwName)
    {
        AddMessage(string.Format("LoadStart {0} {1}", placementId, adnwName));

        Debug.LogFormat("[VAMPUnitySDK] VAMPLoadStart: {0} {1}", placementId, adnwName);
    }

    public void VAMPLoadResult(string placementId, bool success, string adnwName, string message)
    {
        AddMessage(string.Format("LoadResult {0} {2} success={1} message={3}",
                placementId, success, adnwName, message));

        Debug.LogFormat("[VAMPUnitySDK] VAMPLoadResult: {0} {1} {2} {3}", placementId, success, adnwName, message);
    }

    public class VAMPListener : VAMPUnitySDK.IVAMPListener, VAMPUnitySDK.IVAMPAdvancedListener
    {
        public delegate void VAMPCallback(string placementId, string adnwName);
        public delegate void VAMPErrorCallback(VAMPUnitySDK.VAMPError error, string placementId);
        public delegate void VAMPLoadResultCallback(string placementId, bool success, string adnwName, string message);

        public event VAMPCallback onReceive;
        public event VAMPErrorCallback onFailToLoad;
        public event VAMPErrorCallback onFailToShow;
        public event VAMPCallback onComplete;
        public event VAMPCallback onClose;
        public event VAMPCallback onExpire;
        public event VAMPCallback onLoadStart;
        public event VAMPLoadResultCallback onLoadResult;

        // IVAMPListener
        public void VAMPDidReceive(string placementId, string adnwName)
        {
            if (onReceive != null)
            {
                onReceive.Invoke(placementId, adnwName);
            }
        }

        public void VAMPDidFailToLoad(VAMPUnitySDK.VAMPError error, string placementId)
        {
            if (onFailToLoad != null)
            {
                onFailToLoad.Invoke(error, placementId);
            }
        }

        public void VAMPDidFailToShow(VAMPUnitySDK.VAMPError error, string placementId)
        {
            if (onFailToShow != null)
            {
                onFailToShow.Invoke(error, placementId);
            }
        }

        public void VAMPDidComplete(string placementId, string adnwName)
        {
            if (onComplete != null)
            {
                onComplete.Invoke(placementId, adnwName);
            }
        }

        public void VAMPDidClose(string placementId, string adnwName)
        {
            if (onClose != null)
            {
                onClose.Invoke(placementId, adnwName);
            }
        }

        public void VAMPDidExpired(string placementId)
        {
            if (onExpire != null)
            {
                onExpire.Invoke(placementId, null);
            }
        }


        // IVAMPAdvancedListener
        public void VAMPLoadStart(string placementId, string adnwName)
        {
            if (onLoadStart != null)
            {
                onLoadStart.Invoke(placementId, adnwName);
            }
        }

        public void VAMPLoadResult(string placementId, bool success, string adnwName, string message)
        {
            if (onLoadResult != null)
            {
                onLoadResult.Invoke(placementId, success, adnwName, message);
            }
        }
    }
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

    public override string ToString()
    {
        return
            string.Format("Top:{0}, Left:{1} Bottom:{2} Right:{3}", Top, Left, Bottom, Right);
    }
}

[System.Serializable]
public class VAMPConfig
{
    public bool playerCancelable = true;
    public string playerAlertTitleText = "注意!";
    public string playerAlertBodyText = "視聴途中でキャンセルすると報酬がもらえません";
    public string playerAlertCloseButtonText = "終了";
    public string playerAlertContinueButtonText = "再開";
}

[System.Serializable]
public class FrequencyCap
{
    public uint impressions = 3;
    public uint timeLimit = 3;
}

[System.Serializable]
public class Birthday
{
    public int year = 1980;
    public int month = 2;
    public int day = 20;
}