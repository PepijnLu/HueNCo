using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Paintable : NetworkBehaviour
{
    // Start is called before the first frame update
    SpriteRenderer spriteRenderer;
    private readonly NetworkVariable<Color> serverColor = new(writePerm: NetworkVariableWritePermission.Owner);
    //public GameObject paintManagerObj;
    public Paint paintManager;
    public Color color;
    public Color userColor;
    public string correctColor;
    string actualCorrectColor;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        paintManager.objectsToPaint.Add(gameObject);
    }

    public override void OnNetworkSpawn()
    {
        //color.OnValueChanged += ColorToNetworkVariable;
        //serverColor.OnValueChanged += ChangeServerColor;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            userColor = paintManager.paintColors["red"];
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            userColor = paintManager.paintColors["blue"];
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            userColor = paintManager.paintColors["yellow"];
        }
    }

    public void ColorToNetworkVariable()
    {
        serverColor.Value = color;
    }

    private void ChangeServerColor(int previous, int current)
    {
        
    }

    public bool IsCorrectColor()
    {
        return (color == paintManager.paintColors[correctColor]);
    }
    // public void OnMouseDown()
    // {
    //     Debug.Log("Clicked");
    //     if (paintManager.gameStarted)
    //     {
    //         //ChangeColor(userColor);
    //     }
    // }

    // private void OnMouseOver()
    // {
    //     if (Input.GetMouseButtonDown(1)) // Check if right mouse button is clicked (1 = right mouse button)
    //     {
    //         color = paintManager.paintColors["white"];
    //         spriteRenderer.color = color;
    //     }
    // }

    public void ChangeColor(Color newColor, GameObject cursor)
    {
        Debug.Log("Change Color");

        if (newColor == paintManager.paintColors["white"])
        {
            color = paintManager.paintColors["white"];
            spriteRenderer.color = color;
        }
        else if (color == paintManager.paintColors["white"])
        {
            if (newColor == paintManager.paintColors["blue"])
            {
                color = paintManager.paintColors["blue"];
                spriteRenderer.color = color;
            }
            if (newColor == paintManager.paintColors["red"])
            {
                color = paintManager.paintColors["red"];
                spriteRenderer.color = color;
            }
            if (newColor == paintManager.paintColors["yellow"])
            {
                color = paintManager.paintColors["yellow"];
                spriteRenderer.color = color;
            }
        }
        else if (color == paintManager.paintColors["blue"])
        {
            if (newColor == paintManager.paintColors["blue"])
            {
                //return
            }
            if (newColor == paintManager.paintColors["red"])
            {
                color = paintManager.paintColors["purple"];
                spriteRenderer.color = color;
            }
            if (newColor == paintManager.paintColors["yellow"])
            {
                color = paintManager.paintColors["green"];
                spriteRenderer.color = color;
            }
        }
        else if (color == paintManager.paintColors["red"])
        {
            if (newColor == paintManager.paintColors["blue"])
            {
                color = paintManager.paintColors["purple"];
                spriteRenderer.color = color;
            }
            if (newColor == paintManager.paintColors["red"])
            {
                //return
            }
            if (newColor == paintManager.paintColors["yellow"])
            {
                color = paintManager.paintColors["orange"];
                spriteRenderer.color = color;
            }
        }
        else if (color == paintManager.paintColors["yellow"])
        {
            if (newColor == paintManager.paintColors["blue"])
            {
                color = paintManager.paintColors["green"];
                spriteRenderer.color = color;
            }
            if (newColor == paintManager.paintColors["red"])
            {
                color = paintManager.paintColors["orange"];
                spriteRenderer.color = color;
            }
            if (newColor == paintManager.paintColors["yellow"])
            {
                //return
            }
        }

        if (color == paintManager.paintColors["green"])
        {
            //return
        }
        if (color == paintManager.paintColors["orange"])
        {
            //return
        }
        if (color == paintManager.paintColors["purple"])
        {
            //return
        }

        cursor.GetComponent<CursorPrefab>().ChangeColor(color, Paint.instance.objectsToPaint.IndexOf(gameObject));
        // if (!IsServer)
        // {
        //     ChangeColorOnServerRpc(color);
        // }
        // if (IsOwner)
        // {
        //     Debug.Log("Call Server Rpc");
        // }
    }
    // [ServerRpc]
    // private void ChangeColorOnServerRpc(Color newColor)
    // {
    //     Debug.Log("ChangeColorOnServerRpc");
    //     spriteRenderer = GetComponent<SpriteRenderer>();
    //     serverColor.Value = newColor;
    //     spriteRenderer.color = newColor;
    // }

    // [ClientRpc]
    // private void ChangeColorOnClientRpc(Color newColor)
    // {
    //     spriteRenderer = GetComponent<SpriteRenderer>();
    //     if (IsOwner)
    //     {
    //         serverColor.Value = newColor;
    //     }
    //     spriteRenderer.color = newColor;
    // }
    // [ServerRpc]
    // private void ChangeColorOnServerRpc(Color newColor)
    // {
    //     Debug.Log("ChangeColorOnServerRpc");
    //     color = newColor;
    //     spriteRenderer.color = newColor;
    //     ChangeColorOnClientRpc(newColor);
    // }

    // [ClientRpc]
    // private void ChangeColorOnClientRpc(Color newColor)
    // {
    //     Debug.Log("ChangeColorOnClientRpc");
    //     color = newColor;
    //     spriteRenderer.color = newColor;
    // }
    
}
