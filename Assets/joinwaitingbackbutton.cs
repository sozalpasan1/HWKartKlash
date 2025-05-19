using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButtonHandler2 : MonoBehaviour
{
    // This function can be called by a UI Button's OnClick event
    public void OnBackButtonPressed2()
    {
        SceneManager.LoadScene("Join Screen");
    }
}
