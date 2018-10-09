//
//  VAMPNI.mm
//  VAMP-Unity-Plugin ver.3.0.3
//
//  Created by AdGeneratioin.
//  Copyright © 2018年 Supership Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import <CoreTelephony/CTTelephonyNetworkInfo.h>
#import <CoreTelephony/CTCarrier.h>
#import <AdSupport/AdSupport.h>

#import <VAMP/VAMP.h>

#pragma mark - Unity function

extern "C" void UnitySendMessage(const char *, const char *, const char *);
extern UIViewController *UnityGetGLViewController();

#pragma mark - Wrapper class
// VAMPUnity Wrapper class
template<class T>
class VAMPUnityWrapper {
public:
    VAMPUnityWrapper(T *pv) {
        this->pv = pv;
    }
    
    ~VAMPUnityWrapper() {
        pv = NULL;
    }
public:
    T *pv;
};

#pragma mark - Optional function

/**
 エラーコードに対応したエラーメッセージを返します。
 このエラーメッセージはAndroid版と同じ文字列を返します
 */
NSString *VAMPNIGetErrorMessage(NSInteger code) {
    switch (code) {
        case VAMPErrorCodeNotSupportedOsVersion:
            return @"NOT_SUPPORTED_OS_VERSION";
        case VAMPErrorCodeServerError:
            return @"SERVER_ERROR";
        case VAMPErrorCodeNoAdnetwork:
            return @"NO_ADNETWORK";
        case VAMPErrorCodeNeedConnection:
            return @"NEED_CONNECTION";
        case VAMPErrorCodeMediationTimeout:
            return @"MEDIATION_TIMEOUT";
        case VAMPErrorCodeUserCancel:
            return @"USER_CANCEL";
        case VAMPErrorCodeNoAdStock:
            return @"NO_ADSTOCK";
        case VAMPErrorCodeAdnetworkError:
            return @"ADNETWORK_ERROR";
        case VAMPErrorCodeSettingError:
            return @"SETTING_ERROR";
        case VAMPErrorCodeNotLoadedAd:
            return @"NOT_LOADED_AD";
        case VAMPErrorCodeInvalidParameter:
            return @"INVALID_PARAMETER";
        case VAMPErrorCodeFrequencyCapped:
            return @"FREQUENCY_CAPPED";
        default:
            return @"UNKNOWN";
    }
}

NSString *VAMPNIGetDeviceInfo(NSString *infoName) {
    NSString *info = @"nothing";
    
    if ([infoName isEqualToString:@"DeviceName"]) {
        info = [[UIDevice currentDevice] name];
    }
    else if ([infoName isEqualToString:@"OSName"]) {
        info = [[UIDevice currentDevice] systemName];
    }
    else if ([infoName isEqualToString:@"OSVersion"]) {
        info = [[UIDevice currentDevice] systemVersion];
    }
    else if ([infoName isEqualToString:@"OSModel"]) {
        info = [[UIDevice currentDevice] model];
    }
    else if ([infoName isEqualToString:@"Carrier"]) {
        CTTelephonyNetworkInfo *networkInfo = [[CTTelephonyNetworkInfo alloc] init];
        CTCarrier *provider = [networkInfo subscriberCellularProvider];
        
        info = provider.carrierName;
    }
    else if ([infoName isEqualToString:@"ISOCountry"]) {
        CTTelephonyNetworkInfo *networkInfo = [[CTTelephonyNetworkInfo alloc] init];
        CTCarrier *provider = [networkInfo subscriberCellularProvider];
        
        info = provider.isoCountryCode;
    }
    else if ([infoName isEqualToString:@"CountryCode"]) {
        info = [[NSLocale preferredLanguages] objectAtIndex:0];
    }
    else if ([infoName isEqualToString:@"LocaleCode"]) {
        info = [[NSLocale currentLocale] objectForKey:NSLocaleIdentifier];
    }
    else if ([infoName isEqualToString:@"IDFA"]) {
        info = [[ASIdentifierManager sharedManager] advertisingIdentifier].UUIDString;
    }
    else if ([infoName isEqualToString:@"BundleID"]) {
        info = [[NSBundle mainBundle] bundleIdentifier];
    }
    else if ([infoName isEqualToString:@"AppVer"]) {
        info = [NSBundle mainBundle].infoDictionary[@"CFBundleShortVersionString"];
    }
    
    return info;
}

#pragma mark - VAMPNI

static NSString * const kVAMPNIInitializeStateStringAuto = @"AUTO";
static NSString * const kVAMPNIInitializeStateStringAll = @"ALL";
static NSString * const kVAMPNIInitializeStateStringWeight = @"WEIGHT";
static NSString * const kVAMPNIInitializeStateStringWifiOnly = @"WIFIONLY";

static NSString * const kBoolMessage_True = @"True";
static NSString * const kBoolMessage_False = @"False";

typedef void (^VAMPReceiveCallback)(const char *placementId, const char *adnwName);
typedef void (^VAMPFailToLoadCallback)(const int errorCode, const char *placementId);
typedef void (^VAMPFailToShowCallback)(const int errorCode, const char *placementId);
typedef void (^VAMPCompleteCallback)(const char *placementId, const char *adnwName);
typedef void (^VAMPCloseCallback)(const char *placementId, const char *adnwName);
typedef void (^VAMPExpireCallback)(const char *placementId);
typedef void (^VAMPLoadStartCallback)(const char *placementId, const char *adnwName);
typedef void (^VAMPLoadResultCallback)(const char *placementId, const bool success, const char *adnwName, const char *message);

@interface VAMPNI : NSObject <VAMPDelegate>

@property (nonatomic) VAMP *vamp;
@property (nonatomic, copy) NSString *gameObjName;
@property (nonatomic, readonly, getter=canUseGameObj) BOOL useGameObj;
@property (nonatomic, copy) VAMPReceiveCallback receiveCallback;
@property (nonatomic, copy) VAMPFailToLoadCallback failToLoadCallback;
@property (nonatomic, copy) VAMPFailToShowCallback failToShowCallback;
@property (nonatomic, copy) VAMPCompleteCallback completeCallback;
@property (nonatomic, copy) VAMPCloseCallback closeCallback;
@property (nonatomic, copy) VAMPExpireCallback expireCallback;
@property (nonatomic, copy) VAMPLoadStartCallback loadStartCallback;
@property (nonatomic, copy) VAMPLoadResultCallback loadResultCallback;

@end

@implementation VAMPNI

static VAMPNI *_vampInstance = nil;

#pragma mark - property

- (BOOL)canUseGameObj {
    return self.gameObjName != nil && self.gameObjName.length > 0;
}

#pragma mark - public

- (void)setVAMPWithGameObjectName:(NSString *)gameObjName
               rootViewController:(UIViewController *)viewController
                      placementId:(NSString *)placementId {
    [self setVAMP:viewController placementId:placementId];
    self.gameObjName = gameObjName;
}

- (void)setVAMP:(UIViewController *)viewController
    placementId:(NSString *)placementId {
    self.vamp = [VAMP new];
    self.vamp.delegate = self;
    [self.vamp setPlacementId:placementId];
    [self.vamp setRootViewController:viewController];
}

- (void)preload {
    if (self.vamp) {
        // VAMP ver.3.0.0から追加されたメソッドです
        [self.vamp preload];
    }
}

- (void)load {
    if (self.vamp) {
        [self.vamp load];
    }
}

- (BOOL)show {
    if (self.vamp) {
        return [self.vamp show];
    }
    return NO;
}

- (BOOL)isReady {
    if (self.vamp) {
        return [self.vamp isReady];
    }
    return NO;
}

- (void)clearLoaded {
    if (self.vamp) {
        // VAMP ver.2.0.3から追加されたメソッドです。
        // ver.3.0.0からdeprecatedになりました
        [self.vamp clearLoaded];
    }
}

#pragma mark - static public

+ (void)initializeAdnwSDK:(NSString *)placementId {
    [[VAMP new] initializeAdnwSDK:placementId];
}

+ (void)initializeAdnwSDK:(NSString *)placementId state:(NSString *)state duration:(int)duration {
    VAMPInitializeState initializeState = kVAMPInitializeStateAUTO;
    
    if ([state isEqualToString:kVAMPNIInitializeStateStringAll]) {
        initializeState = kVAMPInitializeStateALL;
    }
    else if ([state isEqualToString:kVAMPNIInitializeStateStringWeight]) {
        initializeState = kVAMPInitializeStateWEIGHT;
    }
    else if ([state isEqualToString:kVAMPNIInitializeStateStringWifiOnly]) {
        initializeState = kVAMPInitializeStateWIFIONLY;
    }
    
    [[VAMP new] initializeAdnwSDK:placementId initializeState:initializeState duration:duration];
}

+ (void)retainInstance:(VAMPNI *)vampni {
    _vampInstance = vampni;
}

#pragma mark - VAMPDelegate

- (void)vampDidReceive:(NSString *)placementId adnwName:(NSString *)adnwName {
    if (self.canUseGameObj) {
        NSString *msg = [NSString stringWithFormat:@"%@,%@", placementId, adnwName];
        UnitySendMessage(self.gameObjName.UTF8String, "VAMPDidReceive", msg.UTF8String);
    }
    
    if (self.receiveCallback) {
        const char *cPlacementId = [placementId UTF8String];
        const char *cAdnwName = [adnwName UTF8String];
        
        self.receiveCallback(cPlacementId, cAdnwName);
    }
}

- (void)vamp:(VAMP *)vamp didFailToLoadWithError:(VAMPError *)error withPlacementId:(NSString *)placementId {
    if (self.canUseGameObj) {
        NSString *msg = [NSString stringWithFormat:@"%@,%@", VAMPNIGetErrorMessage(error.code), placementId];
        UnitySendMessage(self.gameObjName.UTF8String, "VAMPDidFailToLoad", msg.UTF8String);
        // v3.0.0からVAMPDidFailはdeprecatedです。代わりにVAMPDidFailToLoadを使用してください
        UnitySendMessage(self.gameObjName.UTF8String, "VAMPDidFail", msg.UTF8String);
    }
    
    if (self.failToLoadCallback) {
        const char *cPlacementId = [placementId UTF8String];
        self.failToLoadCallback((int)error.code, cPlacementId);
    }
}

- (void)vamp:(VAMP *)vamp didFailToShowWithError:(VAMPError *)error withPlacementId:(NSString *)placementId {
    if (self.canUseGameObj) {
        NSString *msg = [NSString stringWithFormat:@"%@,%@", VAMPNIGetErrorMessage(error.code), placementId];
        UnitySendMessage(self.gameObjName.UTF8String, "VAMPDidFailToShow", msg.UTF8String);
        // v3.0.0からVAMPDidFailはdeprecatedです。代わりにVAMPDidFailToShowを使用してください
        UnitySendMessage(self.gameObjName.UTF8String, "VAMPDidFail", msg.UTF8String);
    }
    
    if (self.failToShowCallback) {
        const char *cPlacementId = [placementId UTF8String];
        self.failToShowCallback((int)error.code, cPlacementId);
    }
}

- (void)vampDidComplete:(NSString *)placementId adnwName:(NSString *)adnwName {
    if (self.canUseGameObj) {
        NSString *msg = [NSString stringWithFormat:@"%@,%@", placementId, adnwName];
        UnitySendMessage(self.gameObjName.UTF8String, "VAMPDidComplete", msg.UTF8String);
    }
    
    if (self.completeCallback) {
        const char *cPlacementId = [placementId UTF8String];
        const char *cAdnwName = [adnwName UTF8String];
        self.completeCallback(cPlacementId, cAdnwName);
    }
}

- (void)vampDidClose:(NSString *)placementId adnwName:(NSString *)adnwName {
    if (self.canUseGameObj) {
        NSString *msg = [NSString stringWithFormat:@"%@,%@", placementId, adnwName];
        UnitySendMessage(self.gameObjName.UTF8String, "VAMPDidClose", msg.UTF8String);
    }
    
    if (self.closeCallback) {
        const char *cPlacementId = [placementId UTF8String];
        const char *cAdnwName = [adnwName UTF8String];
        self.closeCallback(cPlacementId, cAdnwName);
    }
}

- (void)vampLoadStart:(NSString *)placementId adnwName:(NSString *)adnwName {
    if (self.canUseGameObj) {
        NSString *msg = [NSString stringWithFormat:@"%@,%@", placementId, adnwName];
        UnitySendMessage(self.gameObjName.UTF8String, "VAMPLoadStart", msg.UTF8String);
    }
    
    if (self.loadStartCallback) {
        const char *cPlacementId = [placementId UTF8String];
        const char *cAdnwName = [adnwName UTF8String];
        self.loadStartCallback(cPlacementId, cAdnwName);
    }
}

- (void)vampLoadResult:(NSString *)placementId success:(BOOL)success adnwName:(NSString *)adnwName
               message:(NSString *)message {
    if (self.canUseGameObj) {
        NSString *msg = [NSString stringWithFormat:@"%@,%@,%@,%@",
                         placementId, (success ? kBoolMessage_True : kBoolMessage_False), adnwName, message];
        UnitySendMessage(self.gameObjName.UTF8String, "VAMPLoadResult", msg.UTF8String);
    }
    
    if (self.loadResultCallback) {
        const char *cPlacementId = [placementId UTF8String];
        const char *cAdnwName = [adnwName UTF8String];
        const char *cMessage = [message UTF8String];
        self.loadResultCallback(cPlacementId, success, cAdnwName, cMessage);
    }
}

- (void)vampDidExpired:(NSString *)placementId {
    if (self.canUseGameObj) {
        NSString *msg = [NSString stringWithFormat:@"%@", placementId];
        UnitySendMessage(self.gameObjName.UTF8String, "VAMPDidExpired", msg.UTF8String);
    }
    
    if (self.expireCallback) {
        const char *cPlacementId = [placementId UTF8String];
        self.expireCallback(cPlacementId);
    }
}

@end

#pragma mark - Bridge code

extern "C" {
    typedef void (*VAMPUnityReceiveCallback)(const char *placementId, const char *adnwName);
    typedef void (*VAMPUnityFailToLoadCallback)(const int errorCode, const char *placementId);
    typedef void (*VAMPUnityFailToShowCallback)(const int errorCode, const char *placementId);
    typedef void (*VAMPUnityCompleteCallback)(const char* placementId, const char *adnwName);
    typedef void (*VAMPUnityCloseCallback)(const char *placementId, const char *adnwName);
    typedef void (*VAMPUnityLoadStartCallback)(const char *placementId, const char *adnwName);
    typedef void (*VAMPUnityLoadResultCallback)(const char *placementId, const bool success, const char *adnwName, const char *message);
    typedef void (*VAMPUnityExpireCallback)(const char *placementId);
    typedef void (*VAMPUnityGetCountryCodeCallback)(const char *countryCode);
    typedef void (*VAMPUnityIsEUAccessCallback)(const bool access);
    
    void *VAMPUnityInitialize(const char *cPlacementId) {
        NSString *placementId = [NSString stringWithCString:cPlacementId encoding:NSUTF8StringEncoding];
        
        VAMPNI *vampniTemp = [VAMPNI new];
        [vampniTemp setVAMP:UnityGetGLViewController() placementId:placementId];
        [VAMPNI retainInstance:vampniTemp];
        return (__bridge void*) vampniTemp;
    }
    
    void *VAMPUnityInit(void *vampni , const char *cPlacementId , const char *cObjName) {
        NSString *placementId = [NSString stringWithCString:cPlacementId encoding:NSUTF8StringEncoding];
        NSString *objName = [NSString stringWithCString:cObjName encoding:NSUTF8StringEncoding];
        
        VAMPNI *vampniTemp;
        
        if (vampni == NULL) {
            vampniTemp = [VAMPNI new];
        }
        else {
            vampniTemp = (__bridge VAMPNI *) vampni;
        }
        
        [vampniTemp setVAMPWithGameObjectName:objName rootViewController:UnityGetGLViewController()
                                  placementId:placementId];
        
        [VAMPNI retainInstance:vampniTemp];
        
        return (__bridge void *) vampniTemp;
    }
    
    void VAMPUnitySetCallbacks(void *vampni,
                               VAMPUnityReceiveCallback onReceive,
                               VAMPUnityFailToLoadCallback onFailToLoad,
                               VAMPUnityFailToShowCallback onFailToShow,
                               VAMPUnityCompleteCallback onComplete,
                               VAMPUnityCloseCallback onClose,
                               VAMPUnityExpireCallback onExpire,
                               VAMPUnityLoadStartCallback onLoadStart,
                               VAMPUnityLoadResultCallback onLoadResult) {
        VAMPNI *vampniTemp = (__bridge VAMPNI *) vampni;
        VAMPReceiveCallback receiveCallback = ^(const char *placementId, const char *adnwName) {
            if (onReceive) {
                onReceive(placementId, adnwName);
            }
        };
        
        VAMPFailToLoadCallback failToLoadCallback = ^(const int errorCode, const char *placementId) {
            if (onFailToLoad) {
                onFailToLoad(errorCode, placementId);
            }
        };
        
        VAMPFailToShowCallback failToShowCallback = ^(const int errorCode, const char *placementId) {
            if (onFailToShow) {
                onFailToShow(errorCode, placementId);
            }
        };
        
        VAMPCompleteCallback completeCallback = ^(const char *placementId, const char *adnwName) {
            if (onComplete) {
                onComplete(placementId, adnwName);
            }
        };
        
        VAMPCloseCallback closeCallback = ^(const char *placementId, const char *adnwName) {
            if (onClose) {
                onClose(placementId, adnwName);
            }
        };
        
        VAMPExpireCallback expireCallback = ^(const char *placementId) {
            if (onExpire) {
                onExpire(placementId);
            }
        };
        
        VAMPLoadStartCallback loadStartCallback = ^(const char *placementId, const char *adnwName) {
            if (onLoadStart) {
                onLoadStart(placementId, adnwName);
            }
        };
        
        VAMPLoadResultCallback loadResultCallback = ^(const char *placementId, const bool success, const char *adnwName, const char *message) {
            if (onLoadResult) {
                onLoadResult(placementId, success, adnwName, message);
            }
        };
        
        [vampniTemp setReceiveCallback:receiveCallback];
        [vampniTemp setFailToLoadCallback:failToLoadCallback];
        [vampniTemp setFailToShowCallback:failToShowCallback];
        [vampniTemp setCompleteCallback:completeCallback];
        [vampniTemp setCloseCallback:closeCallback];
        [vampniTemp setExpireCallback:expireCallback];
        [vampniTemp setLoadStartCallback:loadStartCallback];
        [vampniTemp setLoadResultCallback:loadResultCallback];
    }
    
    void VAMPUnityPreload(void *vampni) {
        [((__bridge VAMPNI *) vampni) preload];
    }
    
    void VAMPUnityLoad(void *vampni) {
        [((__bridge VAMPNI *) vampni) load];
    }
    
    bool VAMPUnityShow(void *vampni) {
        return [((__bridge VAMPNI *) vampni) show];
    }
    
    bool VAMPUnityIsReady(void *vampni) {
        return [((__bridge VAMPNI *) vampni) isReady];
    }
    
    void VAMPUnityClearLoaded(void *vampni) {
        [((__bridge VAMPNI *) vampni) clearLoaded];
    }
    
    void VAMPUnityInitializeAdnwSDK(const char *cPlacementId) {
        NSString *placementId = [NSString stringWithCString:cPlacementId encoding:NSUTF8StringEncoding];
        
        [VAMPNI initializeAdnwSDK:placementId];
    }
    
    void VAMPUnityInitializeAdnwSDKWithConfig(const char *cPlacementId, const char *cState, int duration) {
        NSString *placementId = [NSString stringWithCString:cPlacementId encoding:NSUTF8StringEncoding];
        NSString *state = [NSString stringWithCString:cState encoding:NSUTF8StringEncoding];
        
        [VAMPNI initializeAdnwSDK:placementId state:state duration:duration];
    }
    
    void VAMPUnitySetTestMode(bool enableTest) {
        [VAMP setTestMode:enableTest];
    }
    
    bool VAMPUnityIsTestMode() {
        return [VAMP isTestMode];
    }
    
    void VAMPUnitySetDebugMode(bool enableDebug) {
        [VAMP setDebugMode:enableDebug];
    }
    
    bool VAMPUnityIsDebugMode() {
        return [VAMP isDebugMode];
    }
    
    float VAMPUnitySupportedOSVersion() {
        return [VAMP SupportedOSVersion];
    }
    
    bool VAMPUnityIsSupportedOSVersion() {
        return [VAMP isSupportedOSVersion];
    }
    
    char *VAMPUnitySDKVersion() {
        NSString *version = [VAMP SDKVersion];
        
        if (!version) {
            version = @"";
        }
        
        char *cVersion = (char *) version.UTF8String;
        char *res = (char *) malloc(strlen(cVersion) + 1);
        strcpy(res, cVersion);
        
        return res;
    }
    
    void VAMPUnitySetMediationTimeout(int timeout) {
        [VAMP setMediationTimeout:(float) timeout];
    }
    
    void VAMPUnityGetCountryCode(const char *cObjName) {
        NSString *objName = [NSString stringWithCString:cObjName encoding:NSUTF8StringEncoding];
        [VAMP getCountryCode:^(NSString *countryCode) {
            UnitySendMessage(objName.UTF8String, "VAMPCountryCode", countryCode.UTF8String);
        }];
    }
    
    void VAMPUnityGetCountryCode2(VAMPUnityGetCountryCodeCallback callback) {
        
        [VAMP getCountryCode:^(NSString *countryCode) {
            if (callback) {
                callback(countryCode.UTF8String);
            }
        }];
    }
    
    // VAMP v3.0.1から追加されたメソッドです
    void VAMPUnityIsEUAccess(const char *cObjName) {
        if (![[VAMP class] respondsToSelector:@selector(isEUAccess:)]) {
            NSLog(@"VAMPUnityPlugin requires VAMP SDK v3.0.1 or higher.");
            return;
        }
        
        NSString *objName = [NSString stringWithCString:cObjName encoding:NSUTF8StringEncoding];
        
        [VAMP isEUAccess:^(BOOL access) {
            NSString *msg = access ? kBoolMessage_True : kBoolMessage_False;
            UnitySendMessage(objName.UTF8String, "VAMPIsEUAccess", msg.UTF8String);
        }];
    }
    
    void VAMPUnityIsEUAccess2(VAMPUnityIsEUAccessCallback callback) {
        if (![[VAMP class] respondsToSelector:@selector(isEUAccess:)]) {
            NSLog(@"VAMPUnityPlugin requires VAMP SDK v3.0.1 or higher.");
            return;
        }
        
        [VAMP isEUAccess:^(BOOL access) {
            if (callback) {
                callback(access);
            }
        }];
    }
    
    // VAMP v3.0.4から追加されたメソッドです
    void VAMPUnitySetFrequencyCap(const char *cPlacementId, const unsigned int impressions, const unsigned int minutes) {
        NSString *placementId = [NSString stringWithCString:cPlacementId encoding:NSUTF8StringEncoding];
        [VAMP setFrequencyCap:placementId impressions:impressions minutes:minutes];
    }
    
    void VAMPUnityClearFrequencyCap(const char *cPlacementId) {
        NSString *placementId = [NSString stringWithCString:cPlacementId encoding:NSUTF8StringEncoding];
        [VAMP clearFrequencyCap:placementId];
    }
    
    bool VAMPUnityIsFrequencyCapped(const char *cPlacementId) {
        NSString *placementId = [NSString stringWithCString:cPlacementId encoding:NSUTF8StringEncoding];
        return [VAMP isFrequencyCapped:placementId];
    }
    
    void *VAMPUnityGetFrequencyCappedStatus(const char *cPlacementId) {
        NSString *placementId = [NSString stringWithCString:cPlacementId encoding:NSUTF8StringEncoding];
        VAMPFrequencyCappedStatus *status = [VAMP getFrequencyCappedStatus:placementId];
        VAMPUnityWrapper<VAMPFrequencyCappedStatus> *wrapper = new VAMPUnityWrapper<VAMPFrequencyCappedStatus>(status);
        return wrapper;
    }
    
    void VAMPUnityResetFrequencyCap(const char *cPlacementId) {
        NSString *placementId = [NSString stringWithCString:cPlacementId encoding:NSUTF8StringEncoding];
        [VAMP resetFrequencyCap:placementId];
    }
    
    void VAMPUnityResetFrequencyCapAll() {
        [VAMP resetFrequencyCapAll];
    }
    
    // VAMP v3.0.1から追加されたメソッドです
    void VAMPUnitySetUserConsent(int consentStatus) {
        if (![[VAMP class] respondsToSelector:@selector(setUserConsent:)]) {
            NSLog(@"VAMPUnityPlugin requires VAMP SDK v3.0.1 or higher.");
            return;
        }
        
        switch (consentStatus) {
            case 1:
                [VAMP setUserConsent:kVAMPConsentStatusAccepted];
                break;
            case 2:
                [VAMP setUserConsent:kVAMPConsentStatusDenied];
                break;
            default:
                [VAMP setUserConsent:kVAMPConsentStatusUnknown];
        }
    }
    
    void VAMPUnitySetTargeting(int gender, int birthYear, int birthMonth, int birthDay) {
        switch (gender) {
            case 1:
                [VAMP setGender:kVAMPGenderMale];
                break;
            case 2:
                [VAMP setGender:kVAMPGenderFemale];
                break;
            default:
                [VAMP setGender:kVAMPGenderUnknown];
        }
        
        if (birthYear > 0 && birthMonth > 0 && birthDay > 0) {
            NSDateComponents *components = [NSDateComponents new];
            components.year = birthYear;
            components.month = birthMonth;
            components.day = birthDay;
            NSDate *date = [[[NSCalendar alloc] initWithCalendarIdentifier:NSCalendarIdentifierGregorian]
                            dateFromComponents:components];
            [VAMP setBirthday:date];
        }
    }
    
    char *VAMPUnityDeviceInfo(const char *cInfoName) {
        NSString *infoName = [NSString stringWithCString:cInfoName encoding:NSUTF8StringEncoding];
        
        NSString *info = VAMPNIGetDeviceInfo(infoName);
        
        if (!info) {
            info = @"";
        }
        
        char *cInfo = (char *) info.UTF8String;
        char *res = (char *) malloc(strlen(cInfo) + 1);
        strcpy(res, cInfo);
        
        return res;
    }
    
    void *VAMPUnityVAMPConfigurationDefaultConfiguration() {
        return (__bridge void *)[VAMPConfiguration defaultConfiguration];
    }
    
    bool VAMPUnityVAMPConfigurationIsPlayerCancelable(void *configuration) {
        return ((__bridge VAMPConfiguration *)configuration).isPlayerCancelable;
    }
    
    void VAMPUnityVAMPConfigurationSetPlayerCancelable(void *configuration, const bool playerCancelable) {
        ((__bridge VAMPConfiguration *)configuration).playerCancelable = playerCancelable;
    }
    
    char *VAMPUnityVAMPConfigurationGetPlayerAlertTitleText(void *configuration) {
        NSString *title = ((__bridge VAMPConfiguration *)configuration).playerAlertTitleText;
        
        if (!title) {
            title = @"";
        }
        
        char *cTitle = (char *) title.UTF8String;
        char *res = (char *) malloc(strlen(cTitle) + 1);
        strcpy(res, cTitle);
        
        return res;
    }
    
    void VAMPUnityVAMPConfigurationSetPlayerAlertTitleText(void *configuration, const char *cTitle) {
        NSString *title = [NSString stringWithCString:cTitle encoding:NSUTF8StringEncoding];
        ((__bridge VAMPConfiguration *)configuration).playerAlertTitleText = title;
    }
    
    char *VAMPUnityVAMPConfigurationGetPlayerAlertBodyText(void *configuration) {
        NSString *body = ((__bridge VAMPConfiguration *)configuration).playerAlertBodyText;
        if (!body) {
            body = @"";
        }
        
        char *cBody = (char *) body.UTF8String;
        char *res = (char *) malloc(strlen(cBody) + 1);
        strcpy(res, cBody);
        
        return res;
    }
    
    void VAMPUnityVAMPConfigurationSetPlayerAlertBodyText(void *configuration, const char *cBody) {
        NSString *body = [NSString stringWithCString:cBody encoding:NSUTF8StringEncoding];
        ((__bridge VAMPConfiguration *)configuration).playerAlertBodyText = body;
    }
    
    char *VAMPUnityVAMPConfigurationGetPlayerAlertCloseButtonText(void *configuration) {
        NSString *buttonText = ((__bridge VAMPConfiguration *)configuration).playerAlertCloseButtonText;
        
        if (!buttonText) {
            buttonText = @"";
        }
        
        char *cButtonText = (char *) buttonText.UTF8String;
        char *res = (char *) malloc(strlen(cButtonText) + 1);
        strcpy(res, cButtonText);
        
        return res;
    }
    
    void VAMPUnityVAMPConfigurationSetPlayerAlertCloseButtonText(void *configuration, const char *cButtonText) {
        NSString *buttonText = [NSString stringWithCString:cButtonText encoding:NSUTF8StringEncoding];
        ((__bridge VAMPConfiguration *)configuration).playerAlertCloseButtonText = buttonText;
    }
    
    char *VAMPUnityVAMPConfigurationGetPlayerAlertContinueButtonText(void *configuration) {
        NSString *buttonText = ((__bridge VAMPConfiguration *)configuration).playerAlertContinueButtonText;
        if (!buttonText) {
            buttonText = @"";
        }
        
        char *cButtonText = (char *) buttonText.UTF8String;
        char *res = (char *) malloc(strlen(cButtonText) + 1);
        strcpy(res, cButtonText);
        
        return res;
    }
    
    void VAMPUnityVAMPConfigurationSetPlayerAlertContinueButtonText(void *configuration, const char *cButtonText) {
        NSString *buttonText = [NSString stringWithCString:cButtonText encoding:NSUTF8StringEncoding];
        ((__bridge VAMPConfiguration *)configuration).playerAlertContinueButtonText = buttonText;
    }
    
    bool VAMPUnityVAMPFrequencyCappedStatusIsCapped(void *frequencyCappedStatus) {
        VAMPUnityWrapper<VAMPFrequencyCappedStatus> *wrapper = (VAMPUnityWrapper<VAMPFrequencyCappedStatus> *)frequencyCappedStatus;
        if (wrapper != NULL) {
            return wrapper->pv.isCapped;
        }
        return false;
    }
    
    unsigned int VAMPUnityVAMPFrequencyCappedStatusImpressionLimit(void *frequencyCappedStatus) {
        VAMPUnityWrapper<VAMPFrequencyCappedStatus> *wrapper = (VAMPUnityWrapper<VAMPFrequencyCappedStatus> *)frequencyCappedStatus;
        if (wrapper != NULL) {
            return wrapper->pv.impressionLimit;
        }
        return 0;
    }
    
    unsigned int VAMPUnityVAMPFrequencyCappedStatusTimeLimit(void *frequencyCappedStatus) {
        VAMPUnityWrapper<VAMPFrequencyCappedStatus> *wrapper = (VAMPUnityWrapper<VAMPFrequencyCappedStatus> *)frequencyCappedStatus;
        if (wrapper != NULL) {
            return wrapper->pv.timeLimit;
        }
        return 0;
    }
    
    unsigned int VAMPUnityVAMPFrequencyCappedStatusImpressions(void *frequencyCappedStatus) {
        VAMPUnityWrapper<VAMPFrequencyCappedStatus> *wrapper = (VAMPUnityWrapper<VAMPFrequencyCappedStatus> *)frequencyCappedStatus;
        if (wrapper != NULL) {
            return wrapper->pv.impressions;
        }
        return 0;
    }
    
    unsigned int VAMPUnityVAMPFrequencyCappedStatusRemainingTime(void *frequencyCappedStatus) {
        VAMPUnityWrapper<VAMPFrequencyCappedStatus> *wrapper = (VAMPUnityWrapper<VAMPFrequencyCappedStatus> *)frequencyCappedStatus;
        if (wrapper != NULL) {
            return wrapper->pv.remainingTime;
        }
        return 0;
    }
    
    void VAMPUnityDeleteVAMPFrequencyCappedStatus(void *frequencyCappedStatus) {
        VAMPUnityWrapper<VAMPFrequencyCappedStatus> *wrapper = (VAMPUnityWrapper<VAMPFrequencyCappedStatus> *)frequencyCappedStatus;
        if (wrapper != NULL) {
            delete wrapper;
            wrapper = NULL;
        }
    }
}   // end extern "C"
