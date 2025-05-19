using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButtonHandler1 : MonoBehaviour
{
    // This function can be called by a UI Button's OnClick event
    public void OnBackButtonPressed1()
    {
        SceneManager.LoadScene("Multiplayer Screen");
    }
}
