using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonHandler5 : MonoBehaviour
{
    // This function can be called by a UI Button's OnClick event
    public void OnHostScreenButtonPressed()
    {
        SceneManager.LoadScene("Host Screen");
    }
}
