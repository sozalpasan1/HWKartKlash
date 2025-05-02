
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls a UI cursor/selector icon that hovers over whichever
/// menu button is currently highlighted (mouse‑hover or keyboard/gamepad selection).
/// Place this on an empty GameObject in the Canvas and assign the
/// cursorIcon field to an Image (or any RectTransform) that contains
/// your arrow/hand/pointer sprite.
/// </summary>
public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance { get; private set; }

    [Tooltip("UI element representing the cursor/selector icon.")]
    [SerializeField] private RectTransform cursorIcon;

    [Tooltip("Lerp speed (units per second) for the cursor to follow its target.")]
    [SerializeField] private float moveSpeed = 12f;

    // Current target the cursor should follow
    private RectTransform target;

    private void Awake()
    {
        // Simple singleton so other scripts can talk to us quickly.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Hide the cursor if no target is set at start.
        if (cursorIcon != null)
            cursorIcon.gameObject.SetActive(false);
    }

    /// <summary>
    /// Public API: call this when a button gets focus/hover to move the cursor there.
    /// </summary>
    public void SetTarget(RectTransform newTarget)
    {
        target = newTarget;

        if (cursorIcon == null) return;

        cursorIcon.gameObject.SetActive(true);
        // Jump instantly so the cursor doesn't tween from (0,0) the first time.
        cursorIcon.position = target.position;
    }

    private void Update()
    {
        if (cursorIcon == null || target == null) return;

        // Smoothly interpolate toward the target position.
        cursorIcon.position = Vector3.Lerp(cursorIcon.position, target.position, Time.deltaTime * moveSpeed);
    }
}
