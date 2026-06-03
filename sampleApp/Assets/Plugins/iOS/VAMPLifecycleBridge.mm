//
//  VAMPLifecycleBridge.mm
//  VAMP-Unity-Plugin
//
//  App Open Ads のライフサイクル統合実装例 (iOS)。
//
//  UIApplication と UIWindowScene の両系統から active 状態変化を観察し、
//  関数ポインタ経由で C# 側 (AppOpenAdManager) に通知します。
//
//  公開マニュアル vamp/ios/ad-format/app-open-ad.mdx l.537-558 に記載されている
//  applicationDidBecomeActive + sceneDidBecomeActive の両系統 hook を実装します。
//  Unity の UnityAppController が UIApplicationDelegate を実装済みのため、
//  継承ではなく NSNotificationCenter 経由で観察します。
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

typedef void (*VAMPLifecycleCallback)(void);

// ---------------------------------------------------------------------------
// VAMPUnityLifecycleBridge
// ---------------------------------------------------------------------------

@interface VAMPUnityLifecycleBridge : NSObject

+ (instancetype) sharedInstance;
- (void) registerWithBecomeActiveCallback:(VAMPLifecycleCallback)becomeActive
                     resignActiveCallback:(VAMPLifecycleCallback)resignActive;
- (void) unregister;

@end

@implementation VAMPUnityLifecycleBridge {
    VAMPLifecycleCallback _becomeActiveCallback;
    VAMPLifecycleCallback _resignActiveCallback;
    // applicationDidBecomeActive と sceneDidBecomeActive が両方発火するケースで
    // 通知を 1 回だけ届けるための dedup フラグ。
    BOOL _becomeActiveDelivered;
}

+ (instancetype) sharedInstance {
    static VAMPUnityLifecycleBridge *instance;
    static dispatch_once_t onceToken;

    dispatch_once(&onceToken, ^{
        instance = [[VAMPUnityLifecycleBridge alloc] init];
    });
    return instance;
}

- (instancetype) init {
    if (self = [super init]) {
        _becomeActiveCallback = NULL;
        _resignActiveCallback = NULL;
        _becomeActiveDelivered = NO;
    }

    return self;
}

- (void) registerWithBecomeActiveCallback:(VAMPLifecycleCallback)becomeActive
                     resignActiveCallback:(VAMPLifecycleCallback)resignActive {
    // 二重呼び出し時の Observer 重複登録を防ぐため、先に解除する。
    // 全通知は UIApplication / UIScene の通知センター経由で届くため
    // メインスレッド保証があり、_becomeActiveDelivered の read-modify-write は安全。
    [[NSNotificationCenter defaultCenter] removeObserver:self];

    _becomeActiveCallback = becomeActive;
    _resignActiveCallback = resignActive;
    _becomeActiveDelivered = NO;

    // 登録時点で既に active なら delivered 扱いにし、登録直後の resign を
    // C# 側 (AppOpenAdManager) に届ける。C# 側は Initialize 直後に TryShowOrLoad()
    // を直接呼んで初回 active を消化する設計のため、初回 becomeActive コールバックは
    // 送らないままで整合する。
    if ([UIApplication sharedApplication].applicationState == UIApplicationStateActive) {
        _becomeActiveDelivered = YES;
    }

    NSNotificationCenter *nc = [NSNotificationCenter defaultCenter];

    // UIApplication-based (Single Window / 全アプリ共通)
    [nc addObserver:self
           selector:@selector(handleApplicationDidBecomeActive:)
               name:UIApplicationDidBecomeActiveNotification
             object:nil];
    [nc addObserver:self
           selector:@selector(handleApplicationWillResignActive:)
               name:UIApplicationWillResignActiveNotification
             object:nil];

    // UIWindowScene-based (iOS 13 以降、SceneDelegate / SwiftUI App Lifecycle)
    // Single Window アプリでは両系統が発火するが、dedup フラグで重複を排除する。
    if (@available(iOS 13.0, *)) {
        [nc addObserver:self
               selector:@selector(handleSceneDidBecomeActive:)
                   name:UISceneDidActivateNotification
                 object:nil];
        [nc addObserver:self
               selector:@selector(handleSceneWillResignActive:)
                   name:UISceneWillDeactivateNotification
                 object:nil];
    }
}

- (void) unregister {
    [[NSNotificationCenter defaultCenter] removeObserver:self];
    _becomeActiveCallback = NULL;
    _resignActiveCallback = NULL;
}

// ---------------------------------------------------------------------------
// Notification handlers
// ---------------------------------------------------------------------------

- (void) handleApplicationDidBecomeActive:(NSNotification *)notification {
    [self deliverBecomeActive];
}

- (void) handleSceneDidBecomeActive:(NSNotification *)notification {
    [self deliverBecomeActive];
}

- (void) handleApplicationWillResignActive:(NSNotification *)notification {
    [self deliverResignActive];
}

- (void) handleSceneWillResignActive:(NSNotification *)notification {
    [self deliverResignActive];
}

// ---------------------------------------------------------------------------
// Dedup helpers
// ---------------------------------------------------------------------------

- (void) deliverBecomeActive {
    if (_becomeActiveDelivered || !_becomeActiveCallback) {
        return;
    }

    _becomeActiveDelivered = YES;
    _becomeActiveCallback();
}

- (void) deliverResignActive {
    if (!_becomeActiveDelivered || !_resignActiveCallback) {
        return;
    }

    _becomeActiveDelivered = NO;
    _resignActiveCallback();
}

@end

// ---------------------------------------------------------------------------
// C bridge
// ---------------------------------------------------------------------------

#ifdef __cplusplus
extern "C" {
#endif

void VAMPUnityRegisterLifecycleBridge(VAMPLifecycleCallback onBecomeActive,
                                      VAMPLifecycleCallback onResignActive) {
    [[VAMPUnityLifecycleBridge sharedInstance]
     registerWithBecomeActiveCallback:onBecomeActive
                 resignActiveCallback:onResignActive];
}

void VAMPUnityUnregisterLifecycleBridge(void) {
    [[VAMPUnityLifecycleBridge sharedInstance] unregister];
}

#ifdef __cplusplus
}
#endif
