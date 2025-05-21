using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButtonHandler : MonoBehaviour
{
    // This function can be called by a UI Button's OnClick event
    public void OnBackButtonPressed()
    {
        SceneManager.LoadScene("StartScreen");
    }
}
