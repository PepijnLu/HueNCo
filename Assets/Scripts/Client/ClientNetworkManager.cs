using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;


public class ClientNetworkManager : NetworkManager
{
    // Start is called before the first frame update
    void Start()
    {
        UnityTransport transport = gameObject.AddComponent<UnityTransport>();
        NetworkConfig.NetworkTransport = transport;

        if (PlayerPrefs.HasKey("Gamertag"))
        {
            string gamerTag = PlayerPrefs.GetString("Gamertag");
        }
        else
        {
            string gamerTag = System.Guid.NewGuid().ToString();
            PlayerPrefs.SetString("Gamertab", gamerTag);
        }

        StartClient();
        Debug.Log("Client Ready");
    }

    
}