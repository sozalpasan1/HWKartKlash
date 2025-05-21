using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HostButtonHandler : MonoBehaviour
{
    [SerializeField] private string hostStartScreenName = "Host Start Screen";
    [SerializeField] private string playerIPTextObjectName = "PlayerIPText";
    
    private Button hostButton;
    private string localIP;
    
    // Static variable to pass the IP between scenes
    public static string PlayerIPToDisplay = "";

    private void Awake()
    {
        hostButton = GetComponent<Button>();
        
        if (hostButton == null)
        {
            Debug.LogError("Host Button: Button component not found!");
            return;
        }
        
        hostButton.onClick.AddListener(OnHostButtonClicked);
        
        // Get the local IP address
        localIP = GetLocalIPAddress();
        
        // Store it in the static variable
        PlayerIPToDisplay = localIP;
    }

    private void OnHostButtonClicked()
    {
        // Load the Host Start Screen scene
        SceneManager.LoadScene(hostStartScreenName);
    }

    private string GetLocalIPAddress()
    {
        string ip = "Not found";
        
        try
        {
            // Get IP address that can be used for connecting
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var address in host.AddressList)
            {
                // IPv4 address that's not a loopback
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    ip = address.ToString();
                    break;
                }
            }
            
            // If we couldn't find one through host name, try network interfaces
            if (ip == "Not found")
            {
                foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (item.OperationalStatus == OperationalStatus.Up)
                    {
                        foreach (UnicastIPAddressInformation ipInfo in item.GetIPProperties().UnicastAddresses)
                        {
                            if (ipInfo.Address.AddressFamily == AddressFamily.InterNetwork &&
                                !IPAddress.IsLoopback(ipInfo.Address))
                            {
                                ip = ipInfo.Address.ToString();
                                break;
                            }
                        }
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error getting IP address: " + e.Message);
        }
        
        return ip;
    }
}