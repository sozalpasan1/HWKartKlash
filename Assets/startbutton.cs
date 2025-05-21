using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    // This function can be called by a UI Button's OnClick event
    public void OnStartButtonPressed()
    {
        SceneManager.LoadScene("Account Screen");
    }
}
