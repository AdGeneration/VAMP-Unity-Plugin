using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class AppOpenAdSample : MonoBehaviour
{
    [SerializeField]
    [Tooltip("App Open Ads 用の placement ID。Pangle のみ対応。")]
    private string placementId = "";

    [SerializeField]
    private Button loadButton;

    [SerializeField]
    private Button showButton;

    [SerializeField]
    private Text infoText;

    [SerializeField]
    private ScrollLogText scrollLog;

    private SynchronizationContext mainThreadContext;
    private VAMP.AppOpenAd appOpenAd;

    // ラムダをフィールドに保持して OnDestroy で -= による解除を可能にする。
    private EventHandler<VAMP.AdEventArgs> onReceived;
    private EventHandler<VAMP.AdFailEventArgs> onFailedToLoad;
    private EventHandler<VAMP.AdEventArgs> onOpened;
    private EventHandler<VAMP.AdCloseEventArgs> onClosed;
    private EventHandler<VAMP.AdFailEventArgs> onFailedToShow;

    private void Awake() {
        mainThreadContext = SynchronizationContext.Current;
    }

    private void Start() {
        // ホームの InputField で設定された AppOpen 専用 ID があれば Inspector 値より優先する。
        // (ConfigurationManager.AppOpenAdPlacementID は MainScene で onEndEdit から更新される)
        var configPlacementID = ConfigurationManager.Instance.AppOpenAdPlacementID;

        if (!string.IsNullOrEmpty(configPlacementID)) {
            placementId = configPlacementID;
        }

        // MainScene を経由せず直接起動された場合でも表示と SDK 設定がズレないよう、
        // AdSample と同様に ConfigurationManager の値を SDK へ反映してから表示する。
        VAMP.SDK.TestMode = ConfigurationManager.Instance.TestMode;
        VAMP.SDK.DebugMode = ConfigurationManager.Instance.DebugMode;

        infoText.text = $"[TestMode:{VAMP.SDK.TestMode}] ID:{placementId}";

        if (string.IsNullOrEmpty(placementId)) {
            AddMessage("placementId is not set. Set it on the home (MainScene) InputField, or configure it via Inspector.");
            loadButton.interactable = false;
            showButton.interactable = false;
            return;
        }

        appOpenAd = new VAMP.AppOpenAd(placementId);

        onReceived = (s, e) =>
            mainThreadContext.Post(_ => {
            loadButton.interactable = true;
            showButton.interactable = true;
            AddMessage($"onReceived({e.PlacementId})");
        }, null);
        onFailedToLoad = (s, e) =>
            mainThreadContext.Post(_ => {
            loadButton.interactable = true;
            // Load 失敗時は次回 Load 成功 (onReceived) まで Show 無効を維持する意図を明確化。
            showButton.interactable = false;
            AddMessage($"onFailedToLoad({e.PlacementId}, {e.Error})");
        }, null);
        onOpened = (s, e) =>
            mainThreadContext.Post(_ => AddMessage($"onOpened({e.PlacementId})"), null);
        onClosed = (s, e) =>
            mainThreadContext.Post(_ => AddMessage($"onClosed({e.PlacementId}, adClicked={e.AdClicked})"), null);
        onFailedToShow = (s, e) =>
            mainThreadContext.Post(_ => {
            loadButton.interactable = true;
            AddMessage($"onFailedToShow({e.PlacementId}, {e.Error})");
        }, null);

        appOpenAd.OnReceived += onReceived;
        appOpenAd.OnFailedToLoad += onFailedToLoad;
        appOpenAd.OnOpened += onOpened;
        appOpenAd.OnClosed += onClosed;
        appOpenAd.OnFailedToShow += onFailedToShow;

        loadButton.onClick.AddListener(() =>
        {
            AddMessage("click load button.");
            loadButton.interactable = false;
            showButton.interactable = false;
            appOpenAd.Load(new VAMP.Request.Builder().Build());
        });

        showButton.onClick.AddListener(() =>
        {
            AddMessage("click show button.");
            if (appOpenAd.IsReady) {
                appOpenAd.Show();
            }
            else {
                AddMessage("Not loaded.");
            }
        });
    }

    private void OnDestroy() {
        if (appOpenAd != null) {
            appOpenAd.OnReceived -= onReceived;
            appOpenAd.OnFailedToLoad -= onFailedToLoad;
            appOpenAd.OnOpened -= onOpened;
            appOpenAd.OnClosed -= onClosed;
            appOpenAd.OnFailedToShow -= onFailedToShow;
            appOpenAd.Dispose();
            appOpenAd = null;
        }
    }

    private void AddMessage(string str) {
        var line = DateTime.Now.ToString("MM/dd HH:mm:ss ") + str;

        Debug.Log("[AppOpenAdSample] " + line);
        scrollLog?.AddLine(line);
    }
}
