using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButtonHandler0 : MonoBehaviour
{
    // This function can be called by a UI Button's OnClick event
    public void OnBackButtonPressed0()
    {
        SceneManager.LoadScene("Multiplayer Screen");
    }
}
