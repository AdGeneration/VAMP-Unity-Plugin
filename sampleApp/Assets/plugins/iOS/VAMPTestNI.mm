#import <CoreTelephony/CTCarrier.h>
#import <CoreTelephony/CTTelephonyNetworkInfo.h>

#import <VAMP/VAMP.h>

#import <AppLovinSDK/AppLovinSDK.h>
#import <FBAudienceNetwork/FBAudienceNetwork.h>
#import <GoogleMobileAds/GoogleMobileAds.h>
#import <Maio/Maio.h>
#import <MTGSDK/MTGSDK.h>
#import <NendAd/NendAd.h>
#import <Tapjoy/Tapjoy.h>
#import <UnityAds/UnityAds.h>
#import <VungleSDK/VungleSDK.h>

// MoPubを使う場合は以下のコメントアウトを外してください
//#import <MoPubSDKFramework/MoPub.h>

NSString *VAMPTestNIGetAdnwSDKVersion(NSString *adnwName) {
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
#ifdef MTGSDKVersion
    else if ([adnwName isEqualToString:@"Mintegral"]) {
        version = MTGSDKVersion;
    }
#endif
#ifdef MP_SDK_VERSION
    else if ([adnwName isEqualToString:@"MoPub"]) {
        version = MP_SDK_VERSION;
    }
#endif
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
NSString *VAMPTestNIGetDeviceInfo(NSString *infoName) {
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

extern "C" {
    char *VAMPUnityTestAdnwSDKVersion(const char *cAdnwName) {
        NSString *adnwName = [NSString stringWithCString:cAdnwName encoding:NSUTF8StringEncoding];
        
        NSString *version = VAMPTestNIGetAdnwSDKVersion(adnwName);
        
        if (!version) {
            version = @"";
        }
        
        char *cVersion = (char *) version.UTF8String;
        char *res = (char *) malloc(strlen(cVersion) + 1);
        strcpy(res, cVersion);
        
        return res;
    }
    
    char *VAMPUnityTestDeviceInfo(const char *cInfoName) {
        NSString *infoName = [NSString stringWithCString:cInfoName encoding:NSUTF8StringEncoding];
        
        NSString *info = VAMPTestNIGetDeviceInfo(infoName);
        
        if (!info) {
            info = @"";
        }
        
        char *cInfo = (char *) info.UTF8String;
        char *res = (char *) malloc(strlen(cInfo) + 1);
        strcpy(res, cInfo);
        
        return res;
    }
}
