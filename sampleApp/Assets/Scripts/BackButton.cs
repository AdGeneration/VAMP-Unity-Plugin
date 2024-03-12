using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BackButton : MonoBehaviour
{
    private void Start() {
        GetComponent<Button>().onClick.AddListener(OnBack);
        gameObject.SetActive(BackEventManager.Instance.CanBack);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            OnBack();
        }
    }

    void OnBack() {
        BackEventManager.Instance.OnBack();
    }
}

public class BackEventManager
{
    private static Queue<UnityAction> onBackQueue = new Queue<UnityAction>();

    private static BackEventManager instance;
    public static BackEventManager Instance
    {
        get
        {
            if (instance == null) {
                instance = new BackEventManager();
            }
            return instance;
        }
    }

    public bool CanBack
    {
        get
        {
            return onBackQueue.Any();
        }
    }

    private BackEventManager() {

    }

    public void RegisterBackEvent(UnityAction onBack) {
        if (onBack != null) {
            onBackQueue.Enqueue(onBack);
        }
    }

    public void OnBack() {
        if (CanBack) {
            var onBack = onBackQueue.Dequeue();
            if (onBack != null) {
                onBack.Invoke();
            }
        }
    }
}