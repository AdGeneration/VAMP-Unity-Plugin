public class ConfigurationManager
{
    /*
     * テスト用広告枠IDを使用して広告表示を確認することができます
     * iOS: 59755
     * Android: 59756
     * (テストIDのままリリースしないでください)
     */

    // iOS 広告枠ID
    public const string IOSPlacementID1 = "59755";
    public const string IOSPlacementID2 = "59755";

    // Android 広告枠ID
    public const string AndroidPlacementID1 = "59756";
    public const string AndroidPlacementID2 = "59756";

    public string PlacementID1 {
        get; set;
    }
    public string PlacementID2 {
        get; set;
    }
    public bool DebugMode {
        get; set;
    }
    public bool TestMode {
        get; set;
    }

    public SampleType SampleType {
        get; set;
    }

    private static ConfigurationManager _instance;
    public static ConfigurationManager Instance => _instance ??= new ConfigurationManager();

    private ConfigurationManager() {
        DebugMode = true;
        TestMode = true;
#if UNITY_IOS
        PlacementID1 = IOSPlacementID1;
        PlacementID2 = IOSPlacementID2;
#elif UNITY_ANDROID
        PlacementID1 = AndroidPlacementID1;
        PlacementID2 = AndroidPlacementID2;
#endif
    }
}
