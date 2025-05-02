
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Arranges the start‑screen UI as:
///       Start   (big warm‑font title, centred)
///       ───────────────────────────────
///       [ Multiplayer ]  (large rounded button)
///       [ Singleplayer ] (large rounded button)
///
///  Store & Options = small rounded buttons anchored bottom‑left
///
/// Assign all references in the Inspector, along with:
///   • roundedSprite → a 9‑sliced rounded rectangle sprite
///   • funFont       → a TMP Font Asset (Comic Neue, Nunito, etc.)
/// </summary>
[ExecuteAlways]
public class MainMenuLayout : MonoBehaviour
{
    [Header("Central elements (required)")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Button multiplayerButton;
    [SerializeField] private Button singleplayerButton;

    [Header("Corner elements (required)")]
    [SerializeField] private Button storeButton;
    [SerializeField] private Button optionsButton;

    [Header("Styling assets (required)")]
    [SerializeField] private Sprite roundedSprite;     // 9‑slice rounded rect
    [SerializeField] private TMP_FontAsset funFont;

    [Header("Colours")]
    [SerializeField] private Color titleColor = Color.white;
    [SerializeField] private Color buttonBg   = new Color32(0xC4,0x00,0x00,0xFF);
    [SerializeField] private Color buttonTxt  = Color.white;
    [SerializeField] private Color highlight  = new Color32(0xFF,0x33,0x33,0xFF);

    private const float bigW = 500f,  bigH = 100f;
    private const float smW  = 240f,  smH  = 70f;

    private void Start()          { Apply(); }
    private void OnValidate()     { Apply(); }

    private void Apply()
    {
        if (!AllSet()) return;

        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;

        // ----- Central stack (title + 2 big buttons) -----
        RectTransform stack = GetOrCreate("CenterStack", canvas.transform,
                                          new Vector2(0.5f,0.5f), Vector2.zero);
        VerticalLayoutGroup vlg = stack.GetComponent<VerticalLayoutGroup>() ??
                                  stack.gameObject.AddComponent<VerticalLayoutGroup>();
        vlg.childAlignment = TextAnchor.MiddleCenter;
        vlg.spacing = 20f;
        vlg.childForceExpandHeight = vlg.childForceExpandWidth = false;

        StyleTitle(titleText, stack);
        StyleBig(multiplayerButton,   stack);
        StyleBig(singleplayerButton,  stack);

        // ----- Corner buttons parent -----
        RectTransform corner = GetOrCreate("CornerButtons", canvas.transform,
                                           new Vector2(0,0), new Vector2(25,25));
        HorizontalLayoutGroup hlg = corner.GetComponent<HorizontalLayoutGroup>() ??
                                    corner.gameObject.AddComponent<HorizontalLayoutGroup>();
        hlg.childAlignment = TextAnchor.LowerLeft;
        hlg.spacing = 12f;
        hlg.childForceExpandWidth = hlg.childForceExpandHeight = false;

        StyleSmall(storeButton,   corner);
        StyleSmall(optionsButton, corner);
    }

    private bool AllSet()
    {
        return titleText && multiplayerButton && singleplayerButton &&
               storeButton && optionsButton && roundedSprite && funFont;
    }

    // ---------- styling helpers ----------
    private void StyleTitle(TextMeshProUGUI tmp, Transform parent)
    {
        tmp.rectTransform.SetParent(parent, false);
        tmp.text = "Start";
        tmp.font = funFont;
        tmp.fontSize = 90;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = titleColor;
    }

    private void StyleBig(Button btn, Transform parent)
    {
        SetupButton(btn, parent, bigW, bigH, 38);
    }
    private void StyleSmall(Button btn, Transform parent)
    {
        SetupButton(btn, parent, smW, smH, 30);
    }

    private void SetupButton(Button btn, Transform parent, float w, float h, int fontSize)
    {
        RectTransform rt = btn.transform as RectTransform;
        rt.SetParent(parent, false);
        rt.sizeDelta = new Vector2(w, h);

        Image img = btn.GetComponent<Image>();
        img.sprite = roundedSprite;
        img.type = Image.Type.Sliced;
        img.color = buttonBg;

        TextMeshProUGUI tmp = btn.GetComponentInChildren<TextMeshProUGUI>();
        tmp.font = funFont;
        tmp.fontSize = fontSize;
        tmp.color = buttonTxt;

        ColorBlock cb = btn.colors;
        cb.normalColor      = buttonBg;
        cb.highlightedColor = highlight;
        cb.pressedColor     = highlight * 0.9f;
        cb.selectedColor    = highlight;
        cb.disabledColor    = new Color(0.5f,0.5f,0.5f,0.35f);
        btn.colors = cb;
    }

    // ---------- util ----------
    private RectTransform GetOrCreate(string name, Transform parent, Vector2 anchorMinMax, Vector2 offset)
    {
        Transform t = parent.Find(name);
        RectTransform rt = t? t as RectTransform : new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();

        rt.SetParent(parent, false);
        rt.anchorMin = rt.anchorMax = anchorMinMax;
        rt.pivot = anchorMinMax;
        rt.anchoredPosition = offset;
        return rt;
    }
}
