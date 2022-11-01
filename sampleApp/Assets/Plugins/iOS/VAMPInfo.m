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

typedef NSString *(*AdNetworkVersionMethod)(id, SEL);
typedef NSString *(*AdapterVersionMethod)(id, SEL);

NSString *VAMPInfoGetAdNetworkVersionWithAdapterClass(NSString *adapterClass) {
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

NSString *VAMPInfoGetAdapterVersionWithAdapterClass(NSString *adapterClass) {
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

static NSString *const kAdMobAdapter = @"VAMPAdMobSDKAdapter";
static NSString *const kFANAdapter = @"VAMPFANSDKAdapter";
static NSString *const kLINEAdsAdapter = @"VAMPLINEAdsSDKAdapter";
static NSString *const kMaioAdapter = @"VAMPMaioSDKAdapter";
static NSString *const kNendAdapter = @"VAMPNendSDKAdapter";
static NSString *const kPangleAdapter = @"VAMPPangleSDKAdapter";
static NSString *const kTapjoyAdapter = @"VAMPTapjoySDKAdapter";
static NSString *const kUnityAdsAdapter = @"VAMPUnityAdsSDKAdapter";

+ (nullable NSString *)adNetworkVersionOfAdNetworkName:(NSString *)name {
    NSString *adnwName = name.lowercaseString;

    if ([adnwName isEqualToString:@"vamp"]) {
        return VAMPSDKVersion;
    }
    else if ([adnwName isEqualToString:@"admob"]) {
        return VAMPInfoGetAdNetworkVersionWithAdapterClass(kAdMobAdapter);
    }
    else if ([adnwName isEqualToString:@"fan"] ||
        [adnwName isEqualToString:@"meta"]) {
        return VAMPInfoGetAdNetworkVersionWithAdapterClass(kFANAdapter);
    }
    else if ([adnwName isEqualToString:@"lineads"]) {
        return VAMPInfoGetAdNetworkVersionWithAdapterClass(kLINEAdsAdapter);
    }
    else if ([adnwName isEqualToString:@"maio"]) {
        return VAMPInfoGetAdNetworkVersionWithAdapterClass(kMaioAdapter);
    }
    else if ([adnwName isEqualToString:@"nend"]) {
        return VAMPInfoGetAdNetworkVersionWithAdapterClass(kNendAdapter);
    }
    else if ([adnwName isEqualToString:@"pangle"]) {
        return VAMPInfoGetAdNetworkVersionWithAdapterClass(kPangleAdapter);
    }
    else if ([adnwName isEqualToString:@"tapjoy"]) {
        return VAMPInfoGetAdNetworkVersionWithAdapterClass(kTapjoyAdapter);
    }
    else if ([adnwName isEqualToString:@"unityads"]) {
        return VAMPInfoGetAdNetworkVersionWithAdapterClass(kUnityAdsAdapter);
    }
    else {
        return nil;
    }
}

+ (nullable NSString *)adapterVersionOfAdNetworkName:(NSString *)name {
    NSString *adnwName = name.lowercaseString;

    if ([adnwName isEqualToString:@"vamp"]) {
        return VAMPSDKVersion;
    }
    else if ([adnwName isEqualToString:@"admob"]) {
        return VAMPInfoGetAdapterVersionWithAdapterClass(kAdMobAdapter);
    }
    else if ([adnwName isEqualToString:@"fan"] ||
        [adnwName isEqualToString:@"meta"]) {
        return VAMPInfoGetAdapterVersionWithAdapterClass(kFANAdapter);
    }
    else if ([adnwName isEqualToString:@"lineads"]) {
        return VAMPInfoGetAdapterVersionWithAdapterClass(kLINEAdsAdapter);
    }
    else if ([adnwName isEqualToString:@"maio"]) {
        return VAMPInfoGetAdapterVersionWithAdapterClass(kMaioAdapter);
    }
    else if ([adnwName isEqualToString:@"nend"]) {
        return VAMPInfoGetAdapterVersionWithAdapterClass(kNendAdapter);
    }
    else if ([adnwName isEqualToString:@"pangle"]) {
        return VAMPInfoGetAdapterVersionWithAdapterClass(kPangleAdapter);
    }
    else if ([adnwName isEqualToString:@"tapjoy"]) {
        return VAMPInfoGetAdapterVersionWithAdapterClass(kTapjoyAdapter);
    }
    else if ([adnwName isEqualToString:@"unityads"]) {
        return VAMPInfoGetAdapterVersionWithAdapterClass(kUnityAdsAdapter);
    }
    else {
        return nil;
    }
}

+ (nullable NSString *)deviceInfoForKey:(NSString *)key {
    if ([key isEqualToString:@"DeviceName"]) {
        return [[UIDevice currentDevice] name];
    }
    else if ([key isEqualToString:@"OSName"]) {
        return [[UIDevice currentDevice] systemName];
    }
    else if ([key isEqualToString:@"OSVersion"]) {
        return [[UIDevice currentDevice] systemVersion];
    }
    else if ([key isEqualToString:@"OSModel"]) {
        return [[UIDevice currentDevice] model];
    }
    else if ([key isEqualToString:@"Carrier"]) {
        CTTelephonyNetworkInfo *networkInfo = [CTTelephonyNetworkInfo new];
        CTCarrier *provider = [networkInfo subscriberCellularProvider];
        return provider.carrierName;
    }
    else if ([key isEqualToString:@"ISOCountry"]) {
        CTTelephonyNetworkInfo *networkInfo = [CTTelephonyNetworkInfo new];
        CTCarrier *provider = [networkInfo subscriberCellularProvider];
        return provider.isoCountryCode;
    }
    else if ([key isEqualToString:@"CountryCode"]) {
        return [NSLocale preferredLanguages].firstObject;
    }
    else if ([key isEqualToString:@"LocaleCode"]) {
        return [[NSLocale currentLocale] objectForKey:NSLocaleIdentifier];
    }
    else if ([key isEqualToString:@"IDFA"]) {
        return [ASIdentifierManager sharedManager].advertisingIdentifier.UUIDString;
    }
    else if ([key isEqualToString:@"BundleID"]) {
        return [NSBundle mainBundle].bundleIdentifier;
    }
    else if ([key isEqualToString:@"AppVer"]) {
        return [NSBundle mainBundle].infoDictionary[@"CFBundleShortVersionString"];
    }
    else {
        return nil;
    }
}

@end
