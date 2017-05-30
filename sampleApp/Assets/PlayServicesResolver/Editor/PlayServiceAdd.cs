using Google.JarResolver;
using UnityEditor;

[InitializeOnLoad]
public static class PlayServiceAdd {

	static PlayServiceAdd() {

		PlayServicesSupport svcSupport = PlayServicesSupport.CreateInstance(
			"AdsSample", EditorPrefs.GetString("AndroidSdkRoot"), "ProjectSettings");

		svcSupport.DependOn("com.google.android.gms", "play-services-ads", "8+");
	}
}
