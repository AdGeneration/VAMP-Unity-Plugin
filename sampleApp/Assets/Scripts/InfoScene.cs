using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoScene : MonoBehaviour
{
    [SerializeField]
    private Text text;

    private List<string> infoList;

    void Start() {
        infoList = SDKTestUtil.GetDeviceInfo();
        text.text = string.Join("\n", infoList);
    }
}

