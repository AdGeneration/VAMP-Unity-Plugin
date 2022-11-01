//
//  VAMPInfo.h
//  VAMPInfoUtil
//
//  Created by AdGeneration on 2022/09/07.
//  Copyright © 2022 Supership Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface VAMPInfo : NSObject

+ (nullable NSString *)adNetworkVersionOfAdNetworkName:(NSString *)name;
+ (nullable NSString *)adapterVersionOfAdNetworkName:(NSString *)name;
+ (nullable NSString *)deviceInfoForKey:(NSString *)key;

@end

NS_ASSUME_NONNULL_END
