//
//  VAMPInfo.h
//  VAMPInfoUtil
//
//  Created by AdGeneration on 2022/09/07.
//  Copyright © 2022 Supership Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

FOUNDATION_EXTERN NSString *const VAMPInfoDeviceInfoKeyDeviceName;
FOUNDATION_EXTERN NSString *const VAMPInfoDeviceInfoKeyOSName;
FOUNDATION_EXTERN NSString *const VAMPInfoDeviceInfoKeyOSVersion;
FOUNDATION_EXTERN NSString *const VAMPInfoDeviceInfoKeyModel;
FOUNDATION_EXTERN NSString *const VAMPInfoDeviceInfoKeyCarrier;
FOUNDATION_EXTERN NSString *const VAMPInfoDeviceInfoKeyISOCountryCode;
FOUNDATION_EXTERN NSString *const VAMPInfoDeviceInfoKeyCountryCode;
FOUNDATION_EXTERN NSString *const VAMPInfoDeviceInfoKeyLocaleCode;
FOUNDATION_EXTERN NSString *const VAMPInfoDeviceInfoKeyIDFA;
FOUNDATION_EXTERN NSString *const VAMPInfoDeviceInfoKeyIDFV;
FOUNDATION_EXTERN NSString *const VAMPInfoDeviceInfoKeyBundleID;
FOUNDATION_EXTERN NSString *const VAMPInfoDeviceInfoKeyAppVersion;
FOUNDATION_EXTERN NSString *const VAMPInfoDeviceInfoKeyAdMobAppID;

@interface VAMPInfo : NSObject
/**
 * VAMPInfoフレームワークのバージョンを返します
 */
+ (NSString *) versionString;
/**
 * アドネットワークのバージョンを返します。
 * 対応するアダプタがインポートされていない場合はnilを返します
 */
+ (nullable NSString *) adNetworkVersionOfAdNetworkName:(NSString *)adNetworkName;
/**
 * アドネットワークアダプタのバージョンを返します。
 * 対応するアダプタがインポートされていない場合はnilを返します
 */
+ (nullable NSString *) adapterVersionOfAdNetworkName:(NSString *)adNetworkName;
/**
 * 指定したキーの端末情報およびアプリ情報を返します
 *
 * @param key VAMPInfoDeviceInfoKey*
 */
+ (nullable NSString *) deviceInfoForKey:(NSString *)key;

@end

NS_ASSUME_NONNULL_END
