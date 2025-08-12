using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using UnityEngine.Tilemaps;

public class PlayerMovement : NetworkBehaviour
{
    float horizontalInput;
    float verticalInput;
    float moveSpeed = 5f;
    Vector2 movement;
    Rigidbody2D rb;
    public Camera playerCam;
    public int playerInt;
    public TextMeshProUGUI viewText, enterText;
    public Vector3 mousePosition;
    public UnityEngine.Rendering.Universal.Light2D playerLight;
    public float speed;
    GameObject tileMapObj2;
    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(WaitForStart());
        speed = 5f;

    }
    // Update is called once per frame++
    void Update()
    {
        if (DGM.instance != null)
        {
            if ((IsOwner) && (playerInt != 1) && (DGM.instance.ableToMove))
            {
                horizontalInput = Input.GetAxis("Horizontal");
                verticalInput = Input.GetAxis("Vertical");

                movement = new Vector2(horizontalInput, verticalInput).normalized;
            }
        }

        // if ((playerInt == 1))
        // {
        //     Vector3 originalMousePosition = Input.mousePosition;

        //     // Convert the mouse position from screen space to world space
        //     Vector3 mouseWorldPosition = playerCam.ScreenToWorldPoint(originalMousePosition);
        //     // Set the position of the object to the mouse position
        //     mousePosition = new Vector3(mouseWorldPosition.x, mouseWorldPosition.y, 0);
            
        // }

    }

    void FixedUpdate()
    {
        // Move the character
        if ((rb != null) && (DGM.instance.ableToMove))
        {
            rb.velocity = new Vector2(horizontalInput * speed, verticalInput * speed);
        }
    }

    public void StartDesignerGames()
    {
        if (IsServer)
        {
            StartCoroutine(WaitForStart());
        }
    }

    IEnumerator WaitForStart()
    {
        yield return new WaitForSeconds(0.5f);
        DGM.instance.mazeGameStarted.Value = true;
    }

    public void StartCustomStart(int p1, int p2, int p3)
    {
        if (IsOwner)
        {
            CustomStartServerRpc(p1, p2, p3);
        }
    }
    [ServerRpc]
    private void CustomStartServerRpc(int p1, int p2, int p3)
    {
        if (IsServer)
        {
            CustomStartClientRpc(p1, p2, p3);
        }
    }
    [ClientRpc]
    public void CustomStartClientRpc(int p1, int p2, int p3)
    {
        Debug.Log("p1: " + p1 + "p2: " + p2 + "p3: " + p3);
        Debug.Log("Custom Start Maze");
        if (IsOwner)
        {
            ChangeSprites();
            Debug.Log("Custom Start Maze IsOwner");
            
            rb = gameObject.GetComponent<Rigidbody2D>();
            Collider2D myCollider = GetComponent<Collider2D>();
            myCollider.isTrigger = false;
            Debug.Log("Custom Start");
            playerCam.gameObject.SetActive(true);
            if (Camera.main != null)
            {
                Camera.main.gameObject.SetActive(false);
            }
            playerCam.depth = 1;

            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                GameObject playerLight = player.transform.Find("Light 2D").gameObject;
                playerLight.SetActive(true);
            }

            if (gameObject.GetComponent<LobbyPlayer>().playerInt == p1)
            {
                Debug.Log("Player int is supposed to be: " + p1);
                playerInt = 1;
                Debug.Log("Player int is actually: " + playerInt);
                DGM.instance.viewText.text = "Overseer";
                DGM.instance.whichInstance = 1;
                Debug.Log("Setup Player 1");
                DGM.instance.globalLight.intensity = 1;
                gameObject.transform.position = gameObject.transform.position + new Vector3(0, 0, -50);
                playerCam.gameObject.transform.position = GameObject.Find("DesCamPos").transform.position;
                switch(GameData.designerMinigamesPlayed)
                {
                    case 0:
                        playerCam.orthographicSize = 16;
                        break;
                    case 1:
                        playerCam.orthographicSize = 22;
                        break;
                    case 2:
                        playerCam.orthographicSize = 27;
                        break;
                }
                GameObject[] draggables = GameObject.FindGameObjectsWithTag("Draggable");
                GameObject[] bombs = GameObject.FindGameObjectsWithTag("Bomb");
                foreach (GameObject obj in draggables)
                {
                    obj.GetComponent<Drag>().cameraToUse = playerCam;
                }
                foreach (GameObject obj in bombs)
                {
                    obj.GetComponent<Drag>().cameraToUse = playerCam;
                }
                
            }

            else if (gameObject.GetComponent<LobbyPlayer>().playerInt == p2)
            {
                Debug.Log("Player int is supposed to be: " + p2);
                playerInt = 2;
                Debug.Log("Player int is actually: " + playerInt);
                SpriteRenderer newSpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                switch (Lobby.instance.players.IndexOf(gameObject.GetComponent<NetworkObject>()))
                {
                    case 0: 
                        newSpriteRenderer.sprite = Lobby.instance.redDesigner;
                        break;
                    case 1:
                        newSpriteRenderer.sprite = Lobby.instance.blueDesigner;
                        break;
                    case 2:
                        newSpriteRenderer.sprite = Lobby.instance.yellowDesigner;
                        break;
                }
                DGM.instance.viewText.text = "Escaper";
                DGM.instance.objectsTexts.SetActive(false);
                DGM.instance.enterText.gameObject.SetActive(false);
                DGM.instance.whichInstance = 2;
                Debug.Log("Setup Player 2");
                //playerLight.gameObject.SetActive(true);
                playerCam.gameObject.SetActive(true);
                gameObject.transform.position = GameObject.Find("PlayerSpawn").transform.position + new Vector3(-2f, 0, 0);
                DGM.instance.globalLight.intensity = 0;
                playerCam.gameObject.transform.position = gameObject.transform.position + new Vector3(0f, 0f, -5);
                playerCam.orthographicSize = 5;
                //viewText.text = ("View: Player");
                //enterText.gameObject.SetActive(false);
            }
            else if (gameObject.GetComponent<LobbyPlayer>().playerInt == p3)
            {
                Debug.Log("Player int is supposed to be: " + p3);
                playerInt = 3;
                Debug.Log("Player int is actually: " + playerInt);
                SpriteRenderer newSpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                switch (Lobby.instance.players.IndexOf(gameObject.GetComponent<NetworkObject>()))
                {
                    case 0: 
                        newSpriteRenderer.sprite = Lobby.instance.redDesigner;
                        break;
                    case 1:
                        newSpriteRenderer.sprite = Lobby.instance.blueDesigner;
                        break;
                    case 2:
                        newSpriteRenderer.sprite = Lobby.instance.yellowDesigner;
                        break;
                }

                gameObject.GetComponent<SpriteRenderer>().sprite = Lobby.instance.yellowDesigner;
                DGM.instance.viewText.text = "Escaper";
                DGM.instance.objectsTexts.SetActive(false);
                DGM.instance.enterText.gameObject.SetActive(false);
                DGM.instance.whichInstance = 3;
                Debug.Log("Setup Player 3");
                //playerLight.gameObject.SetActive(true);
                playerCam.gameObject.SetActive(true);
                gameObject.transform.position = GameObject.Find("PlayerSpawn").transform.position + new Vector3(2f, 0, 0);
                //viewText.text = ("View: Player");
                DGM.instance.globalLight.intensity = 0;
                playerCam.gameObject.transform.position = gameObject.transform.position + new Vector3(0f, 0f, -5);
                playerCam.orthographicSize = 5;
                //enterText.gameObject.SetActive(false);
            }

        }
        else
        {
            playerCam.depth = 0;
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        //if (IsOwner)
        //{
            LobbyPlayer lobbyPlayer = GetComponent<LobbyPlayer>();
            if (collider.gameObject.tag == "Exit")
            {
                Exit exit = collider.gameObject.GetComponent<Exit>();
                if (lobbyPlayer.playerInt == exit.exitInt)
                {
                    //NetworkShit.instance.ChangePlayersFinishedServerRpc(true);
                    DGM.instance.playersFinished++;
                    DGM.instance.CheckForFinish();
                    Debug.Log("reached the exit");
                }
            }
        //}
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        LobbyPlayer lobbyPlayer = GetComponent<LobbyPlayer>();
        //if (IsOwner)
        //{
            if (collider.gameObject.tag == "Exit")
            {
                Exit exit = collider.gameObject.GetComponent<Exit>();
                if (lobbyPlayer.playerInt == exit.exitInt)
                {
                    DGM.instance.playersFinished--;
                }
            }
        //}
    }

    public void ChangeSprites()
    {
        if (IsOwner)
        {
            ChangeSpritesServerRpc();
        }
    }
    [ServerRpc]
    private void ChangeSpritesServerRpc()
    {
        if (IsServer)
        {
            ChangeSpritesClientRpc();
        }
    }
    [ClientRpc]
    private void ChangeSpritesClientRpc()
    {
        GameObject[] playersss = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in playersss)
        {
            int i = player.GetComponent<LobbyPlayer>().playerInt;
            SpriteRenderer spriteRenderer = player.GetComponent<SpriteRenderer>();
            switch (i)
            {
                case 1:
                    spriteRenderer.sprite = Lobby.instance.redDesigner;
                    break;
                case 2:
                    spriteRenderer.sprite = Lobby.instance.blueDesigner;
                    break;
                case 3:
                    spriteRenderer.sprite = Lobby.instance.yellowDesigner;
                    break;
            }
        }
    }

    public void BlowUpTile(Vector3 worldPosition, int i)
    {
        if (IsOwner)
        {
            BlowUpTileServerRpc(worldPosition, i);
        }
    }

    [ServerRpc]
    private void BlowUpTileServerRpc(Vector3 worldPosition, int i)
    {
        BlowUpTileClientRpc(worldPosition, i);
    }

    [ClientRpc]
    private void BlowUpTileClientRpc(Vector3 worldPosition, int i)
    {   
        if (i == -1)
        {
            GameObject tilemapObj = GameObject.FindGameObjectWithTag("PlayerBox");
            Tilemap tilemap = tilemapObj.GetComponent<Tilemap>();

            GameObject[] entrances = GameObject.FindGameObjectsWithTag("Entrance");
            foreach (GameObject entrance in entrances)
            {
                Vector3Int cellPosition = tilemap.WorldToCell(entrance.transform.position);
                if (tilemap.HasTile(cellPosition))
                {
                    tilemap.SetTile(cellPosition, null);
                }
            }
        }
        else
        {
            switch(GameData.designerMinigamesPlayed)
            {
                case 0:
                    tileMapObj2 = GameObject.Find("Tilemap1");
                    break;
                case 1:
                    tileMapObj2 = GameObject.Find("Tilemap1");
                    break;
                case 2:
                    tileMapObj2 = GameObject.Find("Tilemap1");
                    break;  
            }
            Tilemap tilemap2 = tileMapObj2.GetComponent<Tilemap>();
            Vector3Int cellPosition = tilemap2.WorldToCell(worldPosition);
            if (tilemap2.HasTile(cellPosition))
            {
                tilemap2.SetTile(cellPosition, null);
            }

        }
        
        if (i >= 0)
        {
            Destroy(DGM.instance.objectsList[i]);
        }
    }
}
