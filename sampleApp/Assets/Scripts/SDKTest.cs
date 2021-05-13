using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// SDKTest class.
/// </summary>
public class SDKTest : MonoBehaviour
{
    /*
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
    private VAMP.Privacy.ChildDirected childDirected;

    [SerializeField]
    private VAMP.Privacy.UnderAgeOfConsent underAgeOfConsent;

    // ターゲティング属性 ユーザの性別
    [SerializeField]
    private VAMP.Targeting.Gender userGender = VAMP.Targeting.Gender.Unknown;

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
        AR,
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

    private VAMP.RewardedAd rewardedAd;
    private VAMP.ARAd arAd;

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
        var targeting = new VAMP.Targeting.Targeting
        {
            Gender = userGender,
            Birthday = new VAMP.Targeting.Birthday
            {
                Year = birthday.year,
                Month = birthday.month,
                Day = birthday.day
            }
        };

        VAMP.Targeting.TargetingManager.SetTargeting(targeting);

        // COPPA対象ユーザかどうかを設定します
        VAMP.Privacy.PrivacySettings.SetChildDirected(childDirected);

        // GDPRの対象ユーザで特定の年齢未満であるかどうかを設定します。
        if (underAgeOfConsent != VAMP.Privacy.UnderAgeOfConsent.Unknown)
        {
            VAMP.Privacy.PrivacySettings.SetUnderAgeOfConsent(underAgeOfConsent);
        }

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

        sdkVersion = VAMP.SDK.SDKVersion;
        appVersion = Application.version;
        infos = SDKTestUtil.GetDeviceInfo();

        //// EU圏内からのアクセスか判定します
        VAMP.SDK.IsEUAccess((bool access) =>
        {
            AddMessage(string.Format("IsEUAccess {0}", access));

            Debug.Log("[VAMPUnitySDK] IsEUAccess: " + access);

            if (access)
            {
                // TODO: ユーザに広告が個人に関連する情報を取得することの同意を求めます

                // ユーザの入力を受け付けACCEPTEDまたはDENIEDをセットします
                VAMP.Privacy.PrivacySettings.SetConsentStatus(VAMP.Privacy.ConsentStatus.Accepted);
            }
        });
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

            if (GUI.Button(new Rect(100, 330, 340, 60), "Ad1"))
            {
                blk = Block.Ad1;
                // VAMPを初期化します。必ずLoadより先に実行してください
                VAMP.SDK.TestMode = testMode;
                VAMP.SDK.DebugMode = debugMode;
                InitializeRewardedAd();
            }

            if (GUI.Button(new Rect(100, 410, 340, 60), "Ad2"))
            {
                blk = Block.Ad2;

                VAMP.SDK.TestMode = testMode;
                VAMP.SDK.DebugMode = debugMode;
                InitializeRewardedAd();
                rewardedAd.Preload(CreateRequest());
            }

            if (GUI.Button(new Rect(100, 490, 340, 60), "AR"))
            {
                blk = Block.AR;

                VAMP.SDK.TestMode = testMode;
                VAMP.SDK.DebugMode = debugMode;
                InitializeARAd();
            }

            if (GUI.Button(new Rect(100, 570, 340, 60), "INFO"))
            {
                blk = Block.Info;

                VAMP.SDK.GetLocation((VAMP.Location location) =>
                {
                    this.countryCode = location.CountryCode;
                    AddMessage(string.Format("CountryCode {0}", countryCode));

                    Debug.Log("[VAMPUnitySDK] CountryCode: " + countryCode);

                    //if (countryCode == "US") {
                    //    // COPPA対象ユーザである場合はtrueを設定する
                    //    VAMP.Privacy.PrivacySettings.SetChildDirected(VAMP.Privacy.ChildDirected.True);
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
                Debug.Log("[VAMPUnitySDK] RewardedAd.load()");

                rewardedAd.Load(CreateRequest());
                isLoading = true;
            }

            if (GUI.Button(new Rect(280, 60 + safeAreaInsets.Top, 120, 60), "SHOW"))
            {
                AddMessage("click show button.");

                // 動画広告が準備できているときは広告を表示します
                if (rewardedAd.IsReady)
                {
                    Debug.Log("[VAMPUnitySDK] RewardedAd.show()");
                    PauseSound();
                    rewardedAd.Show();
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

                if (!rewardedAd.IsReady)
                {
                    // 動画広告ロードを開始します
                    Debug.Log("[VAMPUnitySDK] RewardedAd.Load()");

                    rewardedAd.Load(CreateRequest());

                    isLoading = true;
                }
                else
                {
                    // 動画広告が準備できているときは広告を表示します
                    Debug.Log("[VAMPUnitySDK] RewardedAd.Show()");
                    PauseSound();
                    rewardedAd.Show();
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
        else if (blk == Block.AR)
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
                Debug.Log("[VAMPUnitySDK] RewardedAd.Load()");

                arAd.Load(CreateRequest());
                isLoading = true;
            }

            if (GUI.Button(new Rect(280, 60 + safeAreaInsets.Top, 120, 60), "SHOW"))
            {
                AddMessage("click show button.");

                // 動画広告が準備できているときは広告を表示します
                if (arAd.IsReady)
                {
                    Debug.Log("[VAMPUnitySDK] RewardedAd.Show()");
                    PauseSound();
                    arAd.Show();
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

    private void InitializeRewardedAd()
    {
        if (rewardedAd != null)
        {
            rewardedAd.Dispose();
            rewardedAd = null;
        }

        rewardedAd = new VAMP.RewardedAd(placementID);
        rewardedAd.OnStartedLoad += HandleVAMPRewardedAdLoadStart;
        rewardedAd.OnLoaded += HandleVAMPRewardedAdLoadResult;
        rewardedAd.OnReceived += HandleVAMPRewardedAdDidReceive;
        rewardedAd.OnFailedToLoad += HandleVAMPRewardedAdDidFailToLoad;
        rewardedAd.OnFailedToShow += HandleVAMPRewardedAdDidFailToShow;
        rewardedAd.OnOpened += HandleVAMPRewardedAdDidOpen;
        rewardedAd.OnCompleted += HandleVAMPRewardedAdDidComplete;
        rewardedAd.OnClosed += HandleVAMPRewardedAdDidClose;
        rewardedAd.OnExpired += HandleVAMPRewardedAdDidExpire;
    }

    public void InitializeARAd()
    {
        if (arAd != null)
        {
            arAd.Dispose();
            arAd = null;
        }

        arAd = new VAMP.ARAd(placementID);
        arAd.OnReceived += HandleVAMPARAdReceived;
        arAd.OnFailedToLoad += HandleVAMPARAdFailedToLoad;
        arAd.OnFailedToShow += HandleVAMPARAdFailedToShow;
        arAd.OnExpired += HandleVAMPARAdExpired;
        arAd.OnClosed += HandleVAMPARAdClosed;
        arAd.OnCameraAccessNotAuthorized += HandleVAMPARAdCameraAccessNotAuthorized;
    }

    private VAMP.Request CreateRequest()
    {
        var config = new VAMP.VideoConfiguration
        {
            IsPlayerCancelable = vampConfig.playerCancelable,
            PlayerAlertTitleText = vampConfig.playerAlertTitleText,
            PlayerAlertBodyText = vampConfig.playerAlertBodyText,
            PlayerAlertCloseButtonText = vampConfig.playerAlertCloseButtonText,
            PlayerAlertContinueButtonText = vampConfig.playerAlertContinueButtonText
        };

        return new VAMP.Request.Builder()
            .SetVideoConfiguration(config)
            .Build();
    }

    public void HandleVAMPRewardedAdDidReceive(object sender, System.EventArgs args)
    {
        AddMessage($"Receive adnwName={rewardedAd.ResponseInfo.AdNetworkName}");

        isLoading = false;

        Debug.Log($"[VAMPUnitySDK] OnReceive: {GetRewardedAdInfoString()}");

        if (blk == Block.Ad2)
        {
            PauseSound();
            rewardedAd.Show();
        }
    }

    public void HandleVAMPRewardedAdDidFailToLoad(object sender, VAMP.AdFailEventArgs args)
    {
        AddMessage($"FailToLoad error={args.Error}");

        isLoading = false;
        ResumeSound();
        Debug.Log($"[VAMPUnitySDK] OnFailedToLoad: " +
            $"error={args.Error}, {GetRewardedAdInfoString()}");
    }

    public void HandleVAMPRewardedAdDidFailToShow(object sender, VAMP.AdFailEventArgs args)
    {
        AddMessage($"FailToShow error={args.Error}");

        Debug.Log($"[VAMPUnitySDK] OnFailedToShow: " +
            $"error={args.Error}, {GetRewardedAdInfoString()}");
    }

    public void HandleVAMPRewardedAdDidOpen(object sender, System.EventArgs args)
    {
        AddMessage($"Open adnwName={rewardedAd.ResponseInfo?.AdNetworkName ?? ""}");

        Debug.Log($"[VAMPUnitySDK] OnOpen: {GetRewardedAdInfoString()}");
    }

    public void HandleVAMPRewardedAdDidComplete(object sender, System.EventArgs args)
    {
        AddMessage($"Complete adnwName={rewardedAd.ResponseInfo?.AdNetworkName ?? ""}");

        Debug.Log($"[VAMPUnitySDK] OnComplete: {GetRewardedAdInfoString()}");
    }

    public void HandleVAMPRewardedAdDidClose(object sender, VAMP.AdCloseEventArgs args)
    {
        AddMessage($"Close adnwname={rewardedAd.ResponseInfo?.AdNetworkName ?? ""}, AdClicked={args.AdClicked}");

        ResumeSound();

        Debug.Log($"[VAMPUnitySDK] OnClose: {GetRewardedAdInfoString()}");
    }

    public void HandleVAMPRewardedAdDidExpire(object sender, System.EventArgs args)
    {
        AddMessage("Expire");

        isLoading = false;

        Debug.Log($"[VAMPUnitySDK] OnExpire: {GetRewardedAdInfoString()}");
    }

    public void HandleVAMPRewardedAdLoadStart(object sender, VAMP.AdLoadEventArgs args)
    {
        AddMessage($"LoadStart adnwName={args.AdNetworkName}");

        Debug.Log($"[VAMPUnitySDK] OnLoadStart: {GetRewardedAdInfoString()}");
    }

    public void HandleVAMPRewardedAdLoadResult(object sender, VAMP.AdLoadResultEventArgs args)
    {
        AddMessage($"LoadResult adnwName={args.AdNetworkName}, success={args.IsSuccess}, message={args.Message}");

        Debug.Log($"[VAMPUnitySDK] OnLoadStart: " +
            $"placementId={rewardedAd.PlacementId}, adnwName={args.AdNetworkName}, seqId={rewardedAd.ResponseInfo?.SeqId ?? ""}, success={args.IsSuccess}, message={args.Message}");
    }

    public void HandleVAMPARAdReceived(object sender, System.EventArgs args)
    {
        AddMessage("Receive");
        isLoading = false;

        Debug.Log("[VAMPUnitySDK] OnReceived");
    }

    public void HandleVAMPARAdFailedToLoad(object sender, VAMP.AdFailEventArgs args)
    {
        var message = $"FailToLoad error={args.Error}";
        AddMessage(message);

        Debug.Log($"[VAMPUnitySDK] {message}");
    }

    public void HandleVAMPARAdFailedToShow(object sender, VAMP.AdFailEventArgs args)
    {
        var message = $"FailToShow error={args.Error}";
        AddMessage(message);

        Debug.Log($"[VAMPUnitySDK] {message}");
    }

    public void HandleVAMPARAdExpired(object sender, System.EventArgs args)
    {
        var message = "Expired";
        AddMessage(message);

        Debug.Log($"[VAMPUnitySDK] {message}");
    }

    public void HandleVAMPARAdClosed(object sender, VAMP.AdCloseEventArgs args)
    {
        var message = $"Closed clicked={args.AdClicked}";
        AddMessage(message);

        Debug.Log($"[VAMPUnitySDK] {message}");
    }

    public void HandleVAMPARAdCameraAccessNotAuthorized(object sender, System.EventArgs args)
    {
        var message = "CameraAccessNotAuthorized";
        AddMessage(message);

        Debug.Log($"[VAMPUnitySDK] {message}");
    }

    private string GetRewardedAdInfoString()
    {
        return $"placementId={rewardedAd.PlacementId}, adnwName={rewardedAd.ResponseInfo?.AdNetworkName ?? ""}, seqId={rewardedAd.ResponseInfo?.SeqId ?? ""}";
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
    public uint year = 1980;
    public uint month = 2;
    public uint day = 20;
}