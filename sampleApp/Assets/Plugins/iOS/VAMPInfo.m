//
//  VAMPInfo.m
//  VAMPInfoUtil
//
//  Created by AdGeneration on 2022/09/07.
//  Copyright © 2022 Supership Inc. All rights reserved.
//

#import <AdSupport/AdSupport.h>
#import <CoreTelephony/CTCarrier.h>
#import <CoreTelephony/CTTelephonyNetworkInfo.h>
#import <UIKit/UIKit.h>

#import <VAMP/VAMP.h>

#import "VAMPInfo.h"

NSString *const VAMPInfoDeviceInfoKeyDeviceName = @"DeviceName";
NSString *const VAMPInfoDeviceInfoKeyOSName = @"OSName";
NSString *const VAMPInfoDeviceInfoKeyOSVersion = @"OSVer";
NSString *const VAMPInfoDeviceInfoKeyModel = @"Model";
NSString *const VAMPInfoDeviceInfoKeyCarrier = @"Carrier";
NSString *const VAMPInfoDeviceInfoKeyISOCountryCode = @"ISOCountryCode";
NSString *const VAMPInfoDeviceInfoKeyCountryCode = @"CountryCode";
NSString *const VAMPInfoDeviceInfoKeyLocaleCode = @"LocaleCode";
NSString *const VAMPInfoDeviceInfoKeyIDFA = @"IDFA";
NSString *const VAMPInfoDeviceInfoKeyIDFV = @"IDFV";
NSString *const VAMPInfoDeviceInfoKeyBundleID = @"BundleID";
NSString *const VAMPInfoDeviceInfoKeyAppVersion = @"AppVer";
NSString *const VAMPInfoDeviceInfoKeyAdMobAppID = @"AdMobAppID";

typedef NSString *(*AdNetworkVersionMethod)(id, SEL);
typedef NSString *(*AdapterVersionMethod)(id, SEL);

NSString * VAMPInfoGetAdNetworkVersionWithAdapterClass(NSString *adapterClass) {
    Class cls = NSClassFromString(adapterClass);

    if (!cls) {
        return nil;
    }
    SEL sel = NSSelectorFromString(@"adNetworkVersion");
    IMP method = [cls instanceMethodForSelector:sel];
    AdNetworkVersionMethod func = (void *) method;
    id obj = [cls new];
    return func(obj, sel);
}

NSString * VAMPInfoGetAdapterVersionWithAdapterClass(NSString *adapterClass) {
    Class cls = NSClassFromString(adapterClass);

    if (!cls) {
        return nil;
    }
    SEL sel = NSSelectorFromString(@"adapterVersion");
    IMP method = [cls instanceMethodForSelector:sel];
    AdapterVersionMethod func = (void *) method;
    id obj = [cls new];
    NSString *ver = func(obj, sel);

    // アダプタバージョンは以下の形式のため、
    // @(#)PROGRAM:VAMPAdMobAdapter  PROJECT:VAMPAdMobAdapter-9.3.0.0\n
    // 最後のバージョン番号だけを抽出する
    NSArray<NSString *> *comps = [ver componentsSeparatedByString:@"-"];
    return [comps.lastObject
            stringByTrimmingCharactersInSet:NSCharacterSet.whitespaceAndNewlineCharacterSet] ?: @"";
}

@implementation VAMPInfo

static NSString *const VAMPInfoVersion = @"0.1.1";

static NSString *const VAMPInfoAdMobAdapter = @"VAMPAdMobSDKAdapter";
static NSString *const VAMPInfoIronSourceAdapter = @"VAMPIronSourceSDKAdapter";
static NSString *const VAMPInfoLINEAdsAdapter = @"VAMPLINEAdsSDKAdapter";
static NSString *const VAMPInfoMaioAdapter = @"VAMPMaioSDKAdapter";
static NSString *const VAMPInfoPangleAdapter = @"VAMPPangleSDKAdapter";
static NSString *const VAMPInfoUnityAdsAdapter = @"VAMPUnityAdsSDKAdapter";

+ (NSString *) versionString {
    return VAMPInfoVersion;
}

+ (nullable NSString *) adNetworkVersionOfAdNetworkName:(NSString *)adNetworkName {
    NSString *name = adNetworkName.lowercaseString;

    if ([name isEqualToString:@"vamp"]) {
        return VAMPSDKVersion;
    }
    else if ([name isEqualToString:@"admob"]) {
        return VAMPInfoGetAdNetworkVersionWithAdapterClass(VAMPInfoAdMobAdapter);
    }
    else if ([name isEqualToString:@"ironsource"] || [name isEqualToString:@"is"]) {
        return VAMPInfoGetAdNetworkVersionWithAdapterClass(VAMPInfoIronSourceAdapter);
    }
    else if ([name isEqualToString:@"lineads"]) {
        return VAMPInfoGetAdNetworkVersionWithAdapterClass(VAMPInfoLINEAdsAdapter);
    }
    else if ([name isEqualToString:@"maio"]) {
        return VAMPInfoGetAdNetworkVersionWithAdapterClass(VAMPInfoMaioAdapter);
    }
    else if ([name isEqualToString:@"pangle"]) {
        return VAMPInfoGetAdNetworkVersionWithAdapterClass(VAMPInfoPangleAdapter);
    }
    else if ([name isEqualToString:@"unityads"]) {
        return VAMPInfoGetAdNetworkVersionWithAdapterClass(VAMPInfoUnityAdsAdapter);
    }
    else {
        return nil;
    }
}

+ (nullable NSString *) adapterVersionOfAdNetworkName:(NSString *)adNetworkName {
    NSString *name = adNetworkName.lowercaseString;

    if ([name isEqualToString:@"vamp"]) {
        return VAMPSDKVersion;
    }
    else if ([name isEqualToString:@"admob"]) {
        return VAMPInfoGetAdapterVersionWithAdapterClass(VAMPInfoAdMobAdapter);
    }
    else if ([name isEqualToString:@"ironsource"] || [name isEqualToString:@"is"]) {
        return VAMPInfoGetAdapterVersionWithAdapterClass(VAMPInfoIronSourceAdapter);
    }
    else if ([name isEqualToString:@"lineads"]) {
        return VAMPInfoGetAdapterVersionWithAdapterClass(VAMPInfoLINEAdsAdapter);
    }
    else if ([name isEqualToString:@"maio"]) {
        return VAMPInfoGetAdapterVersionWithAdapterClass(VAMPInfoMaioAdapter);
    }
    else if ([name isEqualToString:@"pangle"]) {
        return VAMPInfoGetAdapterVersionWithAdapterClass(VAMPInfoPangleAdapter);
    }
    else if ([name isEqualToString:@"unityads"]) {
        return VAMPInfoGetAdapterVersionWithAdapterClass(VAMPInfoUnityAdsAdapter);
    }
    else {
        return nil;
    }
}

+ (nullable NSString *) deviceInfoForKey:(NSString *)key {
    if ([key isEqualToString:VAMPInfoDeviceInfoKeyDeviceName]) {
        return [UIDevice currentDevice].name;
    }
    else if ([key isEqualToString:VAMPInfoDeviceInfoKeyOSName]) {
        return [UIDevice currentDevice].systemName;
    }
    else if ([key isEqualToString:VAMPInfoDeviceInfoKeyOSVersion]) {
        return [UIDevice currentDevice].systemVersion;
    }
    else if ([key isEqualToString:VAMPInfoDeviceInfoKeyModel]) {
        return [UIDevice currentDevice].model;
    }
    else if ([key isEqualToString:VAMPInfoDeviceInfoKeyCarrier]) {
        CTTelephonyNetworkInfo *networkInfo = [CTTelephonyNetworkInfo new];
        CTCarrier *provider = [networkInfo subscriberCellularProvider];
        return provider.carrierName;
    }
    else if ([key isEqualToString:VAMPInfoDeviceInfoKeyISOCountryCode]) {
        CTTelephonyNetworkInfo *networkInfo = [CTTelephonyNetworkInfo new];
        CTCarrier *provider = [networkInfo subscriberCellularProvider];
        return provider.isoCountryCode;
    }
    else if ([key isEqualToString:VAMPInfoDeviceInfoKeyCountryCode]) {
        return [NSLocale preferredLanguages].firstObject;
    }
    else if ([key isEqualToString:VAMPInfoDeviceInfoKeyLocaleCode]) {
        return [[NSLocale currentLocale] objectForKey:NSLocaleIdentifier];
    }
    else if ([key isEqualToString:VAMPInfoDeviceInfoKeyIDFA]) {
        return [ASIdentifierManager sharedManager].advertisingIdentifier.UUIDString;
    }
    else if ([key isEqualToString:VAMPInfoDeviceInfoKeyIDFV]) {
        return [UIDevice currentDevice].identifierForVendor.UUIDString;
    }
    else if ([key isEqualToString:VAMPInfoDeviceInfoKeyBundleID]) {
        return [NSBundle mainBundle].bundleIdentifier;
    }
    else if ([key isEqualToString:VAMPInfoDeviceInfoKeyAppVersion]) {
        return [NSBundle mainBundle].infoDictionary[@"CFBundleShortVersionString"];
    }
    else if ([key isEqualToString:VAMPInfoDeviceInfoKeyAdMobAppID]) {
        return [[NSBundle mainBundle] objectForInfoDictionaryKey:@"GADApplicationIdentifier"];
    }
    else {
        return nil;
    }
}

@end
