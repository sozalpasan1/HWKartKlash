using TMPro;
using UnityEngine;

public class IPDisplayHandler : MonoBehaviour
{
    [SerializeField] private string playerIPTextObjectName = "PlayerIPText";
    
    void Start()
    {
        // Find the TMP text object
        TextMeshProUGUI ipText = GameObject.Find(playerIPTextObjectName)?.GetComponent<TextMeshProUGUI>();
        
        if (ipText != null)
        {
            // Display the IP from the static variable
            ipText.text = "Your IP: " + HostButtonHandler.PlayerIPToDisplay;
            Debug.Log("Setting IP text to: " + HostButtonHandler.PlayerIPToDisplay);
        }
        else
        {
            Debug.LogError("IP Text component named '" + playerIPTextObjectName + "' not found!");
        }
    }
}