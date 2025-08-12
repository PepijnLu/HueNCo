using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;
using UnityEngine.SceneManagement;

public class Paint : NetworkBehaviour
{
    public readonly NetworkVariable<bool> paintGameStarted = new(writePerm: NetworkVariableWritePermission.Owner);
    //public readonly NetworkVariable<int> startValue = new(writePerm: NetworkVariableWritePermission.Owner);
    public readonly NetworkVariable<float> progress = new(writePerm: NetworkVariableWritePermission.Owner);
    // private readonly NetworkVariable<int> progress = new(writePerm: NetworkVariableWritePermission.Owner);
    public Dictionary<string, Color> paintColors = new Dictionary<string, Color>();
    public List<GameObject> objectsToPaint;
    public GameObject flower, rainbow, controller;
    public bool gameStarted;
    public static Paint instance;
    public Slider timeSlider;
    public int timeRemaining;
    public float totalObjects, correctObjects, points;
    public TextMeshProUGUI scoreText;
    public bool ableToPaint;
    public int paintTimer;
    public bool finishedEarly;
    //public static Paint instance;

    void Awake()
    {
        instance = this;

        objectsToPaint = new List<GameObject>(){};
        paintColors["white"] = new Color((247f / 255f), (224f / 255f), (180f / 255f));
        paintColors["blue"] = new Color((23f / 255f), (82f / 255f), (138f / 255f));
        paintColors["red"] = new Color((237f / 255f), (64f / 255f), (34f / 255f));
        paintColors["yellow"] = new Color((253f / 255f), (168f / 255f), (52f / 255f));
        paintColors["purple"] = new Color((162f / 255f), (31f / 255f), (81f / 255f));
        paintColors["orange"] = new Color((253f / 255f), (113f / 255f), (12f / 255f));
        paintColors["green"] = new Color((24f / 255f), (138f / 255f), (138f / 255f));
        Debug.Log(paintColors);
    }
    void Start()
    {
        
    }

    public override void OnNetworkSpawn()
    {
        paintGameStarted.OnValueChanged += StartTheNextGame;
        progress.OnValueChanged += UpdateSlider;
        flower.SetActive(false);
        controller.SetActive(false);

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            LobbyPlayer lobbyScript = player.GetComponent<LobbyPlayer>();
        }

        instance = this;
        //StartCoroutine(StartTheFlowerGame());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartTheNextGame(bool previous, bool current)
    {
        finishedEarly = false;
        if (GameData.paintMinigamesPlayed == 0)
        {
            instance = this;
            timeSlider.gameObject.SetActive(true);
            controller.SetActive(false);
            rainbow.SetActive(true);
            flower.SetActive(false);   
        }
        else if (GameData.paintMinigamesPlayed == 1)
        {
            timeSlider.gameObject.SetActive(true);
            controller.SetActive(false);
            rainbow.SetActive(false);
            flower.SetActive(true); 
        }
        else if (GameData.paintMinigamesPlayed == 2)
        {
            timeSlider.gameObject.SetActive(true);
            controller.SetActive(true);
            rainbow.SetActive(false);
            flower.SetActive(false); 
        }
        else if (GameData.paintMinigamesPlayed == 3)
        {
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

        if (GameData.paintMinigamesPlayed < 3)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                LobbyPlayer lobbyScript = player.GetComponent<LobbyPlayer>();
                lobbyScript.StartTimer(9, gameObject, "StartAPaintGame");
            }
        }
    }

    public void StartAPaintGame()
    {
        GameObject[] paintables = GameObject.FindGameObjectsWithTag("Paintable");
        foreach (GameObject paintable in paintables)
        {
            Paintable paintableScript = paintable.GetComponent<Paintable>();
            paintableScript.color = paintColors["white"];
            paintable.GetComponent<SpriteRenderer>().color = paintableScript.color;
        }
        gameStarted = true;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            LobbyPlayer lobbyScript = player.GetComponent<LobbyPlayer>();

            //supposed to be 30!
            lobbyScript.StartTimer(30, gameObject, "EndAPaintGame");
            ableToPaint = true;
        }

        //StartCoroutine(GameTimer());
    }

    public IEnumerator EndAPaintGame()
    {
        ableToPaint = false;
        GameObject[] paintables = GameObject.FindGameObjectsWithTag("Paintable");
        foreach (GameObject paintable in paintables)
        {
            totalObjects++;
            Paintable paintableScript = paintable.GetComponent<Paintable>();
            // float color1 = paintableScript.correctColor.g;
            // float color2 = paintableScript.color.g;
            // Debug.Log(paintable.name + " " + paintableScript.correctColor.g + " " + paintableScript.color.g);
            bool correct = paintableScript.IsCorrectColor();
            if (correct)
            {
                correctObjects++;
            }
        }
        float percentageTimeRemaining = (timeSlider.value / 30);
        int points1 = Mathf.FloorToInt((correctObjects / totalObjects) * 1000);
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
                scoreText.text = (currentValue1.ToString() + " - Drawing Points" + System.Environment.NewLine + currentValue2.ToString() + " - Time Remaining Bonus");
                

                // Break out of the loop if currentValue reaches targetNumber
                if ( (currentValue1 >= targetNumber1) && (currentValue2 >= targetNumber2))
                {
                    break;
                }
                yield return null;
            }
        }

        yield return new WaitForSeconds(5f);
        if (scoreText.gameObject != null)
        {
            scoreText.gameObject.SetActive(false);
        }
        GameData.paintMinigamesPlayed++;
        gameStarted = false;
        //StartCoroutine(StartTheRainbowGame());
        correctObjects = 0;
        totalObjects = 0;
        points = 0;

        StartTheNextGame(false, true);
    }


    public IEnumerator StartTheFlowerGame()
    {
        timeSlider.gameObject.SetActive(true);
        controller.SetActive(false);
        rainbow.SetActive(false);
        flower.SetActive(true);

        float duration = 9f;
        float startValue = 30f;
        float startTime;

        startTime = Time.time;

        while (Time.time - startTime < duration)
        {
            // Calculate the progress based on time
            float progress = (Time.time - startTime) / duration;

            // Lerp between start and target values to smoothly move the slider
            timeSlider.value = Mathf.Lerp(startValue, 0, progress);

            yield return null; // Wait for the next frame
        }

        // Ensure the slider reaches the exact target value
        timeSlider.value = 0;

        GameObject[] paintables = GameObject.FindGameObjectsWithTag("Paintable");
        foreach (GameObject paintable in paintables)
        {
            Paintable paintableScript = paintable.GetComponent<Paintable>();
            paintableScript.color = paintColors["white"];
            paintable.GetComponent<SpriteRenderer>().color = paintableScript.color;
        }
        gameStarted = true;
        StartCoroutine(GameTimer());
    }

    public IEnumerator GameTimer()
    {
        timeSlider.gameObject.SetActive(true);

        float duration = 30f;
        float startValue = 30f;
        float startTime;

        startTime = Time.time;
        while (Time.time - startTime < duration)
        {
            // Calculate the progress based on time
            float progress = (Time.time - startTime) / duration;

            // Lerp between start and target values to smoothly move the slider
            timeSlider.value = Mathf.Lerp(startValue, 0, progress);

            yield return null; // Wait for the next frame
        }

        // Ensure the slider reaches the exact target value
        timeSlider.value = 0;
        
        GameObject[] paintables = GameObject.FindGameObjectsWithTag("Paintable");
        foreach (GameObject paintable in paintables)
        {
            totalObjects++;
            Paintable paintableScript = paintable.GetComponent<Paintable>();
            // float color1 = paintableScript.correctColor.g;
            // float color2 = paintableScript.color.g;
            // Debug.Log(paintable.name + " " + paintableScript.correctColor.g + " " + paintableScript.color.g);
            bool correct = paintableScript.IsCorrectColor();
            if (correct)
            {
                correctObjects++;
            }
        }

        points = Mathf.RoundToInt((correctObjects / totalObjects) * 1000);
        GameData.totalScore += points;

        float targetNumber = points;
        float currentValue = 0;
        scoreText.gameObject.SetActive(true);
        if (targetNumber == 0)
        {
            scoreText.text = "0";
        }
        else
        {
            while (currentValue < targetNumber)
            {
                // Calculate the amount to add based on speed and deltaTime
                int increment = 1;

                // Make sure not to overshoot the target
                currentValue = Mathf.Min(currentValue + increment, targetNumber);
                scoreText.text = currentValue.ToString();

                // Break out of the loop if currentValue reaches targetNumber
                if (currentValue >= targetNumber)
                {
                    break;
                }
                yield return null;
            }
        }

        yield return new WaitForSeconds(5f);
        scoreText.gameObject.SetActive(false);
        GameData.paintMinigamesPlayed++;
        gameStarted = false;
        //StartCoroutine(StartTheRainbowGame());
        correctObjects = 0;
        totalObjects = 0;
        points = 0;

        StartTheNextGame(false, true);
        yield return null;
    }

    private void UpdateSlider(float previous, float current)
    {
        timeSlider.value = Mathf.Lerp(30, 0, current);
    }
    public IEnumerator StartTheRainbowGame()
    {
        flower.SetActive(false);
        controller.SetActive(false);
        rainbow.SetActive(true);
        timeSlider.gameObject.SetActive(true);
        float duration = 5f;
        float startValue = 30f;
        float startTime;

        startTime = Time.time;

        while (Time.time - startTime < duration)
        {
            // Calculate the progress based on time
            float progress = (Time.time - startTime) / duration;

            // Lerp between start and target values to smoothly move the slider
            timeSlider.value = Mathf.Lerp(startValue, 0, progress);

            yield return null; // Wait for the next frame
        }

        // Ensure the slider reaches the exact target value
        timeSlider.value = 0;

        GameObject[] paintables = GameObject.FindGameObjectsWithTag("Paintable");
        foreach (GameObject paintable in paintables)
        {
            paintable.GetComponent<Paintable>().color = paintColors["white"];
            paintable.GetComponent<SpriteRenderer>().color = paintable.GetComponent<Paintable>().color;
        }
        gameStarted = true;
        StartCoroutine(GameTimer());
    }

    public IEnumerator StartTheControllerGame()
    {
        flower.SetActive(false);
        rainbow.SetActive(false);
        controller.SetActive(true);
        timeSlider.gameObject.SetActive(true);
        float duration = 5f;
        float startValue = 30f;
        float startTime;

        startTime = Time.time;

        while (Time.time - startTime < duration)
        {
            // Calculate the progress based on time
            float progress = (Time.time - startTime) / duration;

            // Lerp between start and target values to smoothly move the slider
            timeSlider.value = Mathf.Lerp(startValue, 0, progress);

            yield return null; // Wait for the next frame
        }

        // Ensure the slider reaches the exact target value
        timeSlider.value = 0;

        GameObject[] paintables = GameObject.FindGameObjectsWithTag("Paintable");
        foreach (GameObject paintable in paintables)
        {
            paintable.GetComponent<Paintable>().color = paintColors["white"];
            paintable.GetComponent<SpriteRenderer>().color = paintable.GetComponent<Paintable>().color;
        }
        gameStarted = true;
        StartCoroutine(GameTimer());
    }

    public void CheckIfComplete()
    {
        GameObject[] paintables = GameObject.FindGameObjectsWithTag("Paintable");
        int totalObjects = 0;
        int correctObjects = 0;
        foreach (GameObject paintable in paintables)
        {
            totalObjects++;
            Paintable paintableScript = paintable.GetComponent<Paintable>();
            // float color1 = paintableScript.correctColor.g;
            // float color2 = paintableScript.color.g;
            // Debug.Log(paintable.name + " " + paintableScript.correctColor.g + " " + paintableScript.color.g);
            bool correct = paintableScript.IsCorrectColor();
            if (correct)
            {
                correctObjects++;
            }
        }
        if (correctObjects == totalObjects)
        {
            finishedEarly = true;
        }
    }
}
