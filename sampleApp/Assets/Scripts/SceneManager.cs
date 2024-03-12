using System.Collections;
using UnityEngine;

public class SceneManager : SingletonMonoBehaviour<SceneManager>
{
    public void LoadScene(Scene scene) {
        StartCoroutine(LoadSceneAsync(scene.ToString()));
    }

    public void LoadScene(string sceneName) {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    IEnumerator LoadSceneAsync(string sceneName) {
        var asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);

        yield return new WaitUntil(() => asyncLoad.isDone);
    }
}

public enum Scene {
    Main,
    AdSample,
    Info
}
