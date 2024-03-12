using System;
using UnityEngine;
using UnityEngine.UI;

public class AdSample : MonoBehaviour
{
    [SerializeField]
    private Button loadButton;

    [SerializeField]
    private Button showButton;

    [SerializeField]
    private Button loadAndShowButton;

    [SerializeField]
    private Button loadButton1;

    [SerializeField]
    private Button showButton1;

    [SerializeField]
    private Button loadButton2;

    [SerializeField]
    private Button showButton2;

    [SerializeField]
    private Button soundButton;

    [SerializeField]
    private Texture2D soundOnImage;

    [SerializeField]
    private Texture2D soundOffImage;

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private Text infoText;

    [SerializeField]
    private TMPro.TextMeshProUGUI messageText;

    [SerializeField]
    private Transform content;

    private bool isPlayingPrev;

    private VAMP.RewardedAd rewardedAd;
    private VAMP.RewardedAd rewardedAd2;

    private static bool IsAd1 => ConfigurationManager.Instance.SampleType == SampleType.Ad1;
    private static bool IsAd2 => ConfigurationManager.Instance.SampleType == SampleType.Ad2;
    private static bool IsAd3 => ConfigurationManager.Instance.SampleType == SampleType.Ad3;

    private static string PlacementID1 => ConfigurationManager.Instance.PlacementID1;
    private static string PlacementID2 => ConfigurationManager.Instance.PlacementID2;

    private void Start() {
        if (rewardedAd != null) {
            rewardedAd.Dispose();
            rewardedAd = null;
        }

        if (rewardedAd2 != null) {
            rewardedAd2.Dispose();
            rewardedAd2 = null;
        }

        VAMP.SDK.TestMode = ConfigurationManager.Instance.TestMode;
        VAMP.SDK.DebugMode = ConfigurationManager.Instance.DebugMode;
        infoText.text = $"[Test:{VAMP.SDK.TestMode}][Debug:{VAMP.SDK.DebugMode}]";

        VAMP.DebugUtils.LogSDKDetails();

        loadButton.gameObject.SetActive(IsAd1);
        showButton.gameObject.SetActive(IsAd1);
        loadAndShowButton.gameObject.SetActive(IsAd2);
        loadButton1.gameObject.SetActive(IsAd3);
        showButton1.gameObject.SetActive(IsAd3);
        loadButton2.gameObject.SetActive(IsAd3);
        showButton2.gameObject.SetActive(IsAd3);
        messageText.text = string.Empty;
        messageText.gameObject.SetActive(false);

        var soundTexture = audioSource.isPlaying ? soundOffImage : soundOnImage;
        var soundImage = soundButton.transform.Find("Image").GetComponent<Image>();
        soundImage.sprite = Sprite.Create(soundTexture, new Rect(0, 0, soundTexture.width, soundTexture.height), Vector2.zero);
        soundButton.onClick.AddListener(() =>
        {
            if (audioSource.isPlaying) {
                audioSource.Stop();
                soundImage.sprite = Sprite.Create(soundOnImage, new Rect(0, 0, soundOnImage.width, soundOnImage.height), Vector2.zero);
            }
            else {
                audioSource.Play();
                soundImage.sprite = Sprite.Create(soundOffImage, new Rect(0, 0, soundOffImage.width, soundOffImage.height), Vector2.zero);
            }
        });

        if (IsAd1) {
            SetupAd1();
        }
        else if (IsAd2) {
            SetupAd2();
        }
        else if (IsAd3) {
            SetupAd3();
        }
    }

    private void SetupAd1() {
        rewardedAd = new VAMP.RewardedAd(PlacementID1);
        SetEventHandlers(rewardedAd);

        infoText.text += $"\nID:{PlacementID1}";

        loadButton.onClick.AddListener(() =>
        {
            AddMessage("click load button.");

            Debug.Log("[VAMPUnitySDK] RewardedAd.Load()");

            // 動画広告のロードを開始します
            rewardedAd.Load(CreateRequest());
        });

        showButton.onClick.AddListener(() =>
        {
            AddMessage("click show button.");

            // 動画広告が準備できているときは広告を表示します
            if (rewardedAd.IsReady) {
                Debug.Log("[VAMPUnitySDK] RewardedAd.Show()");

                PauseSound();

                rewardedAd.Show();
            }
            else {
                AddMessage("Not loaded.");
            }
        });
    }

    private void SetupAd2() {
        rewardedAd = new VAMP.RewardedAd(PlacementID1);
        SetEventHandlers(rewardedAd);

        infoText.text += $"\nID:{PlacementID1}";

        loadAndShowButton.onClick.AddListener(() =>
        {
            AddMessage("click load & show button.");

            if (!rewardedAd.IsReady) {
                Debug.Log("[VAMPUnitySDK] RewardedAd.Load()");

                // 動画広告のロードを開始します
                rewardedAd.Load(CreateRequest());
            }
            else {
                Debug.Log("[VAMPUnitySDK] RewardedAd.Show()");

                PauseSound();

                // 動画広告が準備できているときは広告を表示します
                rewardedAd.Show();
            }
        });

        rewardedAd.Preload(CreateRequest());
    }

    private void SetupAd3() {
        rewardedAd = new VAMP.RewardedAd(PlacementID1);
        SetEventHandlers(rewardedAd);
        rewardedAd2 = new VAMP.RewardedAd(PlacementID2);
        SetEventHandlers(rewardedAd2);

        infoText.text += $"\nID1:{PlacementID1} ID2:{PlacementID2}";

        loadButton1.onClick.AddListener(() =>
        {
            AddMessage("click load button 1.");

            Debug.Log("[VAMPUnitySDK] RewardedAd1.Load()");

            // 動画広告のロードを開始します
            rewardedAd.Load(CreateRequest());
        });

        showButton1.onClick.AddListener(() =>
        {
            AddMessage("click show button 1.");

            // 動画広告が準備できているときは広告を表示します
            if (rewardedAd.IsReady) {
                Debug.Log("[VAMPUnitySDK] RewardedAd1.Show()");

                PauseSound();

                rewardedAd.Show();
            }
            else {
                AddMessage("Ad1 not loaded.");
            }

            rewardedAd2.Load(CreateRequest());
        });

        loadButton2.onClick.AddListener(() =>
        {
            AddMessage("click load button 2.");

            Debug.Log("[VAMPUnitySDK] RewardedAd2.Load()");

            // 動画広告のロードを開始します
            rewardedAd2.Load(CreateRequest());
        });

        showButton2.onClick.AddListener(() =>
        {
            AddMessage("click show button 2.");

            // 動画広告が準備できているときは広告を表示します
            if (rewardedAd2.IsReady) {
                Debug.Log("[VAMPUnitySDK] RewardedAd2.Show()");

                PauseSound();

                rewardedAd2.Show();
            }
            else {
                AddMessage("Ad2 not loaded.");
            }

            rewardedAd.Load(CreateRequest());
        });
    }

    private void PauseSound() {
        isPlayingPrev = audioSource.isPlaying;
        if (audioSource.isPlaying) {
            audioSource.Pause();
        }
    }

    private void ResumeSound() {
        if (isPlayingPrev) {
            audioSource.UnPause();
        }
    }

    private void AddMessage(string str, MessageColor color) {
        if (color == MessageColor.Default) {
            AddMessage(str);
        }
        else {
            var htmlColor = MessageColorToHtmlStringRGBA(color);
            var message = $"<color=#{htmlColor}>{str}</color>";
            AddMessage(message);
        }
    }

    private void AddMessage(string str) {
        var text = GameObject.Instantiate(messageText, content);

        text.gameObject.SetActive(true);
        text.transform.SetAsFirstSibling();
        text.text = DateTime.Now.ToString("MM/dd HH:mm:ss ") + str;
        text.fontSize = 32;
    }

    private void SetEventHandlers(VAMP.RewardedAd ad) {
        ad.OnReceived += HandleRewardedAdDidReceive;
        ad.OnFailedToLoad += HandleRewardedAdDidFailToLoad;
        ad.OnExpired += HandleRewardedAdDidExpire;
        ad.OnStartedLoading += HandleRewardedAdDidStartLoading;
        ad.OnLoaded += HandleRewardedAdDidLoad;
        ad.OnFailedToShow += HandleRewardedAdDidFailToShow;
        ad.OnCompleted += HandleRewardedAdDidComplete;
        ad.OnOpened += HandleRewardedAdDidOpen;
        ad.OnClosed += HandleRewardedAdDidClose;
    }

    private static VAMP.Request CreateRequest() {
        return new VAMP.Request.Builder().Build();
    }

    private void HandleRewardedAdDidReceive(object sender, VAMP.AdEventArgs args) {
        MainThreadDispatcher.Instance.Dispatch(() =>
        {
            AddMessage($"onReceived({args.PlacementId})");

            Debug.Log($"[VAMPUnitySDK] OnReceived: placementId={args.PlacementId}");

            if (IsAd2) {
                PauseSound();
                rewardedAd.Show();
            }
        });
    }

    private void HandleRewardedAdDidFailToLoad(object sender, VAMP.AdFailEventArgs args) {
        MainThreadDispatcher.Instance.Dispatch(() =>
        {
            AddMessage($"onFailedToLoad({args.PlacementId}, {args.Error})", MessageColor.Red);

            Debug.Log($"[VAMPUnitySDK] OnFailedToLoad: placementId={args.PlacementId}, " +
                      $"error={args.Error}");

            ResumeSound();
        });
    }

    private void HandleRewardedAdDidExpire(object sender, VAMP.AdEventArgs args) {
        MainThreadDispatcher.Instance.Dispatch(() =>
        {
            AddMessage($"onExpired({args.PlacementId})", MessageColor.Red);

            Debug.Log($"[VAMPUnitySDK] OnExpired: placementId={args.PlacementId}");

            // 再度、広告をロードします
            rewardedAd.Load(CreateRequest());
        });
    }

    private void HandleRewardedAdDidStartLoading(object sender, VAMP.AdLoadEventArgs args) {
        MainThreadDispatcher.Instance.Dispatch(() =>
        {
            AddMessage($"onStartedLoading({args.PlacementId}, {args.AdNetworkName})");

            Debug.Log($"[VAMPUnitySDK] OnStartedLoading: placementId={args.PlacementId}, " +
                      $"adNetworkName={args.AdNetworkName}");
        });
    }

    private void HandleRewardedAdDidLoad(object sender, VAMP.AdLoadResultEventArgs args) {
        MainThreadDispatcher.Instance.Dispatch(() =>
        {
            AddMessage(
                $"onLoaded({args.PlacementId}, {args.AdNetworkName}, success: {args.IsSuccess}, message: {args.Message})");

            Debug.Log($"[VAMPUnitySDK] OnLoaded: placementId={args.PlacementId}, " +
                      $"adNetworkName={args.AdNetworkName}, " +
                      $"seqId={rewardedAd.ResponseInfo?.SeqId ?? ""}, " +
                      $"success={args.IsSuccess}, " +
                      $"message={args.Message}");
        });
    }

    private void HandleRewardedAdDidFailToShow(object sender, VAMP.AdFailEventArgs args) {
        MainThreadDispatcher.Instance.Dispatch(() =>
        {
            AddMessage($"onFailedToShow({args.PlacementId}, {args.Error})", MessageColor.Red);

            Debug.Log($"[VAMPUnitySDK] OnFailedToShow: placementId={args.PlacementId}, " +
                      $"error={args.Error}");
        });
    }

    private void HandleRewardedAdDidComplete(object sender, VAMP.AdEventArgs args) {
        MainThreadDispatcher.Instance.Dispatch(() =>
        {
            AddMessage($"onCompleted({args.PlacementId})", MessageColor.Green);

            Debug.Log($"[VAMPUnitySDK] OnCompleted: placementId={args.PlacementId}");
        });
    }

    private void HandleRewardedAdDidOpen(object sender, VAMP.AdEventArgs args) {
        MainThreadDispatcher.Instance.Dispatch(() =>
        {
            AddMessage($"onOpened({args.PlacementId})");

            Debug.Log($"[VAMPUnitySDK] OnOpened: placementId={args.PlacementId}");
        });
    }

    private void HandleRewardedAdDidClose(object sender, VAMP.AdCloseEventArgs args) {
        MainThreadDispatcher.Instance.Dispatch(() =>
        {
            AddMessage($"onClosed({args.PlacementId}, adClicked: {args.AdClicked})", MessageColor.Blue);

            Debug.Log($"[VAMPUnitySDK] OnClosed: placementId={args.PlacementId}, " +
                      $"adClicked={args.AdClicked}");

            ResumeSound();

            if (IsAd2) {
                // 必要に応じて次に表示する広告をプリロードします
                rewardedAd.Preload(CreateRequest());
            }
        });
    }

    private static string MessageColorToHtmlStringRGBA(MessageColor messageColor) {
        var color = Color.gray;

        switch (messageColor) {
            case MessageColor.Red:
                color = Color.red;
                break;
            case MessageColor.Blue:
                color = Color.blue;
                break;
            case MessageColor.Green:
                return "00aa00ff";
        }

        return ColorUtility.ToHtmlStringRGBA(color);
    }
}

public enum SampleType {
    Ad1,
    Ad2,
    Ad3
}

public enum MessageColor {
    Default,
    Red,
    Green,
    Blue
}