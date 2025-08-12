using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerNetwork : NetworkBehaviour
{
    private readonly NetworkVariable<Vector3> _netPos = new(writePerm: NetworkVariableWritePermission.Owner);
    //private Color m_Color;
    //private NetworkVariable<SpriteRenderer> _netSpriteRenderer = new(writePerm: NetworkVariableWritePermission.Owner);
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            //_netPos.Value = transform.position;   
            //m_Color = gameObject.GetComponent<SpriteRenderer>().color;
        }
        else
        {
            //transform.position = _netPos.Value;
            //gameObject.GetComponent<SpriteRenderer>().color = m_Color;

        }
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            Destroy(this);
        }
        else
        {
            //transform.position = new Vector3(0, -27, 0);
            // GameObject networkShit = GameObject.Find("NetworkShit");
            // networkShit.GetComponent<NetworkShit>().AddToPlayerAmount();
            //NetworkShit.instance.players.Add(gameObject);
            //playerInt = NetworkShit.instance.players.Count;
        }
    }

    // struct PlayerNetworkData : INetworkSerializable
    // {
    //     private float _x, _y;

    //     internal Vector3 Position 
    //     {
    //         get => new Vector3(_x, _y, 0);
    //         set
    //         {
    //             _x = value.x;
    //             _y = value.y;
    //         }
    //     }

    //     public void NetworkSerialize<T>(BufferSeralizer<T> serializer) where T : IReaderWriter
    //     {
    //         serializer.SerializeValue(ref _x);
    //         serializer.SerializeValue(ref _y);
    //     }
    // }
}
