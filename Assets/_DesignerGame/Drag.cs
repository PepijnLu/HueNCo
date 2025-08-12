using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.Netcode;

public class Drag : NetworkBehaviour
{
    public bool isDragging = false;
    private Vector3 offset;
    public Vector3 lastPosition;
    Vector3Int cellPosition;
    public Tilemap tilemap;
    bool collidingWithTile;
    public DGM designerGameManager;
    public Camera cameraToUse;

    void Start()
    {
        DGM.instance.objectsList.Add(gameObject);
    }

    public override void OnNetworkSpawn()
    {
        cameraToUse = Lobby.instance.players[0].gameObject.GetComponent<PlayerMovement>().playerCam;
        lastPosition = transform.position;
    }
    private void OnMouseDown()
    {
        if (DGM.instance.whichInstance == 1)
        {
            isDragging = true;
            lastPosition = transform.position;
            NetworkShit.instance.ChangeDragging(true);
        }
    }

    private void OnMouseUp()
    {
        if (DGM.instance.whichInstance == 1)
        {
            isDragging = false;
            lastPosition = transform.position;
            NetworkShit.instance.ChangeDragging(false);
        }
    }

    private void Update()
    {
        Vector3 mousePosition = cameraToUse.ScreenToWorldPoint(Input.mousePosition);

        if ( (designerGameManager.designing) && (DGM.instance.whichInstance == 1))
        {
            if (isDragging)
            {
                // Update the object's position to follow the mouse position
                // Move the object to the mouse position
                transform.position = Lobby.instance.players[0].gameObject.GetComponent<PlayerMovement>().mousePosition;

                if (Input.GetKeyDown(KeyCode.Q))
                {
                    RotateObject(90f);
                }
                else if (Input.GetKeyDown(KeyCode.E))
                {
                    RotateObject(-90f);
                }
            }
        }

        if (DGM.instance.whichInstance == 1)
        {
            if ((!isDragging) && (gameObject.tag == "Bomb"))
            {
                //NetworkShit.instance.ChangeBombLocation(gameObject.transform.position);
                cellPosition = tilemap.WorldToCell(gameObject.transform.position);
                if (tilemap.HasTile(cellPosition))
                {
                    // Debug.Log("BOOM");
                    // tilemap.SetTile(cellPosition, null);
                    // gameObject.transform.position = new Vector3(-100, -100, -100);
                    // //Destroy(gameObject);
                    GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                    foreach (GameObject player in players)
                    {
                        player.GetComponent<PlayerMovement>().BlowUpTile(gameObject.transform.position, DGM.instance.objectsList.IndexOf(gameObject));
                    }
                }
            }
        }
        // else
        // {
        //     if ((!NetworkShit.instance.isDragging) && (gameObject.tag == "Bomb"))
        //     {
        //         //NetworkShit.instance.ChangeBombLocation(gameObject.transform.position);
        //         cellPosition = tilemap.WorldToCell(gameObject.transform.position);
        //         if (tilemap.HasTile(cellPosition))
        //         {
        //             Debug.Log("BOOM");
        //             tilemap.SetTile(cellPosition, null);
        //             gameObject.transform.position = new Vector3(-100, -100, -100);
        //             //Destroy(gameObject);
        //         }
        //     }
        // }
        
    }

    private Vector3 GetMouseWorldPosition()
    {
        // Get the mouse position in screen coordinates and convert it to world coordinates
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    private void RotateObject(float angle)
    {
        transform.Rotate(Vector3.forward, angle);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((gameObject.tag == "Bomb") && (collision.gameObject.tag == "CollisionMap"))
        {
            Debug.Log("colliding with tile: " + collidingWithTile);
            collidingWithTile = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if ((gameObject.tag == "Bomb") && (collision.gameObject.tag == "CollisionMap"))
        {
            Debug.Log("colliding with tile: " + collidingWithTile);
            collidingWithTile = false;
        }
    }
}
