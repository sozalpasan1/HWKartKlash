using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonHandler4 : MonoBehaviour
{
    // This function can be called by a UI Button's OnClick event
    public void OnJoinScreenButtonPressed()
    {
        SceneManager.LoadScene("Join Screen");
    }
}
