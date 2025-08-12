using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking;

public class StartMenuButtons : NetworkBehaviour
{
    // Start is called before the first frame update
    public int buttonInt;
    public GameObject startScreen, pleaseWait, pleaseChooseColor;
    public GameObject blueCheck, redCheck, yellowCheck;
    LobbyPlayer hostScript;
    public NetworkManager networkManager;

    void Start()
    {
        blueCheck.SetActive(false);
        redCheck.SetActive(false);
        yellowCheck.SetActive(false);
    }
    public void WhenClicked()
    {
        if (Lobby.instance.players.Count > 0)
        {
            hostScript = Lobby.instance.players[0].gameObject.GetComponent<LobbyPlayer>();
        }
        switch(buttonInt)
        {
            case 1:
                NetworkManager.StartHost();
                gameObject.SetActive(false);
                Lobby.instance.playersConnectedText.gameObject.SetActive(true);
                break;
            case 2:
                NetworkManager.Singleton.StartClient();
                Lobby.instance.playersConnectedText.gameObject.SetActive(true);
                break;
            case 3:
                int playersConnected = 0;
                //if (GameData.team != null)
                //{
                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                foreach (GameObject player in players)
                {
                    playersConnected++;
                }
                if (playersConnected == 3)
                {
                    if (GameData.team != null)
                    {
                        hostScript.StartGameShow();
                        startScreen.SetActive(false);
                    }
                    else
                    {
                        StartCoroutine(SetObjectActive(pleaseChooseColor));
                    }
                }
                else
                {
                    StartCoroutine(SetObjectActive(pleaseWait));
                }
                break;
            case 4:
                if (IsServer)
                {
                    hostScript.UpdateChecks(0, true, "Red");
                    hostScript.UpdateChecks(1, false, "no");
                    hostScript.UpdateChecks(2, false, "no");
                }
                break;
            case 5:
                if (IsServer)
                {
                    hostScript.UpdateChecks(0, false, "no");
                    hostScript.UpdateChecks(1, true, "Blue");
                    hostScript.UpdateChecks(2, false, "no");
                }
                break;
            case 6:
                if (IsServer)
                {
                    hostScript.UpdateChecks(0, false, "no");
                    hostScript.UpdateChecks(1, false, "no");
                    hostScript.UpdateChecks(2, true, "Yellow");
                }
                break;
            case 7:
                Cursor.visible = true;
                Application.Quit();
                break;
            case 8:
                GameObject[] playerss = GameObject.FindGameObjectsWithTag("Player");
                foreach (GameObject player in playerss)
                {
                    player.GetComponent<PlayerTrivia>().LockIn();
                }
                break;
        }
    }

    IEnumerator SetObjectActive(GameObject obj)
    {
        obj.SetActive(true);
        yield return new WaitForSeconds(2f);
        obj.SetActive(false);
    }
}
