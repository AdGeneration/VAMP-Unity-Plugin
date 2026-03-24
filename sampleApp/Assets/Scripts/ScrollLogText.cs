using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class ScrollLogText : Text
{
    [SerializeField]
    private Font fallbackFont;

    protected override void Awake() {
        base.Awake();
        if (font == null) {
            font = fallbackFont ??
                   Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf") ??
                   Resources.GetBuiltinResource<Font>("Arial.ttf");
        }
        fontSize = 48;
        color = new Color(0.2f, 0.2f, 0.2f, 1f);
        alignment = TextAnchor.UpperLeft;
        horizontalOverflow = HorizontalWrapMode.Wrap;
        verticalOverflow = VerticalWrapMode.Overflow;
        supportRichText = true;
        text = "--- Log ready ---";
    }

    public void AddLine(string line) {
        text = $"{line}\n{text}";
    }
}
