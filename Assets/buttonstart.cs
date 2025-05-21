using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonstart : MonoBehaviour
{
    // This function can be called by a UI Button's OnClick event
    public void OnStartButtonPressed()
    {
        SceneManager.LoadScene("Multiplayer Screen");
    }
}
