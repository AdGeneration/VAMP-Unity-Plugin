using UnityEngine;
#if UNITY_ANDROID
using System;
#elif UNITY_IOS
using System.Runtime.InteropServices;
#endif

/// <summary>
/// App Open Ads のライフサイクル統合実装例。
///
/// 公開マニュアルの AppOpenAdManager (Android Kotlin / iOS Swift) に相当する
/// Unity C# 実装例です。初回起動時の自動表示、background → foreground 復帰時の広告表示、
/// 広告 close 後の次回用 preload を管理します。
///
/// 使い方:
///   1. 永続化された GameObject に attach します (DontDestroyOnLoad 推奨)。
///   2. 初期化タイミングで Initialize(placementId) を呼びます。
///   3. 以降はライフサイクルイベントを自動的に処理します。
///
/// メディアはこのコードを自分のアプリへコピー / 移植してください。
/// </summary>
public class AppOpenAdManager : SingletonMonoBehaviour<AppOpenAdManager>
{
    private VAMP.AppOpenAd appOpenAd;

    /// <summary>
    /// Initialize() が呼ばれて appOpenAd が確立されているかどうか。
    /// MainScene 等の再ロード時に Initialize() の二重呼び出しを避けるためのガード用。
    /// </summary>
    public bool IsInitialized => appOpenAd != null;

#if UNITY_ANDROID
    // ----- Android: 6 フラグ (公開マニュアル vamp/android/ad-format/app-open-ad.mdx:76-87 準拠) -----

    // 初回起動かどうか。最初の RunForegroundAction 完走時に false へ。
    private bool isInitialLaunch = true;
    // load が進行中か。二重 load を防ぐ。
    private bool isLoadingAd;
    // 表示中か。表示中の onStart / onActivityResumed での load / show を無視する。
    private bool isShowingAd;
    // 現在の load 完了後に show するか。初回起動時のみ true。onStop / onFailedToLoad で false へ。
    private bool showAfterCurrentLoad;
    // onStart で立てて onActivityResumed で消化する。currentActivity 未確定の onStart で
    // 直接 show / load できないため、2 段構成にする。
    private bool pendingForegroundAction;
    // 広告 close / show 失敗後の preload を次の onActivityResumed へ遅延させる。
    // close 直後は Activity 状態遷移と重なって不安定なため。
    private bool pendingPreloadAfterResume;

    private DefaultLifecycleObserverProxy lifecycleObserverProxy;
    private ActivityLifecycleCallbacksProxy activityLifecycleProxy;

#elif UNITY_IOS
    // ----- iOS: 3 フラグ (公開マニュアル vamp/ios/ad-format/app-open-ad.mdx:68-75 準拠) -----
    // Android と異なり 2 段構成が不要なのは、applicationDidBecomeActive / sceneDidBecomeActive
    // の時点で rootViewController が確定しているため。

    // 初回起動かどうか。最初の TryShowOrLoad 実行時に false へ。
    private bool isInitialLaunch = true;
    // load が進行中か。
    private bool isLoadingAppOpenAd;
    // 表示中か。
    private bool isShowingAppOpenAd;
    // 現在の load 完了後に show するか。初回起動時のみ true。resignActive で false へ。
    private bool showAfterLoad;

    // UnitySendMessage を使わず関数ポインタ経由で通知を受け取るため、
    // MonoPInvokeCallback (静的メソッド必須) からインスタンスへ転送する静的参照。
    // Android では WeakReference を使うが、iOS ではネイティブ関数ポインタを登録済みの
    // インスタンスのみが受信するため、WeakReference ではなく static 直参照で管理する。
    // volatile: ObjC 通知とメインスレッド書き込みのメモリ可視性を保証する。
    // 基底クラス SingletonMonoBehaviour<T> の private static instance とは別ものとして
    // 明示的に名前を分けている。
    private static volatile AppOpenAdManager iOSBridgeTarget;

#endif

    protected override void Awake() {
        // SingletonMonoBehaviour<T>.Awake() が Instance 重複時の自己破棄および
        // DontDestroyOnLoad を担う。派生固有の Awake 処理は base.Awake() 後に追加する。
        base.Awake();
    }

    // -------------------------------------------------------------------------
    // 公開 API
    // -------------------------------------------------------------------------

    /// <summary>
    /// ライフサイクル管理を開始します。このメソッドを呼び出した後、
    /// 初回起動時と background → foreground 復帰時に自動的に広告を表示します。
    /// placementId を変えて再度呼び出すと、既存の広告インスタンスを破棄して再初期化します。
    /// </summary>
    public void Initialize(string placementId) {
        Debug.Log($"[AppOpenAdManager] Initialize placementId={placementId}");
        Cleanup();

        appOpenAd = new VAMP.AppOpenAd(placementId);
#if UNITY_ANDROID || UNITY_IOS
        appOpenAd.OnReceived += OnAdReceived;
        appOpenAd.OnFailedToLoad += OnAdFailedToLoad;
        appOpenAd.OnOpened += OnAdOpened;
        appOpenAd.OnClosed += OnAdClosed;
        appOpenAd.OnFailedToShow += OnAdFailedToShow;
#endif

#if UNITY_ANDROID
        RegisterAndroidLifecycleObservers();
#elif UNITY_IOS
        RegisteriOSLifecycleBridge();
#endif
    }

    private void OnDestroy() {
        Cleanup();
    }

    private void Cleanup() {
        if (appOpenAd == null) {
            return;
        }

#if UNITY_ANDROID
        UnregisterAndroidLifecycleObservers();
        isInitialLaunch = true;
        isLoadingAd = false;
        isShowingAd = false;
        showAfterCurrentLoad = false;
        pendingForegroundAction = false;
        pendingPreloadAfterResume = false;
#elif UNITY_IOS
        UnregisteriOSLifecycleBridge();
        isInitialLaunch = true;
        isLoadingAppOpenAd = false;
        isShowingAppOpenAd = false;
        showAfterLoad = false;
#endif
#if UNITY_ANDROID || UNITY_IOS
        appOpenAd.OnReceived -= OnAdReceived;
        appOpenAd.OnFailedToLoad -= OnAdFailedToLoad;
        appOpenAd.OnOpened -= OnAdOpened;
        appOpenAd.OnClosed -= OnAdClosed;
        appOpenAd.OnFailedToShow -= OnAdFailedToShow;
#endif
        appOpenAd.Dispose();
        appOpenAd = null;
    }

#if UNITY_ANDROID

    // =========================================================================
    // Android: lifecycle observer の登録 / 解除
    // =========================================================================

    private void RegisterAndroidLifecycleObservers() {
        if (lifecycleObserverProxy != null) {
            return;
        }

        using (var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (var activity = player.GetStatic<AndroidJavaObject>("currentActivity")) {
                if (activity == null) {
                    Debug.LogError("[AppOpenAdManager] currentActivity is null. Initialize aborted.");
                    // appOpenAd を破棄して IsInitialized=false に戻す (lifecycle 観測未登録なのに
                    // appOpenAd だけ残ると MainScene 側の !IsInitialized ガードが通らず、
                    // Initialize 再試行も阻まれる状態不整合になるため)。
                    appOpenAd.OnReceived -= OnAdReceived;
                    appOpenAd.OnFailedToLoad -= OnAdFailedToLoad;
                    appOpenAd.OnOpened -= OnAdOpened;
                    appOpenAd.OnClosed -= OnAdClosed;
                    appOpenAd.OnFailedToShow -= OnAdFailedToShow;
                    appOpenAd.Dispose();
                    appOpenAd = null;
                    return;
                }

                // proxy 生成は activity 取得後に行う。activity が null の早期 return 時に
                // proxy だけ非 null で残ると、次回 Initialize() で冒頭ガードに引っかかり
                // observer が永遠に登録されない。
                lifecycleObserverProxy = new DefaultLifecycleObserverProxy(this);
                activityLifecycleProxy = new ActivityLifecycleCallbacksProxy(this);

                // ProcessLifecycleOwner への observer 追加はメインスレッドで行う。
                // Initialize() は Unity C# メインスレッド (= Android UI スレッド) から呼ばれるため
                // runOnUiThread は同期実行される。addObserver 内部でライフサイクル catch-up が起き、
                // 既に STARTED 状態なら ON_START が同期的に届いて pendingForegroundAction = true になる。
                activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    using (var cls = new AndroidJavaClass("androidx.lifecycle.ProcessLifecycleOwner"))
                        using (var owner = cls.CallStatic<AndroidJavaObject>("get"))
                            using (var lifecycle = owner.Call<AndroidJavaObject>("getLifecycle")) {
                                lifecycle.Call("addObserver", lifecycleObserverProxy);
                            }
                }));

                using (var application = activity.Call<AndroidJavaObject>("getApplication")) {
                    application.Call("registerActivityLifecycleCallbacks", activityLifecycleProxy);
                }
            }

        // pendingForegroundAction: catch-up で ON_START が同期到達した場合 (通常パス) に立つ。
        // isInitialLaunch: catch-up が同期発火しない端末 (Pixel 7 Pro Android 17 beta 等) でも
        //                  cold start 時に 1 回は RunForegroundAction を実行する fallback。
        //                  iOS の RegisteriOSLifecycleBridge 末尾 TryShowOrLoad() と等価設計。
        //                  RunForegroundAction 内で isInitialLaunch = false に更新されるため二重発火しない。
        if (pendingForegroundAction || isInitialLaunch) {
            RunForegroundAction();
        }
    }

    private void UnregisterAndroidLifecycleObservers() {
        var proxy = lifecycleObserverProxy;
        var actProxy = activityLifecycleProxy;

        lifecycleObserverProxy = null;
        activityLifecycleProxy = null;

        if (proxy == null && actProxy == null) {
            return;
        }

        using (var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (var activity = player.GetStatic<AndroidJavaObject>("currentActivity")) {
                if (activity == null) {
                    return;
                }

                if (proxy != null) {
                    activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                    {
                        using (var cls = new AndroidJavaClass("androidx.lifecycle.ProcessLifecycleOwner"))
                            using (var owner = cls.CallStatic<AndroidJavaObject>("get"))
                                using (var lifecycle = owner.Call<AndroidJavaObject>("getLifecycle")) {
                                    lifecycle.Call("removeObserver", proxy);
                                }
                    }));
                }

                if (actProxy != null) {
                    using (var application = activity.Call<AndroidJavaObject>("getApplication")) {
                        application.Call("unregisterActivityLifecycleCallbacks", actProxy);
                    }
                }
            }
    }

    // =========================================================================
    // Android: lifecycle イベントハンドラ
    // =========================================================================

    // DefaultLifecycleObserverProxy から呼ばれる (ON_START)。
    // ProcessLifecycleOwner の onStart はプロセスがフォアグラウンドに出た瞬間だが、
    // この時点で currentActivity が未確定なため load / show はせず flag だけ立てる。
    private void OnAppStart() {
        pendingForegroundAction = true;
    }

    // DefaultLifecycleObserverProxy から呼ばれる (ON_STOP)。
    // バックグラウンドへ遷移したため、進行中 load 後の即 show 要求を取り下げる。
    private void OnAppStop() {
        showAfterCurrentLoad = false;
        pendingForegroundAction = false;
    }

    // ActivityLifecycleCallbacksProxy から呼ばれる。
    // currentActivity が確定したタイミングで show / load を判断する。
    private void OnActivityResumed(AndroidJavaObject activity) {
        if (isShowingAd) {
            return;
        }

        RunDeferredActions();

        if (!pendingForegroundAction) {
            return;
        }

        RunForegroundAction();
    }

    // 広告 close 後の preload など、次の onActivityResumed まで遅延させた処理を実行する。
    private void RunDeferredActions() {
        if (!pendingPreloadAfterResume) {
            return;
        }

        pendingPreloadAfterResume = false;

        if (appOpenAd == null || appOpenAd.IsReady || isLoadingAd) {
            return;
        }

        LoadAd(showAfterLoad: false);
    }

    // フォアグラウンド復帰時の show / load 判定。
    // currentActivity が確定している onActivityResumed からのみ呼ぶ。
    private void RunForegroundAction() {
        Debug.Log($"[AppOpenAdManager] RunForegroundAction isInitialLaunch={isInitialLaunch} pendingForegroundAction={pendingForegroundAction}");
        pendingForegroundAction = false;

        if (ShowAdIfAvailable()) {
            return;
        }

        // ready でなかった場合: 初回起動時のみ load 完了後に即 show する。
        bool showAfterLoad = isInitialLaunch;
        isInitialLaunch = false;
        LoadAd(showAfterLoad);
    }

    // 広告が ready なら表示して true を返す。
    private bool ShowAdIfAvailable() {
        // IsReady は JNI 経由で native を叩くため、log と判定の 2 重呼び出しを避けるためキャッシュする。
        bool isReady = appOpenAd?.IsReady ?? false;

        Debug.Log($"[AppOpenAdManager] ShowAdIfAvailable isReady={isReady}");
        if (appOpenAd == null || !isReady) {
            return false;
        }

        isShowingAd = true;
        pendingPreloadAfterResume = false;
        appOpenAd.Show();
        return true;
    }

    private void LoadAd(bool showAfterLoad) {
        Debug.Log($"[AppOpenAdManager] LoadAd showAfterLoad={showAfterLoad} isLoadingAd={isLoadingAd}");
        if (isLoadingAd) {
            return;
        }

        isLoadingAd = true;
        showAfterCurrentLoad = showAfterLoad;
        appOpenAd.Load(new VAMP.Request.Builder().Build());
    }

    // =========================================================================
    // Android: AppOpenAd イベントハンドラ
    // =========================================================================

    private void OnAdReceived(object sender, VAMP.AdEventArgs e) {
        Debug.Log($"[AppOpenAdManager] onReceived placementId={e.PlacementId}");
        isLoadingAd = false;

        if (!showAfterCurrentLoad) {
            return;
        }

        showAfterCurrentLoad = false;
        isShowingAd = true;
        appOpenAd.Show();
    }

    private void OnAdFailedToLoad(object sender, VAMP.AdFailEventArgs e) {
        isLoadingAd = false;
        showAfterCurrentLoad = false;
        pendingForegroundAction = false;
        Debug.Log($"[AppOpenAdManager] onFailedToLoad placementId={e.PlacementId} error={e.Error}");
    }

    private void OnAdOpened(object sender, VAMP.AdEventArgs e) {
        Debug.Log($"[AppOpenAdManager] onOpened placementId={e.PlacementId}");
    }

    private void OnAdClosed(object sender, VAMP.AdCloseEventArgs e) {
        isShowingAd = false;
        pendingPreloadAfterResume = true;
        Debug.Log($"[AppOpenAdManager] onClosed placementId={e.PlacementId} adClicked={e.AdClicked}");
    }

    private void OnAdFailedToShow(object sender, VAMP.AdFailEventArgs e) {
        isShowingAd = false;
        pendingPreloadAfterResume = true;
        Debug.Log($"[AppOpenAdManager] onFailedToShow placementId={e.PlacementId} error={e.Error}");
    }

    // =========================================================================
    // Android: AndroidJavaProxy 実装
    // =========================================================================

    // androidx.lifecycle.LifecycleEventObserver の実装。
    // onStart (ON_START) と onStop (ON_STOP) のみ処理し、他は無視する。
    private class DefaultLifecycleObserverProxy : AndroidJavaProxy
    {
        private readonly WeakReference<AppOpenAdManager> managerRef;

        public DefaultLifecycleObserverProxy(AppOpenAdManager manager)
            : base("androidx.lifecycle.LifecycleEventObserver") {
            managerRef = new WeakReference<AppOpenAdManager>(manager);
        }

        public void onStateChanged(AndroidJavaObject source, AndroidJavaObject lifecycleEvent) {
            if (!managerRef.TryGetTarget(out var manager)) {
                return;
            }

            string eventName = lifecycleEvent.Call<string>("name");
            switch (eventName) {
                case "ON_START":
                    manager.OnAppStart();
                    break;
                case "ON_STOP":
                    manager.OnAppStop();
                    break;
            }
        }
    }

    // android.app.Application.ActivityLifecycleCallbacks の実装。
    // onActivityResumed のみ処理し、他は no-op とする。
    private class ActivityLifecycleCallbacksProxy : AndroidJavaProxy
    {
        private readonly WeakReference<AppOpenAdManager> managerRef;

        public ActivityLifecycleCallbacksProxy(AppOpenAdManager manager)
            : base("android.app.Application$ActivityLifecycleCallbacks") {
            managerRef = new WeakReference<AppOpenAdManager>(manager);
        }

        public void onActivityResumed(AndroidJavaObject activity) {
            if (!managerRef.TryGetTarget(out var manager)) {
                return;
            }

            manager.OnActivityResumed(activity);
        }

        // no-op: AndroidJavaProxy が未実装メソッドを自動的にデフォルト値で処理する
        public void onActivityCreated(AndroidJavaObject activity, AndroidJavaObject savedInstanceState) {
        }
        public void onActivityStarted(AndroidJavaObject activity) {
        }
        public void onActivityPaused(AndroidJavaObject activity) {
        }
        public void onActivityStopped(AndroidJavaObject activity) {
        }
        public void onActivitySaveInstanceState(AndroidJavaObject activity, AndroidJavaObject outState) {
        }
        public void onActivityDestroyed(AndroidJavaObject activity) {
        }
    }

#elif UNITY_IOS

    // =========================================================================
    // iOS: lifecycle ブリッジの登録 / 解除
    // =========================================================================

    private delegate void LifecycleCallback();

    [DllImport("__Internal")]
    private static extern void VAMPUnityRegisterLifecycleBridge(LifecycleCallback onBecomeActive,
                                                                LifecycleCallback onResignActive);

    [DllImport("__Internal")]
    private static extern void VAMPUnityUnregisterLifecycleBridge();

    private void RegisteriOSLifecycleBridge() {
        iOSBridgeTarget = this;
        VAMPUnityRegisterLifecycleBridge(OnBecomeActiveStatic, OnResignActiveStatic);

        // Initialize() 呼び出し時点でアプリはすでに active 状態であり、
        // UIApplicationDidBecomeActiveNotification は過去に発火済みで今後は来ない。
        // そのため初回分を直接 TryShowOrLoad() で処理する。
        TryShowOrLoad();
    }

    private void UnregisteriOSLifecycleBridge() {
        VAMPUnityUnregisterLifecycleBridge();
        iOSBridgeTarget = null;
    }

    // =========================================================================
    // iOS: ObjC から関数ポインタ経由で呼ばれるメソッド
    // =========================================================================

    // MonoPInvokeCallback は静的メソッド必須のため、静的→インスタンスへ転送する。
    [AOT.MonoPInvokeCallback(typeof(LifecycleCallback))]
    private static void OnBecomeActiveStatic() {
        iOSBridgeTarget?.OnApplicationBecomeActive();
    }

    [AOT.MonoPInvokeCallback(typeof(LifecycleCallback))]
    private static void OnResignActiveStatic() {
        iOSBridgeTarget?.OnApplicationResignActive();
    }

    // applicationDidBecomeActive / sceneDidBecomeActive (どちらか一方のみ、ObjC 側で重複排除済み)。
    private void OnApplicationBecomeActive() {
        if (isShowingAppOpenAd) {
            return;
        }

        TryShowOrLoad();
    }

    // applicationWillResignActive / sceneWillResignActive (同上、ObjC 側で重複排除済み)。
    private void OnApplicationResignActive() {
        // バックグラウンドへ出た場合、進行中 load 完了後の即 show を取り下げる。
        showAfterLoad = false;
    }

    // =========================================================================
    // iOS: 状態機械ヘルパー
    // =========================================================================

    private void TryShowOrLoad() {
        // IsReady は P/Invoke 経由で native を叩くため、log と判定の 2 重呼び出しを避けるためキャッシュする。
        bool isReady = appOpenAd?.IsReady ?? false;

        Debug.Log($"[AppOpenAdManager] TryShowOrLoad isInitialLaunch={isInitialLaunch} isShowingAppOpenAd={isShowingAppOpenAd} isLoadingAppOpenAd={isLoadingAppOpenAd} isReady={isReady}");
        if (appOpenAd == null) {
            return;
        }

        if (isReady) {
            isShowingAppOpenAd = true;
            appOpenAd.Show();
            return;
        }

        if (isLoadingAppOpenAd) {
            return;
        }

        // 初回起動時のみ load 完了後に即 show する。
        showAfterLoad = isInitialLaunch;
        isInitialLaunch = false;
        isLoadingAppOpenAd = true;
        appOpenAd.Load(new VAMP.Request.Builder().Build());
    }

    // =========================================================================
    // iOS: AppOpenAd イベントハンドラ
    // =========================================================================

    private void OnAdReceived(object sender, VAMP.AdEventArgs e) {
        Debug.Log($"[AppOpenAdManager] onReceived placementId={e.PlacementId}");
        isLoadingAppOpenAd = false;

        if (!showAfterLoad) {
            return;
        }

        showAfterLoad = false;
        isShowingAppOpenAd = true;
        appOpenAd.Show();
    }

    private void OnAdFailedToLoad(object sender, VAMP.AdFailEventArgs e) {
        isLoadingAppOpenAd = false;
        showAfterLoad = false;
        Debug.Log($"[AppOpenAdManager] onFailedToLoad placementId={e.PlacementId} error={e.Error}");
    }

    private void OnAdOpened(object sender, VAMP.AdEventArgs e) {
        Debug.Log($"[AppOpenAdManager] onOpened placementId={e.PlacementId}");
    }

    private void OnAdClosed(object sender, VAMP.AdCloseEventArgs e) {
        isShowingAppOpenAd = false;
        Debug.Log($"[AppOpenAdManager] onClosed placementId={e.PlacementId} adClicked={e.AdClicked}");

        // close 後は次回フォアグラウンド用に preload しておく。
        if (!isLoadingAppOpenAd) {
            isLoadingAppOpenAd = true;
            appOpenAd?.Load(new VAMP.Request.Builder().Build());
        }
    }

    private void OnAdFailedToShow(object sender, VAMP.AdFailEventArgs e) {
        isShowingAppOpenAd = false;
        Debug.Log($"[AppOpenAdManager] onFailedToShow placementId={e.PlacementId} error={e.Error}");

        // show 失敗後も次回フォアグラウンド用に preload する。
        if (!isLoadingAppOpenAd) {
            isLoadingAppOpenAd = true;
            appOpenAd?.Load(new VAMP.Request.Builder().Build());
        }
    }

#endif
}
