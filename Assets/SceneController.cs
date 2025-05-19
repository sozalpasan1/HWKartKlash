
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Attach to an empty GameObject (e.g., "SceneController") in your menu scene.
/// Drag your UI Buttons into the Inspector.
/// The script wires up button clicks to scene loads.
/// </summary>
public class SceneController : MonoBehaviour
{
    [Header("Scene Buttons")]
    [SerializeField] private Button storeButton;
    [SerializeField] private Button multiplayerButton;
    [SerializeField] private Button singleplayerButton;
    [SerializeField] private Button optionsButton;

    // Names must match your scene filenames (without .unity)
    [Header("Scene Names")]
    [SerializeField] private string storeScene        = "StoreScene";
    [SerializeField] private string multiplayerScene  = "MultiplayerScene";
    [SerializeField] private string singleplayerScene = "SingleplayerScene";
    [SerializeField] private string optionsScene      = "OptionsScene";

    private void Awake()
    {
        if (storeButton != null)
            storeButton.onClick.AddListener(() => LoadScene(storeScene));
        if (multiplayerButton != null)
            multiplayerButton.onClick.AddListener(() => LoadScene(multiplayerScene));
        if (singleplayerButton != null)
            singleplayerButton.onClick.AddListener(() => LoadScene(singleplayerScene));
        if (optionsButton != null)
            optionsButton.onClick.AddListener(() => LoadScene(optionsScene));
    }

    private void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
