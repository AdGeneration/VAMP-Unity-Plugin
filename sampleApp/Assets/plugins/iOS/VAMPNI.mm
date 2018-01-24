//
//  VAMPNI.mm
//  VAMP-Unity-Plugin ver.2.0.3+
//
//  Created by AdGeneratioin.
//  Copyright © 2018年 Supership Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import <CoreTelephony/CTTelephonyNetworkInfo.h>
#import <CoreTelephony/CTCarrier.h>

#import <VAMP/VAMP.h>

#import <AppLovinSDK/AppLovinSDK.h>
#import <FBAudienceNetwork/FBAudienceNetwork.h>
#import <GoogleMobileAds/GoogleMobileAds.h>
#import <Maio/Maio.h>
#import <NendAd/NendAd.h>
#import <Tapjoy/Tapjoy.h>
#import <UnityAds/UnityAds.h>
#import <VungleSDK/VungleSDK.h>

#pragma mark - Unity function

extern "C" void UnitySendMessage(const char *, const char *, const char *);
extern UIViewController *UnityGetGLViewController();

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
        default:
            return @"UNKNOWN";
    }
}

NSString *VAMPNIGetAdnwSDKVersion(NSString *adnwName) {
    NSString *version = @"nothing";
    
    if ([adnwName isEqualToString:@"VAMP"]) {
        version = [VAMP SDKVersion];
    }
    else if ([adnwName isEqualToString:@"Admob"]) {
        version = [NSString stringWithCString:(const char *) GoogleMobileAdsVersionString
                                     encoding:NSUTF8StringEncoding];
    }
    else if ([adnwName isEqualToString:@"AppLovin"]) {
        version = [ALSdk version];
    }
#ifdef FB_AD_SDK_VERSION
    else if ([adnwName isEqualToString:@"FAN"]) {
        version = FB_AD_SDK_VERSION;
    }
#endif
    else if ([adnwName isEqualToString:@"Maio"]) {
        version = [Maio sdkVersion];
    }
    else if ([adnwName isEqualToString:@"Nend"]) {
        NSString *ver = [NSString stringWithCString:(const char *) NendAdVersionString
                                           encoding:NSUTF8StringEncoding];
        NSError *error = nil;
        NSRegularExpression *regexp = [NSRegularExpression regularExpressionWithPattern:@"PROJECT:([a-zA-Z0-9-.]*)"
                                                                                options:0
                                                                                  error:&error];
        
        if (!error) {
            NSTextCheckingResult *match = [regexp firstMatchInString:ver options:0 range:NSMakeRange(0, ver.length)];
            version = [ver substringWithRange:[match rangeAtIndex:1]];
        }
    }
    else if ([adnwName isEqualToString:@"Tapjoy"]) {
        version = [Tapjoy getVersion];
    }
    else if ([adnwName isEqualToString:@"UnityAds"]) {
        version = [UnityAds getVersion];
    }
    else if ([adnwName isEqualToString:@"Vungle"]) {
        version = VungleSDKVersion;
    }
    
    return version;
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

@interface VAMPNI : NSObject <VAMPDelegate>

@property (nonatomic) VAMP *vamp;
@property (nonatomic, copy) NSString *gameObjName;
@property (nonatomic, readonly, getter=canUseGameObj) BOOL useGameObj;

@end

@implementation VAMPNI

static VAMPNI *_vampInstance = nil;

#pragma mark - property

- (BOOL)canUseGameObj {
    return self.gameObjName.length > 0;
}

#pragma mark - public

- (void)setVAMPWithGameObjectName:(NSString *)gameObjName
               rootViewController:(UIViewController *)viewController
                      placementId:(NSString *)placementId {
    
    self.vamp = [VAMP new];
    self.vamp.delegate = self;
    [self.vamp setPlacementId:placementId];
    [self.vamp setRootViewController:viewController];
    
    self.gameObjName = gameObjName;
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
        // ver.2.0.2以下のVAMP SDKではビルドエラーとなります
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
}

- (void)vampDidFail:(NSString *)placementId error:(VAMPError *)error {
    if (self.canUseGameObj) {
        NSString *msg = [NSString stringWithFormat:@"%@,%@", placementId,
                         VAMPNIGetErrorMessage(error.code)];
        UnitySendMessage(self.gameObjName.UTF8String, "VAMPDidFail", msg.UTF8String);
    }
}

- (void)vampDidComplete:(NSString *)placementId adnwName:(NSString *)adnwName {
    if (self.canUseGameObj) {
        NSString *msg = [NSString stringWithFormat:@"%@,%@", placementId, adnwName];
        UnitySendMessage(self.gameObjName.UTF8String, "VAMPDidComplete", msg.UTF8String);
    }
}

- (void)vampDidClose:(NSString *)placementId adnwName:(NSString *)adnwName {
    if (self.canUseGameObj) {
        NSString *msg = [NSString stringWithFormat:@"%@,%@", placementId, adnwName];
        UnitySendMessage(self.gameObjName.UTF8String, "VAMPDidClose", msg.UTF8String);
    }
}

- (void)vampLoadStart:(NSString *)placementId adnwName:(NSString *)adnwName {
    if (self.canUseGameObj) {
        NSString *msg = [NSString stringWithFormat:@"%@,%@", placementId, adnwName];
        UnitySendMessage(self.gameObjName.UTF8String, "VAMPLoadStart", msg.UTF8String);
    }
}

- (void)vampLoadResult:(NSString *)placementId success:(BOOL)success adnwName:(NSString *)adnwName
               message:(NSString *)message {
    
    if (self.canUseGameObj) {
        NSString *msg = [NSString stringWithFormat:@"%@,%@,%@,%@",
                         placementId, (success ? @"True" : @"False"), adnwName, message];
        UnitySendMessage(self.gameObjName.UTF8String, "VAMPLoadResult", msg.UTF8String);
    }
}

- (void)vampDidExpired:(NSString *)placementId {
    if (self.canUseGameObj) {
        NSString *msg = [NSString stringWithFormat:@"%@", placementId];
        UnitySendMessage(self.gameObjName.UTF8String, "VAMPDidExpired", msg.UTF8String);
    }
}

@end

#pragma mark - Bridge code

extern "C" {

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

char *VAMPUnityAdnwSDKVersion(const char *cAdnwName) {
    NSString *adnwName = [NSString stringWithCString:cAdnwName encoding:NSUTF8StringEncoding];
    
    NSString *version = VAMPNIGetAdnwSDKVersion(adnwName);
    
    if (!version) {
        version = @"";
    }
    
    char *cVersion = (char *) version.UTF8String;
    char *res = (char *) malloc(strlen(cVersion) + 1);
    strcpy(res, cVersion);
    
    return res;
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

}   // end extern "C"

