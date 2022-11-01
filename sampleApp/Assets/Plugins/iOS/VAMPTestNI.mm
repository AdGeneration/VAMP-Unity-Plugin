#import <VAMP/VAMP.h>
#import "VAMPInfo.h"

extern "C" {
char *VAMPUnityTestAdnwSDKVersion(const char *cAdnwName) {
    NSString *adnwName = [NSString stringWithCString:cAdnwName encoding:NSUTF8StringEncoding];

    NSString *version = [VAMPInfo adNetworkVersionOfAdNetworkName:adnwName];

    if (!version) {
        version = @"nothing";
    }

    char *cVersion = (char *) version.UTF8String;
    char *res = (char *) malloc(strlen(cVersion) + 1);
    strcpy(res, cVersion);

    return res;
}

char *VAMPUnityTestAdapterVersion(const char *cAdnwName) {
    NSString *adnwName = [NSString stringWithCString:cAdnwName encoding:NSUTF8StringEncoding];

    NSString *version = [VAMPInfo adapterVersionOfAdNetworkName:adnwName];

    if (!version) {
        version = @"nothing";
    }

    char *cVersion = (char *) version.UTF8String;
    char *res = (char *) malloc(strlen(cVersion) + 1);
    strcpy(res, cVersion);

    return res;
}

char *VAMPUnityTestDeviceInfo(const char *cInfoName) {
    NSString *infoName = [NSString stringWithCString:cInfoName encoding:NSUTF8StringEncoding];

    NSString *info = [VAMPInfo deviceInfoForKey:infoName];

    if (!info) {
        info = @"nothing";
    }

    char *cInfo = (char *) info.UTF8String;
    char *res = (char *) malloc(strlen(cInfo) + 1);
    strcpy(res, cInfo);

    return res;
}
}
