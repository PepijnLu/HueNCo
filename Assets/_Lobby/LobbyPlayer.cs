using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class LobbyPlayer : NetworkBehaviour
{
    public CursorPrefab paintScript;
    public PlayerMovement mazeScript;
    public PlayerTrivia triviaScript;
    public SpriteRenderer spriteRenderer;
    public Collider2D myCollider;
    public TextMeshProUGUI scoreTextLobby;
    public GameObject playerLight;
    public GameObject playerCam;
    public int playerInt;
    public readonly NetworkVariable<int> playersConnected = new(writePerm: NetworkVariableWritePermission.Owner);

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public override void OnNetworkSpawn()
    {
        StartCoroutine(WaitForStart());
        SceneManager.sceneLoaded += OnSceneStart;
        playersConnected.OnValueChanged += StartScene;

        Lobby.instance.players.Add(gameObject.GetComponent<NetworkObject>());
        playerInt = Lobby.instance.players.Count;
        //Lobby.instance.chooseTeamColor.SetActive(true);
        Trivia.instance.joinAsClient.SetActive(false);

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            Lobby.instance.playersConnectedText.text = ("Players Connected: " + players.Length.ToString() + "/3");
            if (players.Length == 3)
            {
                Lobby.instance.chooseTeamColor.SetActive(true);
            }
        }
    }

    private void StartScene(int previous, int current)
    {
        int playersConnectedReally = 0;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (player.GetComponent<LobbyPlayer>().playersConnected.Value == 1)
            {
                playersConnectedReally++;
                Debug.Log("players connected really: " + playersConnectedReally);
            }
        }
        if (playersConnectedReally == 3)
        {   
            StartCoroutine(ChillTime());
        }
    }

    IEnumerator ChillTime()
    {
        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            yield return new WaitForSeconds(0.5f);
        }
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            Debug.Log("players connected 3 is owner " + SceneManager.GetActiveScene().name);
            player.GetComponent<LobbyPlayer>().StartSceneServer(SceneManager.GetActiveScene().name);   
            // if (IsOwner)
            // {
            //     player.GetComponent<LobbyPlayer>().playersConnected.Value = 0;
            //     Debug.Log("Set players connected back to 0");
            // }
        }
        yield return null;
    }
    IEnumerator WaitForStart()
    {
        yield return new WaitForSeconds(0.5f);
        CustomStart();
    }

    public void StartGameShow()
    {
        if (IsOwner)
        {
            StartGameShowServerRpc();
        }

    }
    public void CustomStart()
    {
        if (IsOwner)
        {
            paintScript = GetComponent<CursorPrefab>();
            paintScript.enabled = false;

            mazeScript = GetComponent<PlayerMovement>();
            mazeScript.enabled = false;

            triviaScript = GetComponent<PlayerTrivia>();
            triviaScript.enabled = true;

            spriteRenderer = GetComponent<SpriteRenderer>();
            myCollider = GetComponent<Collider2D>();

            Debug.Log("Custom Start " + playerInt);

        }

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in players)
        {
            SpriteRenderer objectSpriteRenderer = player.GetComponent<SpriteRenderer>();
            LobbyPlayer lobbyPlayerScript = player.GetComponent<LobbyPlayer>();

            if (lobbyPlayerScript.playerInt <= 1)
            {
                objectSpriteRenderer.sprite = Lobby.instance.redCursor;
            }

            if (lobbyPlayerScript.playerInt == 2)
            {
                objectSpriteRenderer.sprite = Lobby.instance.blueCursor;    
            }
            if (lobbyPlayerScript.playerInt == 3)
            {
                objectSpriteRenderer.sprite = Lobby.instance.yellowCursor;
            }
        }
    }
    [ServerRpc]
    private void StartGameShowServerRpc()
    {
        StartGameShowClientRpc();
    }
    [ClientRpc]
    private void StartGameShowClientRpc()
    {
        Cursor.visible = false;
        StartCoroutine(Trivia.instance.StartIntro());
        Lobby.instance.startScreen.SetActive(false);
        // string m_SceneName = "MazeMinigame";
        // var status = NetworkManager.SceneManager.LoadScene(m_SceneName, LoadSceneMode.Single);
        // if (status != SceneEventProgressStatus.Started)
        // {
        //     Debug.LogWarning($"Failed to load {m_SceneName} " +
        //             $"with a {nameof(SceneEventProgressStatus)}: {status}");
        // }
    }

    [ServerRpc]
    private void GameSetupServerRpc()
    {
        GameSetupClientRpc();
    }
    [ClientRpc]
    private void GameSetupClientRpc()
    {
        if (playerInt <= 1)
        {
            spriteRenderer.sprite = Lobby.instance.redCursor;
        }

        if (playerInt == 2)
        {
            spriteRenderer.sprite = Lobby.instance.blueCursor;    
        }
        if (playerInt == 3)
        {
            spriteRenderer.sprite = Lobby.instance.yellowCursor;
        }
    }

    private void OnSceneStart(Scene scene, LoadSceneMode mode)
    {
        string sceneName = scene.name;
        if ( (sceneName == "Lobby") && (Trivia.instance != null))
        {
            Trivia.instance.startScreen.SetActive(false);
        }
        if (IsOwner)
        {   
            playersConnected.Value++;
            //StartCoroutine(WaitTwoSeconds(sceneName));
        }
        //Trivia.instance.startScreen.SetActive(false);
    }

    IEnumerator WaitTwoSeconds(string sceneName)
    {
        //Trivia.instance.startScreen.SetActive(false);
        yield return new WaitForSeconds(2f);
        OnSceneStartServerRpc(sceneName);
    }

    public void StartSceneServer(string sceneName)
    {
        Debug.Log("StartSceneServer");
        if (IsOwner)
        {
            OnSceneStartServerRpc(sceneName);
        }
    }

    [ServerRpc]
    private void OnSceneStartServerRpc(string scene)
    {
        if (IsServer)
        {
            Debug.Log("StartSceneServerRpc");
            OnSceneStartClientRpc(scene);
        }
    }

    [ClientRpc]
    private void OnSceneStartClientRpc(string scene)
    {
        if (IsOwner)
        { 
            playersConnected.Value = 0;
            Debug.Log("Set players connected back to 0");
                
            if (scene == "PaintMinigame")
            {
                Debug.Log("Load Paint Scene");
                if (IsServer)
                {
                    StartCoroutine(WaitASecond());
                    GameSetupServerRpc();
                }
                myCollider.isTrigger = true;
                paintScript.enabled = true;
                mazeScript.enabled = false;
                triviaScript.enabled = false;
                paintScript.CustomStart();
            }

            if (scene == "Lobby")
            {
                Debug.Log("Load Lobby Scene");
                Trivia.instance.startScreen.SetActive(false);
                //Lobby.instance.globalLight.gameObject.SetActive(true);
                CustomStart();
                transform.localScale = new Vector3(1f, 1f, 1f);
                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                foreach(GameObject player in players)
                {
                    LobbyPlayer lobbyPlayer = player.GetComponent<LobbyPlayer>();
                    lobbyPlayer.playerLight.SetActive(false);
                    lobbyPlayer.playerCam.SetActive(false);
                    lobbyPlayer.myCollider.isTrigger = true;
                    lobbyPlayer.paintScript.enabled = false;
                    lobbyPlayer.mazeScript.playerCam.gameObject.SetActive(false);
                    lobbyPlayer.mazeScript.enabled = false;
                    lobbyPlayer.triviaScript.enabled = true;
                    lobbyPlayer.transform.position = new Vector3(0, 0, 0);
                }
                // if (GameData.paintMinigamesPlayed > 0)
                // {
                //     GameObject startScreen = GameObject.Find("STARTSCREEN");
                //     startScreen.SetActive(false);
                // }
                GameObject textObj = GameObject.Find("ScoreText");
                textObj.GetComponent<TextMeshProUGUI>().text = ("Score " + GameData.totalScore.ToString());
                if ((GameData.paintMinigamesPlayed == 3) && (GameData.designerMinigamesPlayed == 0))
                {
                    Trivia.instance.startScreen.SetActive(false);
                    StartCoroutine(WaitASecondBreak2());
                }
                else if ((GameData.paintMinigamesPlayed == 3) && (GameData.designerMinigamesPlayed == 3))
                {
                    Trivia.instance.startScreen.SetActive(false);
                    StartCoroutine(WaitASecondBreak3());
                }
            }

            if (scene == "MazeMinigame")
            {
                Debug.Log("Load Maze Game");
                //Lobby.instance.globalLight.gameObject.SetActive(false);
                myCollider.isTrigger = false;
                paintScript.enabled = false;
                mazeScript.enabled = true;
                triviaScript.enabled = false;
                transform.localScale = new Vector3(4f, 4f, 4f);
                DGM.instance.StartTheNextGame();
            }
        }
    }

    IEnumerator WaitASecondBreak2()
    {
        yield return null;
        Trivia.instance.startScreen.SetActive(false);
        StartCoroutine(Trivia.instance.StartTriviaBreak2());
    }
    IEnumerator WaitASecondBreak3()
    {
        yield return null;
        Trivia.instance.startScreen.SetActive(false);
        StartCoroutine(Trivia.instance.StartTriviaBreak3());
    }
    IEnumerator WaitASecond()
    {
        yield return new WaitForSeconds(0.5f);
        Paint.instance.paintGameStarted.Value = true;
    }

    public void StartTimer(float duration, GameObject timerObj, string codeToRun)
    {
        Debug.Log("StartTimer");
        if (IsServer && IsOwner)
        {
            Debug.Log("instance in lobby: " + Lobby.instance.players.IndexOf(gameObject.GetComponent<NetworkObject>()) + "is owner: " + IsOwner);
            if (timerObj.name == "PaintManage")
            {
                Paint paintManagerScript = timerObj.GetComponent<Paint>();
                StartCoroutine(Timer(duration, timerObj, codeToRun, paintManagerScript, null, null));
            }
            else if (timerObj.name == "GameManager")
            {
                DGM mazeScript = timerObj.GetComponent<DGM>();
                StartCoroutine(Timer(duration, timerObj, codeToRun, null, mazeScript, null));
            }
            else if (timerObj.name == "Trivia")
            {
                Trivia triviaScript = timerObj.GetComponent<Trivia>();
                StartCoroutine(Timer(duration, timerObj, codeToRun, null, null, triviaScript));
            }
        }
    }

    IEnumerator Timer(float duration, GameObject timerObj, string codeToRun, Paint paintManagerScript, DGM mazeScript, Trivia triviaScript)
    {
        float startValue = 30f;
        float startTime;

        startTime = Time.time;

        while (Time.time - startTime < duration)
        {
            // Calculate the progress based on time
            if (timerObj.name == "PaintManage")
            {
                paintManagerScript.progress.Value = (Time.time - startTime) / duration;
                if (Paint.instance.finishedEarly)
                {
                    break;
                }
            }
            else if (timerObj.name == "GameManager")
            {
                mazeScript.progress.Value = (Time.time - startTime) / duration;
            }
            else if (timerObj.name == "Trivia")
            {
                triviaScript.progress.Value = (Time.time - startTime) / duration;
                if (Trivia.instance.answeredEarly)
                {
                    break;
                }
            }

            if ( (codeToRun == "EndADesignGame") && (!DGM.instance.gameStarted) )
            {
                break;
            }

            // Lerp between start and target values to smoothly move the slider
            //timeSlider.value = Mathf.Lerp(startValue, 0, progress);

            yield return null; // Wait for the next frame
        }

        if (codeToRun == "StartAPaintGame")
        {
            //paintManagerScript.StartAPaintGame();
            StartAPaintGameServerRpc();
        }
        else if (codeToRun == "EndAPaintGame")
        {
            //StartCoroutine(paintManagerScript.EndAPaintGame());
            StopAPaintGameServerRpc();
        }
        else if (codeToRun == "EndDesignPeriod")
        {
            EndDesignPeriodServerRpc();
        }
        else if (codeToRun == "EndADesignGame")
        {
            EndADesignGameServerRpc(false);
        }
        else if (codeToRun == "EndATriviaQuestion")
        {
            EndATriviaQuestionServerRpc();
        }

        // Ensure the slider reaches the exact target value
        //timeSlider.value = 0;
    }

    public void GenerateRandomNumber(int max)
    {
        if (IsServer && IsOwner)
        {
            GenerateRandomNumberServerRpc(max);
        }
    }
    [ServerRpc]
    private void GenerateRandomNumberServerRpc(int max)
    {
        int randomNumber = Random.Range(0, max);
    }
    [ClientRpc]
    private void GenerateRandomNumberClientRpc(int randomNumber)
    {
        Trivia.instance.randomAnswer = randomNumber;
    }
    public void EndPaintGame()
    {

    }
    public void EndMazeGame(bool early)
    {
        EndADesignGameServerRpc(early);
    }

    [ServerRpc]
    private void StartAPaintGameServerRpc()
    {
        StartAPaintGameClientRpc();
    }

    [ClientRpc]
    private void StartAPaintGameClientRpc()
    {
        Paint.instance.StartAPaintGame();
    }

    [ServerRpc]
    private void StopAPaintGameServerRpc()
    {
        StopAPaintGameClientRpc();
    }

    [ClientRpc]
    private void StopAPaintGameClientRpc()
    {
        StartCoroutine(Paint.instance.EndAPaintGame());
    }

    [ServerRpc]
    private void EndDesignPeriodServerRpc()
    {
        EndDesignPeriodClientRpc();
    }

    [ClientRpc]
    private void EndDesignPeriodClientRpc()
    {
        DGM.instance.EndDesignPeriod();
    }

    [ServerRpc]
    private void EndADesignGameServerRpc(bool early)
    {
        Slider slider = GameObject.Find("Slider").GetComponent<Slider>();
        float sliderValue = slider.value;
        EndADesignClientRpc(sliderValue, early);
    }

    [ClientRpc]
    private void EndADesignClientRpc(float sliderValue, bool early)
    {
        StartCoroutine(DGM.instance.EndADesignGame(sliderValue, early));
    }

    [ServerRpc]
    private void EndATriviaQuestionServerRpc()
    {
        EndATriviaQuestionClientRpc();
    }

    [ClientRpc]
    private void EndATriviaQuestionClientRpc()
    {
        StartCoroutine(Trivia.instance.EndATriviaQuestion());
    }

    public void UpdateChecks(int i, bool active, string team)
    {
        if (IsOwner)
        {
            UpdateChecksServerRpc(i, active, team);
        }
    }

    [ServerRpc]
    private void UpdateChecksServerRpc(int i, bool active, string team)
    {
        if (IsServer)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            if (players.Length == 3)
            {
                UpdateChecksClientRpc(i, active, team);
            }
        }
    }

    [ClientRpc]
    private void UpdateChecksClientRpc(int i, bool active, string team)
    {
        Lobby.instance.menuChecks[i].SetActive(active);
        if (team != "no")
        {
            GameData.team = team; 
        }
    }

    public void ResetGame()
    {
        if (IsOwner)
        {
            ResetGameServerRpc();
        }
    }
    [ServerRpc]
    private void ResetGameServerRpc()
    {
        if (IsServer)
        {
            ResetGameClientRpc();
        }
    }
    [ClientRpc]
    private void ResetGameClientRpc()
    {
        GameData.paintMinigamesPlayed = 0;
        GameData.paintGameScore = 0;
        GameData.mazeGameScore = 0;
        GameData.totalScore = 0;
        GameData.paintMinigamesPlayed = 0;
        GameData.designerMinigamesPlayed = 0;
        GameData.questionsAnswered = 0;
        GameData.team = "";
        Trivia.instance.startScreen.SetActive(true);
        Lobby.instance.playersConnectedText.gameObject.SetActive(false);
        if (Lobby.instance.version == "Client")
        {
            Debug.Log("client reset");
            Trivia.instance.joinAsClient.SetActive(true);
        }
        if (Lobby.instance.version == "Host")
        {
            Debug.Log("host reset");
            Trivia.instance.joinAsHost.SetActive(true);
        }
        if (IsServer)
        {
            NetworkManager.Shutdown();
        }
    }

}
