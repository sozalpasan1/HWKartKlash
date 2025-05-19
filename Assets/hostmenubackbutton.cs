using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButtonHandler3 : MonoBehaviour
{
    // This function can be called by a UI Button's OnClick event
    public void OnBackButtonPressed3()
    {
        SceneManager.LoadScene("Host Screen");
    }
}
