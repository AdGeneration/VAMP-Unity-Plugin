using UnityEngine;

/// <summary>
/// Canvas 直下に配置することで Screen.safeArea に合わせて
/// RectTransform を自動調整する補助コンポーネント。
/// AppOpenAdSample.unity の UI を notch / Dynamic Island から
/// 保護するために追加 (PR #57)。
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class SafeAreaPanel : MonoBehaviour
{
    private RectTransform rectTransform;
    private Rect lastSafeArea = Rect.zero;

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable() {
        ApplySafeArea();
    }

    // 自身の anchor 変更で再発火しうるが、lastSafeArea キャッシュで冗長な ApplySafeArea を抑制する。
    private void OnRectTransformDimensionsChange() {
        ApplySafeArea();
    }

    private void ApplySafeArea() {
        if (rectTransform == null) {
            return;
        }
        var safeArea = Screen.safeArea;
        if (safeArea == lastSafeArea) {
            return;
        }
        lastSafeArea = safeArea;
        var screenSize = new Vector2(Screen.width, Screen.height);
        if (screenSize.x <= 0f || screenSize.y <= 0f) {
            return;
        }
        var anchorMin = safeArea.position / screenSize;
        var anchorMax = (safeArea.position + safeArea.size) / screenSize;
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }
}
