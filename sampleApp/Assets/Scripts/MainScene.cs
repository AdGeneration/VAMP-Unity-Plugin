using System.Collections;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_IOS
using Unity.Advertisement.IosSupport;
#endif

public class MainScene : MonoBehaviour
{
    // iOS 広告枠ID
    [SerializeField] private string iosPlacementID1 = ConfigurationManager.IOSPlacementID1;
    [SerializeField] private string iosPlacementID2 = ConfigurationManager.IOSPlacementID2;

    // Android 広告枠ID
    [SerializeField] private string androidPlacementID1 = ConfigurationManager.AndroidPlacementID1;
    [SerializeField] private string androidPlacementID2 = ConfigurationManager.AndroidPlacementID2;

    [SerializeField] private VAMP.Privacy.ChildDirected childDirected;

    [SerializeField] private VAMP.Privacy.UnderAgeOfConsent underAgeOfConsent;

    [SerializeField] private InputField adIDInputField1;
    [SerializeField] private InputField adIDInputField2;

    [SerializeField] private Toggle testModeToggle;

    [SerializeField] private Toggle debugModeToggle;

    [SerializeField] private Button ad1Button;

    [SerializeField] private Button ad2Button;

    [SerializeField] private Button ad3Button;

    [SerializeField] private Button infoButton;

    [SerializeField] private Text sdkInfoText;

    private IEnumerator Start() {
        sdkInfoText.text = $"App {Application.version} / SDK {VAMP.SDK.SDKVersion}";

        if (string.IsNullOrEmpty(ConfigurationManager.Instance.PlacementID1)) {
#if UNITY_IOS
            ConfigurationManager.Instance.PlacementID1 = iosPlacementID1;
#elif UNITY_ANDROID
            ConfigurationManager.Instance.PlacementID1 = androidPlacementID1;
#endif
        }
        if (string.IsNullOrEmpty(ConfigurationManager.Instance.PlacementID2)) {
#if UNITY_IOS
            ConfigurationManager.Instance.PlacementID2 = iosPlacementID2;
#elif UNITY_ANDROID
            ConfigurationManager.Instance.PlacementID2 = androidPlacementID2;
#endif
        }

        // Hyper IDモードを設定します。有効にするときはtrueを指定します
        // VAMP.SDK.UseHyperID = true;
        Debug.Log("[VAMPUnitySDK] UseHyperID: " + VAMP.SDK.UseHyperID);

        // COPPA対象ユーザかどうかを設定します
        VAMP.Privacy.PrivacySettings.SetChildDirected(childDirected);

        // GDPRの対象ユーザで特定の年齢未満であるかどうかを設定します
        if (underAgeOfConsent != VAMP.Privacy.UnderAgeOfConsent.Unknown) {
            VAMP.Privacy.PrivacySettings.SetUnderAgeOfConsent(underAgeOfConsent);
        }

        //// EU圏内からのアクセスか判定します
        VAMP.SDK.IsEUAccess(access =>
        {
            MainThreadDispatcher.Instance.Dispatch(() =>
            {
                Debug.Log("[VAMPUnitySDK] IsEUAccess: " + access);

                if (access) {
                    // TODO: ユーザに広告が個人に関連する情報を取得することの同意を求めます

                    // ユーザの入力を受け付けACCEPTEDまたはDENIEDをセットします
                    VAMP.Privacy.PrivacySettings.SetConsentStatus(VAMP.Privacy.ConsentStatus.Accepted);
                }
            });
        });

        VAMP.SDK.GetLocation(location =>
        {
            MainThreadDispatcher.Instance.Dispatch(() =>
            {
                sdkInfoText.text = $"App {Application.version} / SDK {VAMP.SDK.SDKVersion} / {location.CountryCode}-{location.Region}";

                // if (location.CountryCode == "US")
                // {
                //    // COPPA対象ユーザである場合はtrueを設定する
                //    VAMP.Privacy.PrivacySettings.SetChildDirected(VAMP.Privacy.ChildDirected.True);
                // }
                SDKTestUtil.CountryCode = location.CountryCode;
            });
        });

        adIDInputField1.text = ConfigurationManager.Instance.PlacementID1;
        adIDInputField1.onEndEdit.AddListener(text =>
        {
            var placementID = text.Trim();
            ConfigurationManager.Instance.PlacementID1 = placementID;
        });

        adIDInputField2.text = ConfigurationManager.Instance.PlacementID2;
        adIDInputField2.onEndEdit.AddListener(text =>
        {
            var placementID = text.Trim();
            ConfigurationManager.Instance.PlacementID2 = placementID;
        });

        testModeToggle.isOn = ConfigurationManager.Instance.TestMode;
        testModeToggle.onValueChanged.AddListener(isOn =>
        {
            ConfigurationManager.Instance.TestMode = isOn;

            Debug.Log("[VAMPUnitySDK] TestMode: " + isOn);
        });

        debugModeToggle.isOn = ConfigurationManager.Instance.DebugMode;
        debugModeToggle.onValueChanged.AddListener(isOn =>
        {
            ConfigurationManager.Instance.DebugMode = isOn;

            Debug.Log("[VAMPUnitySDK] DebugMode: " + isOn);
        });

        ad1Button.onClick.AddListener(() =>
        {
            ConfigurationManager.Instance.SampleType = SampleType.Ad1;
            SceneManager.Instance.LoadScene(Scene.AdSample);

            BackEventManager.Instance.RegisterBackEvent(() => SceneManager.Instance.LoadScene(Scene.Main));
        });

        ad2Button.onClick.AddListener(() =>
        {
            ConfigurationManager.Instance.SampleType = SampleType.Ad2;
            SceneManager.Instance.LoadScene(Scene.AdSample);

            BackEventManager.Instance.RegisterBackEvent(() => SceneManager.Instance.LoadScene(Scene.Main));
        });

        ad3Button.onClick.AddListener(() =>
        {
            ConfigurationManager.Instance.SampleType = SampleType.Ad3;
            SceneManager.Instance.LoadScene(Scene.AdSample);

            BackEventManager.Instance.RegisterBackEvent(() => SceneManager.Instance.LoadScene(Scene.Main));
        });

        infoButton.onClick.AddListener(() =>
        {
            SceneManager.Instance.LoadScene(Scene.Info);

            BackEventManager.Instance.RegisterBackEvent(() => SceneManager.Instance.LoadScene(Scene.Main));
        });

#if UNITY_IOS
        ATTrackingStatusBinding.RequestAuthorizationTracking();

        yield return new WaitWhile(() => ATTrackingStatusBinding.GetAuthorizationTrackingStatus() ==
                                   ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED);

        Debug.Log("[VAMPUnitySDK] ATT status: " + ATTrackingStatusBinding.GetAuthorizationTrackingStatus());
#endif
        yield return null;
    }
}
