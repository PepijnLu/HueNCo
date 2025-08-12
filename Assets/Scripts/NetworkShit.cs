using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Tilemaps;

public class NetworkShit : NetworkBehaviour
{
    // UnityTransport transport = GetComponent<UnityTransport>();
    // NetworkConfig.NetworkTransport = transport;
    // m_SomeValue.Value = k_InitialValue; 

    // OnServerStarted += OnServerStartedCallback;
    // OnClientConnectedCallback += OnClientConnected;
    // OnClientDisconnectCallback += OnClientDisconnect;
    // OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
    // m_SomeValue.OnValueChanged += OnSomeValueChanged;

    public static NetworkShit instance;
    private readonly NetworkVariable<int> serverPlayerAmount = new(writePerm: NetworkVariableWritePermission.Owner);
    private readonly NetworkVariable<bool> serverDragging = new(writePerm: NetworkVariableWritePermission.Owner);
    private readonly NetworkVariable<int> playersFinished = new(writePerm: NetworkVariableWritePermission.Owner);
    public int localPlayersFinished;
    public bool isDragging;
    public Tilemap tilemap;
    public int k_InitialValue = 0;
    public int localPlayerAmount;

    public override void OnNetworkSpawn()
    {
        Debug.Log("OnNetworkSpawn Networkshit");
        instance = this;
       if (IsServer)
       {
           serverPlayerAmount.Value = k_InitialValue;
           NetworkManager.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
       }
       else
       {
           if (serverPlayerAmount.Value != k_InitialValue)
           {
               Debug.LogWarning($"NetworkVariable was {serverPlayerAmount.Value} upon being spawned" +
                   $" when it should have been {k_InitialValue}");
           }
           else
           {
               Debug.Log($"NetworkVariable is {serverPlayerAmount.Value} when spawned.");
           }
           serverPlayerAmount.OnValueChanged += OnSomeValueChanged;
           serverDragging.OnValueChanged += ChangeServerDragging;
           playersFinished.OnValueChanged += ChangeLocalPlayersFinished;
       }
   }

   private void NetworkManager_OnClientConnectedCallback(ulong obj)
   {
       ChangeNetworkVariable();
       
   }

   private void OnSomeValueChanged(int previous, int current)
   {
       Debug.Log($"Detected NetworkVariable Change: Previous: {previous} | Current: {current}");
       //if (IsOwner)
        //{
            localPlayerAmount = current;
            Debug.Log("Running");
            Debug.Log(IsOwner + " " + IsServer);
            // if (current == 2)
            // {
            //     playerInt = 2;
            //     gameObject.transform.position = GameObject.Find("PlayerSpawn").transform.position;
            // }
            // if (current == 3)
            // {
            //     playerInt = 3;
            //     gameObject.transform.position = GameObject.Find("PlayerSpawn").transform.position;
            // }
   }

   private void ChangeServerDragging(bool previous, bool current)
   {
        isDragging = serverDragging.Value;
   }

   private void ChangeLocalPlayersFinished(int previous, int current)
   {

   }

    // [Rpc(SendTo.Server)]
    // public void ToggleServerRpc()
    // {
    //     // this will cause a replication over the network
    //     // and ultimately invoke `OnValueChanged` on receivers
    //     serverDragging.Value = !serverDragging.Value;
    //     Debug.Log("ToggleServerRpc");
    // }

   private void ChangeNetworkVariable()
   {
        serverPlayerAmount.Value++;
        ChangePlayerAmountServerRpc(serverPlayerAmount.Value);
   }

   public void ChangeDragging(bool newValue)
   {
        //serverDragging.Value = newValue;
   }

   [Rpc(SendTo.Server)]
   public void ChangePlayersFinishedServerRpc(bool add)
   {
    if (add)
    {
        playersFinished.Value++;
    }
    else
    {
        playersFinished.Value--;
    }
    localPlayersFinished = playersFinished.Value;
    Debug.Log(playersFinished.Value);
   }


   [Rpc(SendTo.Server)]
   public void ChangePlayerAmountServerRpc(int value)
   {
        serverPlayerAmount.Value = value;
        localPlayerAmount = playersFinished.Value;
        Debug.Log(serverPlayerAmount.Value);
   }

//     [Rpc(SendTo.Server)]
//     public void BlowUpMoreTilesServerRpc(Vector3 bombPosition.Value)
//     {
//         //bombPosition.Value
//     }
}
