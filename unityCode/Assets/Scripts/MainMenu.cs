
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Attach this script to an empty GameObject (e.g., "MainMenu") in your start‑screen scene.
/// Wire the public Button fields to the corresponding UI Buttons in the Inspector.
/// The individual Open* methods are stubbed with Debug.Log calls; replace these
/// with SceneManager.LoadScene() or panel‑activation logic once those scenes/panels exist.
/// </summary>
public class MainMenu : MonoBehaviour
{
    [Header("Menu Buttons")]
    [SerializeField] private Button storeButton;
    [SerializeField] private Button multiplayerButton;
    [SerializeField] private Button singleplayerButton;
    [SerializeField] private Button optionsButton;

    private void Awake()
    {
        // Hook up the button callbacks
        storeButton.onClick.AddListener(OpenStore);
        multiplayerButton.onClick.AddListener(OpenMultiplayer);
        singleplayerButton.onClick.AddListener(OpenSingleplayer);
        optionsButton.onClick.AddListener(OpenOptions);
    }

    private void OpenStore()
    {
        Debug.Log("Store button pressed");
        // Example: SceneManager.LoadScene("StoreScene");
    }

    private void OpenMultiplayer()
    {
        Debug.Log("Multiplayer button pressed");
        // Example: SceneManager.LoadScene("MultiplayerMenu");
    }

    private void OpenSingleplayer()
    {
        Debug.Log("Singleplayer button pressed");
        // Example: SceneManager.LoadScene("SingleplayerMenu");
    }

    private void OpenOptions()
    {
        Debug.Log("Options button pressed");
        // Example: SceneManager.LoadScene("OptionsMenu");
    }
}
