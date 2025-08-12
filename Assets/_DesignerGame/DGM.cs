using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Tilemaps;
using Unity.Netcode;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

/*
SCORE COUNTER FOR DESIGNER GAME
TRIVIAAA
CUTSCENES
SAVING HIGH SCORE TO LIKE JSON OR SOMETHING
*/

public class DGM : NetworkBehaviour
{
    public readonly NetworkVariable<bool> mazeGameStarted = new(writePerm: NetworkVariableWritePermission.Owner);
    public readonly NetworkVariable<float> progress = new(writePerm: NetworkVariableWritePermission.Owner);
    public readonly NetworkVariable<float> timeRemainingServer = new(writePerm: NetworkVariableWritePermission.Owner);

    public GameObject game1objects, game2objects, game3objects;

    public static DGM instance;
    public bool designing;
    public Camera mainCamera;
    public UnityEngine.Rendering.Universal.Light2D globalLight;
    public TextMeshProUGUI viewText, enterText, scoreText;
    public GameObject canvas, objectsTexts;
    public GameObject objectsPack1, objectsPack2, objectsPack3;
    public Tilemap tilemap1, tilemap2, tilemap3;
    public Transform entrance;
    public List<GameObject> objectsList;
    public int whichInstance;
    public Slider timeSlider;
    public int timeRemaining, escapeTime;
    public bool gameStarted;
    public int playersFinished;
    public int points;
    public bool ableToMove;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        objectsList = new List<GameObject>(){};
    }
    void Start()
    {

    }

    public override void OnNetworkSpawn()
    {
        //mazeGameStarted.OnValueChanged += StartTheNextGame;
        //StartTheNextGame(true, true);
        progress.OnValueChanged += UpdateSlider;

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            player.GetComponent<PlayerMovement>().StartDesignerGames();
        }

        escapeTime = 30;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && (designing) && (whichInstance == 1))
        {
            designing = false;
            globalLight.intensity = 0;
            enterText.gameObject.SetActive(false);
            objectsTexts.SetActive(false);

            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                player.GetComponent<PlayerMovement>().BlowUpTile(entrance.position, -1);
                GameObject playerCamera = player.transform.Find("Light 2D").gameObject;
                playerCamera.SetActive(true);
            }
        }
    }

    public void StartTheNextGame()
    {
        if (GameData.designerMinigamesPlayed == 0)
        {
            objectsPack1.SetActive(false);
            objectsPack2.SetActive(false);
            objectsPack3.SetActive(false);
            tilemap1.gameObject.SetActive(true);
            tilemap2.gameObject.SetActive(false);
            tilemap3.gameObject.SetActive(false);
            //code to run for first game   
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                player.GetComponent<PlayerMovement>().StartCustomStart(1, 2, 3);
            }
        }
        else if (GameData.designerMinigamesPlayed == 1)
        {
            objectsPack1.SetActive(false);
            objectsPack2.SetActive(false);
            objectsPack3.SetActive(false);
            tilemap1.gameObject.SetActive(false);
            tilemap2.gameObject.SetActive(true);
            tilemap3.gameObject.SetActive(false);
            //code to run for second game
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                player.GetComponent<PlayerMovement>().StartCustomStart(3, 1, 2);
            }
        }
        else if (GameData.designerMinigamesPlayed == 2)
        {
            objectsPack1.SetActive(false);
            objectsPack2.SetActive(false);
            objectsPack3.SetActive(false);
            tilemap1.gameObject.SetActive(false);
            tilemap2.gameObject.SetActive(false);
            tilemap3.gameObject.SetActive(true);
            //code to run for third game
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                player.GetComponent<PlayerMovement>().StartCustomStart(2, 3, 1);
            }
        }
        else if (GameData.designerMinigamesPlayed == 3)
        {
            //Load the lobby scene
            if (IsServer)
            {
                string m_SceneName = "Lobby";
                var status = NetworkManager.SceneManager.LoadScene(m_SceneName, LoadSceneMode.Single);
                if (status != SceneEventProgressStatus.Started)
                {
                    Debug.LogWarning($"Failed to load {m_SceneName} " +
                            $"with a {nameof(SceneEventProgressStatus)}: {status}");
                }
            }
        }

        if (GameData.designerMinigamesPlayed < 3)
        {
            //code to run for every minigame
            designing = true;

            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                LobbyPlayer lobbyScript = player.GetComponent<LobbyPlayer>();
                lobbyScript.StartTimer(5, gameObject, "EndDesignPeriod");
                ableToMove = true;
            }
        }
    }
    private void UpdateSlider(float previous, float current)
    {
        timeSlider.value = Mathf.Lerp(30, 0, current);
    }

    public void EndDesignPeriod()
    {
        GameObject[] bombObjects = GameObject.FindGameObjectsWithTag("Bomb");
        GameObject[] draggableObjects = GameObject.FindGameObjectsWithTag("Draggable");
        GameObject[] allObjects = bombObjects.Concat(draggableObjects).ToArray();
        foreach (GameObject obj in allObjects)
        {
            if (obj.GetComponent<Drag>().lastPosition != obj.transform.position)
            {
                obj.transform.position = obj.GetComponent<Drag>().lastPosition;
            }
        }

        if (designing && whichInstance == 1)
        {
            designing = false;
            //globalLight.intensity = 0;
            enterText.gameObject.SetActive(false);
            objectsTexts.SetActive(false);

            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                player.GetComponent<PlayerMovement>().BlowUpTile(entrance.position, -1);
                GameObject playerCamera = player.transform.Find("Light 2D").gameObject;
                playerCamera.SetActive(true);
            }
        }

        gameStarted = true;
        switch(GameData.designerMinigamesPlayed)
        {
            case 0:
                //supposed to be 20
                escapeTime = 20;
                break;
            case 1:
                //supposed to be 30
                escapeTime = 40;
                break;
            case 2:
                //supposed to be 60
                escapeTime = 60;
                break;
        }
        GameObject[] players2 = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players2)
        {
            LobbyPlayer lobbyScript = player.GetComponent<LobbyPlayer>();
            lobbyScript.StartTimer(escapeTime, gameObject, "EndADesignGame");
        }

    }

    public IEnumerator EndADesignGame(float sliderValue, bool early)
    {
        ableToMove = false;
        if (gameStarted)
        {
            if (early) {    playersFinished = 2;    }
            gameStarted = false;
            float percentageTimeRemaining = (sliderValue / 30);
            int points1 = Mathf.FloorToInt(playersFinished * 500);
            int points2 = Mathf.FloorToInt(percentageTimeRemaining * 500);
            points = points1 + points2;
            GameData.totalScore += points;

            float targetNumber1 = points1;
            float targetNumber2 = points2;
            float currentValue1 = 0;
            float currentValue2 = 0;
            scoreText.gameObject.SetActive(true);
            if ((targetNumber1 == 0) && (targetNumber2 == 0))
            {
                scoreText.text = "0";
            }
            else
            {
                while ((currentValue1 < targetNumber1) || (currentValue2 < targetNumber2))
                {
                    // Calculate the amount to add based on speed and deltaTime
                    int increment = 1;

                    // Make sure not to overshoot the target
                    if (currentValue1 < targetNumber1)
                    {
                        currentValue1 = Mathf.Min(currentValue1 + increment, targetNumber1);
                    }
                    if (currentValue2 < targetNumber2)
                    {
                        currentValue2 = Mathf.Min(currentValue2 + increment, targetNumber2);
                    }

                    scoreText.text = (currentValue1.ToString() + " - Exits Reached" + System.Environment.NewLine + currentValue2.ToString() + " - Time Remaining Bonus");

                    // Break out of the loop if currentValue reaches targetNumber
                    if ( (currentValue1 >= targetNumber1) && (currentValue2 >= targetNumber2))
                    {
                        break;
                    }
                    yield return null;
                }
            }

            yield return new WaitForSeconds(5f);
            scoreText.gameObject.SetActive(false);
            GameData.designerMinigamesPlayed++;
            points = 0;

            StartTheNextGame();
        }
        playersFinished = 0;
        yield return null;
    }

    public void CheckForFinish()
    {
        if ((playersFinished >= 2) && (gameStarted))
        {
            GameObject[] players2 = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players2)
            {
                LobbyPlayer lobbyScript = player.GetComponent<LobbyPlayer>();
                lobbyScript.EndMazeGame(true);
            }
        }
    }
}
