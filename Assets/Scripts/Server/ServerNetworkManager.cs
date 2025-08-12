using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System;
using Unity.Networking.Transport;

public class ServerNetworkManager : NetworkManager
{
    private void OnEnable()
    {
        UnityTransport transport = gameObject.AddComponent<UnityTransport>();
        NetworkConfig.NetworkTransport = transport;

        
        OnServerStarted += OnServerStartedCallback;
        OnClientConnectedCallback += OnClientConnected;
        OnClientDisconnectCallback += OnClientDisconnect;

        StartServer();
    }

    private void OnServerStartedCallback()
    {
        Console.WriteLine("Server listening for connections......");
    }
    private void OnClientConnected(ulong obj)
    {
        Console.WriteLine("Client Connected!");
    }
    private void OnClientDisconnect(ulong obj)
    {
        Console.WriteLine("Client Disconnected!");
    }

}