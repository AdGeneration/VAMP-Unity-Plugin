#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import <CoreTelephony/CTTelephonyNetworkInfo.h>
#import <CoreTelephony/CTCarrier.h>

#import <VAMP/VAMP.h>

#import <AppLovinSDK/AppLovinSDK.h>
#import <Maio/Maio.h>
#import <UnityAds/UnityAds.h>
#import <ADGPlayer/ADGPlayer.h>
#import <VungleSDK/VungleSDK.h>
#import <GoogleMobileAds/GoogleMobileAds.h>
#import <FBAudienceNetwork/FBAudienceNetwork.h>

extern "C" void UnitySendMessage(const char *, const char *, const char *);
extern UIViewController *UnityGetGLViewController();

#pragma mark - Objective-C code

/**
 * VAMP の情報を保持する
 *
 */
@interface VAMPNI : NSObject<VAMPDelegate>

@property (nonatomic, strong) VAMP *adReward;
@property (nonatomic, strong) NSString *pubId;
@property (nonatomic, strong) UIViewController * viewCon;
@property (nonatomic, strong) NSString *gameObjName;


@end

@implementation VAMPNI

static NSMutableArray *_stockInstance;

- (void)setParams:(UIViewController *)viewCon pubId:(NSString *)pubId enableTestMode:(BOOL)enableTestMode enableDebugMode:(BOOL)enableDebugMode objName:(NSString *)objName{
    
    self.viewCon = viewCon;
    self.pubId = pubId;
    
    self.adReward = [[VAMP alloc] init];
    [self.adReward setPlacementId:self.pubId];
    self.adReward.delegate = self;
    [self.adReward setRootViewController:self.viewCon];
    
    [VAMP setDebugMode:enableDebugMode];
    [VAMP setTestMode:enableTestMode];
    
    //[self.adReward load];
    
    self.gameObjName = [NSString stringWithString:objName];
    
}

/**
 * リワード広告の読み込みを行う
 *
 */
- (void)loadRequest{
    if (self.adReward) {
        [self.adReward load];
    }
}

/**
 * リワード広告の表示を行う
 *
 */
- (BOOL)show{
    if (self.adReward) {
        return [self.adReward show];
    }
    return NO;
}

/**
 * 呼び出し元のオブジェクト名があるかのチェック
 *
 */
- (BOOL)canUseGameObj{
    if(!self.gameObjName)return NO;
    return [self.gameObjName length] > 0;
}

-(void) addIntance
{
    if (!_stockInstance) {
        _stockInstance = [NSMutableArray array];
    }
    
    [_stockInstance addObject:self];
}

+ (void)setTestMode:(BOOL)enableTestMode
{
    [VAMP setTestMode:enableTestMode];
}

+ (BOOL)isTestMode
{
    return [VAMP isTestMode];
}

+ (void)setDebugMode:(BOOL)enableDebugMode
{
    
    [VAMP setDebugMode:enableDebugMode];
}

+ (BOOL)isDebugMode
{
    return [VAMP isDebugMode];
}

+ (float)supportedOSVersion
{
    return [VAMP SupportedOSVersion];
}

+ (BOOL)isSupportedOSVersion
{
    return [VAMP isSupportedOSVersion];
}

+ (NSString *)SDKVersion
{
    return [VAMP SDKVersion];
}

-(void)initializeAdnwSDK:(NSString *)pubId
{
    if (self.adReward) {
        [self.adReward initializeAdnwSDK:pubId];
    }
    
}

-(void) initializeAdnwSDK:(NSString *)pubId state:(NSString *)state duration:(int)duration
{
    if (self.adReward) {
        VAMPInitializeState initializeState = kVAMPInitializeStateAUTO;
        
        if (state != nil) {
            if ([state isEqualToString:@"ALL"]) {
                initializeState = kVAMPInitializeStateALL;
            }
            else if ([state isEqualToString:@"WEIGHT"]) {
                initializeState = kVAMPInitializeStateWEIGHT;
            }
            else if ([state isEqualToString:@"WIFIONLY"]) {
                initializeState = kVAMPInitializeStateWIFIONLY;
            }
        }
        
        [self.adReward initializeAdnwSDK:pubId initializeState:initializeState duration:duration];
    }
    
}

-(void) setMediationTimeout:(int)timeout
{
    
    [VAMP setMediationTimeout:(float)timeout];
}

- (BOOL)isReady
{
    if (self.adReward) {
        return [self.adReward isReady];
    }
    return NO;
}

+ (NSString *)ADNWSDKVersion:(NSString *)adnwName
{
    NSString* version = @"nothing";
    if ([adnwName isEqualToString:@"VAMP"]) {
        version = [VAMP SDKVersion];
    }
    else if ([adnwName isEqualToString:@"UnityAds"]) {
        version = [UnityAds getVersion];
    }
    else if ([adnwName isEqualToString:@"AppLovin"]) {
        version = [ALSdk version];
    }
    else if ([adnwName isEqualToString:@"Maio"]) {
        version = [Maio sdkVersion];
    }
    else if ([adnwName isEqualToString:@"ADGPlayer"]) {
        version = [ADGPlayer sdkVersion];
    }
    else if ([adnwName isEqualToString:@"Vungle"]) {
        version = VungleSDKVersion;
    }
    else if ([adnwName isEqualToString:@"Admob"]) {
        version = [NSString stringWithCString:(const char *) GoogleMobileAdsVersionString
                                     encoding:NSUTF8StringEncoding];
    }
#ifdef FB_AD_SDK_VERSION
    else if ([adnwName isEqualToString:@"FAN"]) {
        version = FB_AD_SDK_VERSION;
    }
#endif
    
    return version;
}

+ (NSString *)SDKInfo:(NSString *)infoName
{
    NSString* info = @"nothing";
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
    
    return info;
}




#pragma mark ADGManagerViewControllerDelegate

// load完了して、広告表示できる状態になったことを通知します
-(void) vampDidReceive:(NSString *)placementId adnwName:(NSString *)adnwName
{
    if([self canUseGameObj]){
        NSString *str = [NSString stringWithFormat:@"vampDidReceive from iOS %@" , adnwName];
        UnitySendMessage([self.gameObjName UTF8String] , "VAMPDidReceive" , [str UTF8String]);
    }
    
}

// エラー
-(void) vampDidFail:(NSString *)placementId error:(VAMPError *)error
{
    NSString *codeString = [error kVAMPErrorString];
    
    if([self canUseGameObj]){
        NSString *str = [NSString stringWithFormat:@"vampDidFail(%@)(%@)" , placementId,codeString];
        UnitySendMessage([self.gameObjName UTF8String] , "VAMPDidFail" , [str UTF8String]);
    }
    
}

// インセンティブ付与可能になったタイミングで通知されます
-(void) vampDidComplete:(NSString *)placementId adnwName:(NSString *)adnwName
{
    if([self canUseGameObj]){
        NSString *str = [NSString stringWithFormat:@"vampDidComplete(%@)", adnwName];
        UnitySendMessage([self.gameObjName UTF8String] , "VAMPDidComplete" , [str UTF8String]);
    }
}

// 広告が閉じられた時に呼ばれます
-(void)vampDidClose:(NSString *)placementId adnwName:(NSString *)adnwName
{
    if([self canUseGameObj]){
        NSString *str = [NSString stringWithFormat:@"vampDidClose(%@)", adnwName];
        UnitySendMessage([self.gameObjName UTF8String] , "VAMPDidClose" , [str UTF8String]);
    }
}

// アドネットワークごとの広告取得が開始されたときに通知されます
-(void)vampLoadStart:(NSString *)placementId adnwName:(NSString *)adnwName
{
    if([self canUseGameObj]){
        NSString *str = [NSString stringWithFormat:@"vampLoadStart(%@)", adnwName];
        UnitySendMessage([self.gameObjName UTF8String] , "VAMPLoadStart" , [str UTF8String]);
    }
    
}

// アドネットワークごとの広告取得結果を通知する。（success,failedどちらも通知）
// この通知をもとにshowしないようご注意ください。showする判定は、onReceiveを受け取ったタイミングで判断ください。
-(void)vampLoadResult:(NSString *)placementId success:(BOOL)success adnwName:(NSString *)adnwName message:(NSString *)message
{
    if([self canUseGameObj]){
        NSString *str = [NSString stringWithFormat:@"vampLoadResult(%@ success:%@)", adnwName, success?@"YES":@"NO"];
        UnitySendMessage([self.gameObjName UTF8String] , "VAMPLoadResult" , [str UTF8String]);
    }
    
}

// 広告準備完了から55分経つと取得した広告が表示はできてもRTBの収益は発生しません
// この通知を受け取ったら、もう一度loadからやり直す必要があります。
-(void)vampDidExpired:(NSString *)placementId
{
    if([self canUseGameObj]){
        NSString *str = [NSString stringWithFormat:@"vampDidExpired(%@)\n", placementId];
        UnitySendMessage([self.gameObjName UTF8String] , "VAMPDidExpired" , [str UTF8String]);
    }
}

@end

#pragma mark - C++ code

#pragma mark definition for NativeInterface

extern "C"{
    void *_initVAMP(void *vampni , const char* pubId , bool enableTest, bool enableDebug, const char* objName);
    void _loadVAMP(void *vampni);
    bool _showVAMP(void *vampni);
    void _setTestModeVAMP(bool enableTest);
    bool _isTestModeVAMP();
    void _setDebugModeVAMP(bool enableTest);
    bool _isDebugModeVAMP();
    float _supportedOSVersionVAMP();
    bool _isSupportedOSVersionVAMP();
    void  _initializeAdnwSDK(void *vampni,const char* pubId);
    void _initializeAdnwSDKState(void *vampni,const char* pubId, const char* state, int duration);
    void _setMediationTimeoutVAMP(void *vampni, int timeout);
    char* _SDKVersionVAMP();
    bool _isReadyVAMP(void *vampni);
    char* _ADNWSDKVersionVAMP (const char* adnwName);
    char* _SDKInfoVAMP (const char* infoName);
    
}

#pragma mark method for NativeInterface

void *_initVAMP(void *vampni , const char* pubId , bool enableTest, bool enableDebug, const char* objName)
{
    NSString *adidStr = [NSString stringWithCString:pubId encoding:NSUTF8StringEncoding];
    
    VAMPNI *vampni_temp;
    NSString *objNameStr = [NSString stringWithCString:objName encoding:NSUTF8StringEncoding];
    
    if(vampni == NULL){
        vampni_temp = [[VAMPNI alloc] init];
    }
    else{
        vampni_temp = (__bridge VAMPNI *)vampni;
    }
    
    [vampni_temp setParams:UnityGetGLViewController() pubId:adidStr enableTestMode:enableTest enableDebugMode:enableDebug objName:objNameStr];
    
    [vampni_temp addIntance];
    
    
    return (__bridge void *)vampni_temp;
}

void _loadVAMP(void *vampni){
    VAMPNI *vampni_temp = (__bridge VAMPNI *)vampni;
    [vampni_temp loadRequest];
}

bool _showVAMP(void *vampni){
    VAMPNI *vampni_temp = (__bridge VAMPNI *)vampni;
    return [vampni_temp show];
}

void _setTestModeVAMP(bool enableTest){
    [VAMPNI setTestMode:enableTest];
}

bool _isTestModeVAMP()
{
    return [VAMPNI isTestMode];
}


void _setDebugModeVAMP(bool enableTest){
    [VAMPNI setDebugMode:enableTest];
}

bool _isDebugModeVAMP()
{
    return [VAMPNI isDebugMode];
}

float _supportedOSVersionVAMP()
{
    return [VAMPNI supportedOSVersion];
    
}

bool _isSupportedOSVersionVAMP()
{
    return [VAMPNI isSupportedOSVersion];
}

char* _SDKVersionVAMP()
{
    NSString *version = [VAMPNI SDKVersion];
    if ( version == nil ) {
        version = @"";
    }
    
    char *charVersion = (char *)[version UTF8String];
    char* res = (char*)malloc(strlen(charVersion) + 1);
    strcpy(res, charVersion);
    
    return res;
    
}

void _initializeAdnwSDK(void *vampni,const char* pubId)
{
    VAMPNI *vampni_temp = (__bridge VAMPNI *)vampni;
    NSString *adidStr = [NSString stringWithCString:pubId encoding:NSUTF8StringEncoding];
    if(vampni == NULL){
        vampni_temp = [[VAMPNI alloc] init];
        [vampni_temp setParams:UnityGetGLViewController() pubId:adidStr enableTestMode:[VAMPNI isTestMode] enableDebugMode:[VAMPNI isDebugMode] objName:@""];
    }
    [vampni_temp initializeAdnwSDK:adidStr];
}

void _initializeAdnwSDKState(void *vampni,const char* pubId, const char* state, int duration)
{
    VAMPNI *vampni_temp = (__bridge VAMPNI *)vampni;
    NSString *adidStr = [NSString stringWithCString:pubId encoding:NSUTF8StringEncoding];
    NSString *stateStr = [NSString stringWithCString:state encoding:NSUTF8StringEncoding];
    if(vampni == NULL){
        vampni_temp = [[VAMPNI alloc] init];
        [vampni_temp setParams:UnityGetGLViewController() pubId:adidStr enableTestMode:[VAMPNI isTestMode] enableDebugMode:[VAMPNI isDebugMode] objName:@""];
    }
    
    [vampni_temp initializeAdnwSDK:adidStr state:stateStr duration:duration];
}

void _setMediationTimeoutVAMP(void *vampni, int timeout){
    VAMPNI *vampni_temp = (__bridge VAMPNI *)vampni;
    [vampni_temp setMediationTimeout:timeout];
}


bool _isReadyVAMP(void *vampni)
{
    VAMPNI *vampni_temp = (__bridge VAMPNI *)vampni;
    return [vampni_temp isReady];
}


char* _ADNWSDKVersionVAMP (const char* adnwName) {
    NSString *adnwNameStr = [NSString stringWithCString:adnwName encoding:NSUTF8StringEncoding];
    
    NSString *version = [VAMPNI ADNWSDKVersion:adnwNameStr];
    
    if ( version == nil ) {
        version = @"";
    }
    
    char *charVersion = (char *)[version UTF8String];
    char* res = (char*)malloc(strlen(charVersion) + 1);
    strcpy(res, charVersion);
    
    return res;
    
}

char* _SDKInfoVAMP (const char* infoName) {
    NSString *infoNameStr = [NSString stringWithCString:infoName encoding:NSUTF8StringEncoding];
    
    NSString *info = [VAMPNI SDKInfo:infoNameStr];
    
    if ( info == nil ) {
        info = @"";
    }
    
    char *charInfo = (char *)[info UTF8String];
    char* res = (char*)malloc(strlen(charInfo) + 1);
    strcpy(res, charInfo);
    
    return res;
    
}


