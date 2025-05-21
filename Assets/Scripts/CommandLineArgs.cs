using UnityEngine;
using Unity.Netcode;

public class CommandLineArgs : MonoBehaviour
{
    void Start()
    {
        string[] args = System.Environment.GetCommandLineArgs();
        bool isHost = false;
        bool isClient = false;
        string ip = "127.0.0.1";
        ushort port = 7777;

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-host") isHost = true;
            if (args[i] == "-client") isClient = true;
            if (args[i] == "-ip" && i + 1 < args.Length) ip = args[i + 1];
            if (args[i] == "-port" && i + 1 < args.Length) ushort.TryParse(args[i + 1], out port);
        }

        var transport = NetworkManager.Singleton.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>();
        if (transport != null)
        {
            transport.ConnectionData.Address = ip;
            transport.ConnectionData.Port = port;
        }

        if (isHost) NetworkManager.Singleton.StartHost();
        else if (isClient) NetworkManager.Singleton.StartClient();
    }
}