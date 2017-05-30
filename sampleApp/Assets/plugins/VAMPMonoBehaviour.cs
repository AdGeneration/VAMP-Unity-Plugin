using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

public class VAMPMonoBehaviour : MonoBehaviour {
	#if UNITY_IPHONE
	[DllImport ("__Internal")]
		protected static extern IntPtr _initVAMP (IntPtr vampni , string pubid, bool enableTest, bool enableDebug , string objName );
	[DllImport ("__Internal")]
		protected static extern void _loadVAMP (IntPtr vampni);
	[DllImport ("__Internal")]
		protected static extern void _showVAMP (IntPtr vampni);
	[DllImport ("__Internal")]
	protected static extern void _setTestModeVAMP(IntPtr vampni, bool enableTest);
	[DllImport ("__Internal")]
	protected static extern bool _isTestModeVAMP(IntPtr vampni);
	[DllImport ("__Internal")]
	protected static extern void _setDebugModeVAMP(IntPtr vampni, bool enableTest);
	[DllImport ("__Internal")]
	protected static extern bool _isDebugModeVAMP(IntPtr vampni);
	[DllImport ("__Internal")]
	protected static extern float _supportedOSVersionVAMP();
	[DllImport ("__Internal")]
	protected static extern void  _initializeAdnwSDK(IntPtr vampni,string pubId);
	[DllImport ("__Internal")]
	protected static extern void _initializeAdnwSDKState(IntPtr vampni,string pubId, string state, int duration);
	[DllImport ("__Internal")]
	protected static extern void _setMediationTimeoutVAMP(IntPtr vampni, int timeout);

	[DllImport ("__Internal")]
	protected static extern string _SDKVersionVAMP (IntPtr vampni);
	[DllImport ("__Internal")]
	protected static extern bool _isReadyVAMP(IntPtr vampni);

	[DllImport ("__Internal")]
	protected static extern string _ADNWSDKVersionVAMP (string adnw);

	[DllImport ("__Internal")]
	protected static extern string _SDKInfoVAMP(string infoname);

	#endif
}