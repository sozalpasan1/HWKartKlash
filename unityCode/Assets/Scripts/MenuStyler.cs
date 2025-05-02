using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Programmatically creates a centred vertical layout panel and
/// themes each button in red/white/black.  Run once at scene start.
/// </summary>
[ExecuteAlways]    // Also runs in Edit mode so you can see the result instantly
public class MenuStyler : MonoBehaviour
{
    [Header("Assign your 4 buttons here (in any order)")]
    [SerializeField] private Button[] menuButtons = new Button[4];

    // ---- colours ----
    private readonly Color panelBg   = Color.black;               // screen background
    private readonly Color buttonBg  = new Color32(0xC4,0x00,0x00,0xFF); // deep red
    private readonly Color buttonTxt = Color.white;
    private readonly Color highlight = new Color32(0xFF,0x33,0x33,0xFF); // lighter red

    private const float buttonHeight = 80f;
    private const float buttonWidth  = 420f;
    private const float spacing      = 18f;

    private void Start()
    {
        if (menuButtons == null || menuButtons.Length == 0) return;

        // 1)  Make/Find a full-screen panel under the Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("MenuStyler - No Canvas found in scene.");
            return;
        }

        RectTransform panel = GetOrCreatePanel(canvas.transform);
        panel.GetComponent<Image>().color = panelBg;

        // 2)  Add a VerticalLayoutGroup for automatic stacking
        VerticalLayoutGroup vlg = panel.GetComponent<VerticalLayoutGroup>();
        if (vlg == null) vlg = panel.gameObject.AddComponent<VerticalLayoutGroup>();

        vlg.childAlignment     = TextAnchor.MiddleCenter;
        vlg.childForceExpandHeight = vlg.childForceExpandWidth = false;
        vlg.spacing = spacing;

        // 3)  Theme each button + text + transition colours
        foreach (Button btn in menuButtons)
        {
            if (btn == null) continue;

            RectTransform rt = btn.GetComponent<RectTransform>();
            rt.SetParent(panel, false);          // parent under panel
            rt.sizeDelta = new Vector2(buttonWidth, buttonHeight);

            // Button background    
            Image img = btn.GetComponent<Image>();
            img.color = buttonBg;

            // Button text (TextMeshProUGUI expected)
            TextMeshProUGUI tmp = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null)
            {
                tmp.color = buttonTxt;
                tmp.fontSize = 38;
            }

            // Tweak Unity's built-in ColourBlock for hover/pressed
            ColorBlock cb = btn.colors;
            cb.normalColor      = buttonBg;
            cb.highlightedColor = highlight;
            cb.pressedColor     = highlight * 0.9f;
            cb.selectedColor    = highlight;
            cb.disabledColor    = new Color(0.5f,0.5f,0.5f,0.35f);
            cb.fadeDuration     = 0.08f;
            btn.colors = cb;
        }
    }

    // --------------- helpers ---------------

    private RectTransform GetOrCreatePanel(Transform canvas)
    {
        Transform existing = canvas.Find("MenuPanel");
        if (existing != null) return existing as RectTransform;

        GameObject go = new GameObject("MenuPanel", typeof(RectTransform), typeof(Image));
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.SetParent(canvas, false);

        // full-screen anchor but keeps layout group content centred
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;

        return rt;
    }
}
