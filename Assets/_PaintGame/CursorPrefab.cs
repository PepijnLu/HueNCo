using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CursorPrefab : NetworkBehaviour
{
    public Color color;
    SpriteRenderer spriteRenderer;
    public List<GameObject> objectsToPaint;
    Paintable paintable;
    public int playerInt;
    private readonly NetworkVariable<Color> serverColor = new(writePerm: NetworkVariableWritePermission.Owner);
    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(WaitForStart());
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            Vector3 mousePosition = Input.mousePosition;

            // Convert the mouse position from screen space to world space
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

            // Ensure the object stays at its original z-position
            mouseWorldPosition.z = transform.position.z;

            // Set the position of the object to the mouse position
            transform.position = mouseWorldPosition;

            if (Input.GetMouseButtonUp(0))
            {
                if ( (objectsToPaint.Count > 0) && (Paint.instance.gameStarted) && (Paint.instance.ableToPaint))
                {
                    paintable = objectsToPaint[0].GetComponent<Paintable>();
                    if (paintable != null)
                    {
                        paintable.ChangeColor(color, gameObject);
                    }
                }
            }

            if (Input.GetMouseButtonUp(1))
            {
                if ((objectsToPaint.Count > 0) && (Paint.instance.gameStarted) && (Paint.instance.ableToPaint))
                {
                    paintable = objectsToPaint[0].GetComponent<Paintable>();
                    if (paintable != null)
                    {
                        paintable.ChangeColor(Paint.instance.paintColors["white"], gameObject);
                    }
                }
            }
        }
    }

    public IEnumerator WaitForStart()
    {
        yield return new WaitForSeconds(5f);
        if (IsOwner)
        {
            CustomStart();
        }
    }
    public void CustomStart()
    {
        //transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        playerInt = NetworkShit.instance.localPlayerAmount;

        objectsToPaint = new List<GameObject>(){};
        spriteRenderer = GetComponent<SpriteRenderer>();
        LobbyPlayer lobbyPlayer = GetComponent<LobbyPlayer>();
        if (Lobby.instance.players.IndexOf(gameObject.GetComponent<NetworkObject>()) == 0)
        {
            color = Paint.instance.paintColors["red"];
            //spriteRenderer.color = color;
            ChangeColorOnServerRpc(color);
        }

        if (Lobby.instance.players.IndexOf(gameObject.GetComponent<NetworkObject>()) == 1)
        {
            color = Paint.instance.paintColors["blue"];
            //spriteRenderer.color = color;
            ChangeColorOnServerRpc(color);

        }
        if (Lobby.instance.players.IndexOf(gameObject.GetComponent<NetworkObject>()) == 2)
        {
            color = Paint.instance.paintColors["yellow"];
            //spriteRenderer.color = color;
            ChangeColorOnServerRpc(color);
        }
    }

    public void ChangeColor(Color newColor, int i)
    {
        if (IsOwner)
        {
            ChangeObjectColorOnServerRpc(newColor, i);
        }
        
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Paintable")
        {
            objectsToPaint.Insert(0, collider.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Paintable")
        {
            objectsToPaint.Remove(collider.gameObject);
        }
    }

    [ServerRpc]
    private void ChangeColorOnServerRpc(Color newColor)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (IsServer && IsOwner)
        {
            serverColor.Value = newColor;
        }
        //spriteRenderer.color = newColor;
        ChangeColorOnClientRpc(newColor);
    }

    [ClientRpc]
    private void ChangeColorOnClientRpc(Color newColor)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (IsOwner)
        {
            serverColor.Value = newColor;
        }
        //spriteRenderer.color = newColor;
    }

    [ServerRpc]
    private void ChangeObjectColorOnServerRpc(Color newColor, int i)
    {
        Debug.Log("ChangeColorOnServerRpc" + i);

        // Paintable paintable = Paint.instance.objectsToPaint[i].GetComponent<Paintable>();
        // SpriteRenderer spriteRenderer = Paint.instance.objectsToPaint[i].GetComponent<SpriteRenderer>();

        // paintable.color = newColor;
        // spriteRenderer.color = newColor;
        ChangeObjectColorOnClientRpc(newColor, i);
    }

    [ClientRpc]
    private void ChangeObjectColorOnClientRpc(Color newColor, int i)
    {
        if (Paint.instance.objectsToPaint[i] != null)
        {
            Paintable newPaintable = Paint.instance.objectsToPaint[i].GetComponent<Paintable>();
            SpriteRenderer newSpriteRenderer = Paint.instance.objectsToPaint[i].GetComponent<SpriteRenderer>();
            Debug.Log("ChangeColorOnClientRpc" + i);
            newPaintable.color = newColor;
            newSpriteRenderer.color = newColor;
            Paint.instance.CheckIfComplete();
        }
        else
        {
            Debug.Log("list is null");
        }
    }
}
