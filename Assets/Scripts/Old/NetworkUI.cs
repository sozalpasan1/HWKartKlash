using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkUI : MonoBehaviour
{

    public Button hostButton;
    public Button clientButton;

    void Awake()
    {
        hostButton.onClick.AddListener(StartHost);

        clientButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
        });
    }

    private void StartHost()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.StartHost();
            Debug.Log("Host started");
        } else
        {
            Debug.Log("No host started");
        }
    }

}
