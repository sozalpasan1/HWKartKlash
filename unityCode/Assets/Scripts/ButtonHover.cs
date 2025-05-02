
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Attach this to each Button that should highlight the hover cursor.
/// Works for both mouse hover (OnPointerEnter) and keyboard/gamepad navigation (OnSelect).
/// </summary>
[RequireComponent(typeof(UnityEngine.UI.Selectable))]
public class ButtonHover : MonoBehaviour, IPointerEnterHandler, ISelectHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        NotifyCursorManager();
    }

    public void OnSelect(BaseEventData eventData)
    {
        NotifyCursorManager();
    }

    private void NotifyCursorManager()
    {
        if (CursorManager.Instance != null)
        {
            CursorManager.Instance.SetTarget(transform as RectTransform);
        }
    }
}
