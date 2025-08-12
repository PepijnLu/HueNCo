using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using UnityEngine.Networking;
using TMPro;

public class Lobby : NetworkBehaviour
{
    public static Lobby instance;
    public List<NetworkObject> players;
    public List<GameObject> menuChecks;
    public UnityEngine.Rendering.Universal.Light2D globalLight;
    public Sprite blueCursor, yellowCursor, redCursor;
    public Sprite redDesigner, blueDesigner, yellowDesigner;
    public GameObject startScreen;
    public GameObject hostButton, clientButton, startButton;
    public GameObject chooseTeamColor;
    public string version;
    public TextMeshProUGUI playersConnectedText;
    // Start is called before the first frame update
    void Awake()
    {
        if (GameData.paintMinigamesPlayed > 0)
        {
            Destroy(gameObject);
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        players = new List<NetworkObject>() {} ;
    }
    void Start()
    {
        #if !UNITY_EDITOR
        Debug.unityLogger.logEnabled = false;
        #endif
        // Check if Allow Remote Connections is enabled
        if (version == "Host")
        {
            clientButton.SetActive(false);
        }
        
        else if (version == "Client")
        {
            clientButton.SetActive(true);
            hostButton.SetActive(false);
            startButton.SetActive(false);
        }
        //chooseTeamColor.SetActive(false);
        playersConnectedText.gameObject.SetActive(false);
    }

    public override void OnNetworkSpawn()
    {
        //StartCoroutine(IntroSequence());
        SceneManager.sceneLoaded += OnSceneStart;
    }

    public void StartGameShow()
    {
        //if(players.Count == 3)
        //{
            //StartCoroutine(IntroSequence());
            // StartCoroutine(Trivia.instance.MakeAQuestion());
        //}
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnSceneStart(Scene scene, LoadSceneMode mode)
    {
        instance = this;
    }

    IEnumerator IntroSequence()
    {
        if (IsServer)
        {
            Debug.Log("Start Intro");
            yield return new WaitForSeconds(5f);
            //string m_SceneName = "PaintMinigame";
            string m_SceneName = "MazeMinigame";
            var status = NetworkManager.SceneManager.LoadScene(m_SceneName, LoadSceneMode.Single);
            if (status != SceneEventProgressStatus.Started)
            {
                Debug.LogWarning($"Failed to load {m_SceneName} " +
                        $"with a {nameof(SceneEventProgressStatus)}: {status}");
            }
        }
    }
}
