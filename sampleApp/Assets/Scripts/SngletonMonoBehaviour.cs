using UnityEngine;

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    protected virtual bool dontDestroyOnLoad {
        get { return true; }
    }

    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null) {
                instance = FindObjectOfType<T>();
                if (instance == null) {
                    instance = new GameObject(typeof(T).ToString()).AddComponent<T>();
                }
            }

            return instance;
        }
    }

    protected virtual void Awake() {
        if (this != Instance) {
            DestroyImmediate(this);
            return;
        }

        if (dontDestroyOnLoad) {
            DontDestroyOnLoad(this.gameObject);
        }
    }
}
