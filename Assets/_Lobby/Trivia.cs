using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class Trivia : NetworkBehaviour
{
    public static Trivia instance;
    public List<string> questions;
    public List<string> answers;
    public List<string> correctAnswers;
    public List<string> correctAnswersResponses;
    public List<string> wrongAnswersResponses;
    public string tiedAnswersResponse = "Looks like you guys couldn't agree on an answer! I... guess i'm gonna pick one of you at random, then. Good luck!";
    public GameObject answersObjects, resetButton, joinAsHost, joinAsClient, lockInButton;
    public List<GameObject> answersList;
    public string chosenAnswer;
    public int randomAnswer;
    public Slider timeSlider;
    float typingSpeed = 0.04f;
    bool doneTyping;
    public readonly NetworkVariable<float> progress = new(writePerm: NetworkVariableWritePermission.Owner);
    public GameObject explanationBoxObj;
    public TextMeshProUGUI explanationBox, scoreText, questionText, answer1Text, answer2Text, answer3Text, answer4Text;
    public GameObject startScreen;
    public GameObject background;
    public SpriteRenderer backgroundSR;
    public Sprite redNeutral, redSad, redHappy, redQuestioning;
    public Sprite blueNeutral, blueSad, blueHappy, blueQuestioning;
    public Sprite yellowNeutral, yellowSad, yellowHappy, yellowQuestioning;
    public bool selectable;
    public bool answeredEarly;

    void Start()
    {
        backgroundSR = background.GetComponent<SpriteRenderer>();
        lockInButton.SetActive(false);
        if (GameData.team == "Red")
        {
            backgroundSR.sprite = redNeutral;
        }
        if (GameData.team == "Blue")
        {
            backgroundSR.sprite = blueNeutral;
        }
        if (GameData.team == "Yellow")
        {
            backgroundSR.sprite = yellowNeutral;
        }
    }
    public IEnumerator StartIntro()
    {
        if (GameData.team == "Red")
        {
            backgroundSR.sprite = redNeutral;
        }
        if (GameData.team == "Blue")
        {
            backgroundSR.sprite = blueNeutral;
        }
        if (GameData.team == "Yellow")
        {
            backgroundSR.sprite = yellowNeutral;
        }

        Debug.Log("Team = " + GameData.team);
        startScreen.SetActive(false);
        scoreText.gameObject.SetActive(false);
        answersObjects.SetActive(false);
        Debug.Log("Start Intro");
        explanationBoxObj.SetActive(true);
        doneTyping = false;
        StartCoroutine(TypeText("Hello contestants, and welcome to the Hue & Co. Game Show!", typingSpeed, explanationBox));
        while (!doneTyping)
        {
            yield return null;
        }
        doneTyping = false;
        yield return new WaitForSeconds(2f);

        StartCoroutine(TypeText("I'm your host, and i'll be guiding you through today's set of minigames to make sure it all runs smoothly.", typingSpeed, explanationBox));
        while (!doneTyping)
        {
            yield return null;
        }
        doneTyping = false;
        yield return new WaitForSeconds(2f);

        StartCoroutine(TypeText("I hope you're all ready!", typingSpeed, explanationBox));
        while (!doneTyping)
        {
            yield return null;
        }
        doneTyping = false;
        yield return new WaitForSeconds(2f);

        StartCoroutine(TypeText("Before we begin, why don't we warm you up with some trivia questions to get your head in the game?", typingSpeed, explanationBox));
        while (!doneTyping)
        {
            yield return null;
        }
        doneTyping = false;
        yield return new WaitForSeconds(2f);;
        explanationBoxObj.SetActive(false);
        explanationBox.text = "";
        StartCoroutine(MakeAQuestion());
    }
    public IEnumerator StartTriviaBreak2()
    {
        UpdateReaction("Happy");
        answersObjects.SetActive(false);
        startScreen.SetActive(false);
        explanationBoxObj.SetActive(true);
        doneTyping = false;
        explanationBox.fontSize = 21;
        StartCoroutine(TypeText("Before moving on to our last minigame, we're gonna have a small intermission to ask you guys some trivia questions for extra points!", typingSpeed, explanationBox));
        while (!doneTyping)
        {
            yield return null;
        }
        doneTyping = false;
        yield return new WaitForSeconds(2f);
        explanationBox.text = "";
        explanationBox.fontSize = 25;
        UpdateReaction("Neutral");
        StartCoroutine(TypeText("You'll all select one of the four options, and whichever one has the most votes will be our answer.", typingSpeed, explanationBox));
        while (!doneTyping)
        {
            yield return null;
        }
        doneTyping = false;
        yield return new WaitForSeconds(2f);

        StartCoroutine(TypeText("Discuss with your teammates to come up with the right answer!", typingSpeed, explanationBox));
        while (!doneTyping)
        {
            yield return null;
        }
        doneTyping = false;
        yield return new WaitForSeconds(2f);
        explanationBoxObj.SetActive(false);
        explanationBox.text = "";
        StartCoroutine(MakeAQuestion());
    }

    public IEnumerator StartTriviaBreak3()
    {
        UpdateReaction("Happy");
        answersObjects.SetActive(false);
        startScreen.SetActive(false);
        explanationBoxObj.SetActive(true);
        doneTyping = false;
        StartCoroutine(TypeText("That's it for our minigames, everyone! But before you go, I have a final round of trivia for you all, so sit tight!", typingSpeed, explanationBox));
        while (!doneTyping)
        {
            yield return null;
        }
        doneTyping = false;
        yield return new WaitForSeconds(2f);
        explanationBoxObj.SetActive(false);
        explanationBox.text = "";
        StartCoroutine(MakeAQuestion());
    }

    public IEnumerator MakeAQuestion()
    {
        UpdateReaction("Neutral");
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            player.GetComponent<PlayerTrivia>().LockOut();
        }
        answeredEarly = false;
        answersObjects.SetActive(true);
        //Type the question
        doneTyping = false;
        Debug.Log(GameData.questionsAnswered);
        StartCoroutine(TypeText(questions[GameData.questionsAnswered], typingSpeed, questionText));
        while (!doneTyping)
        {
            yield return null;
        }
        doneTyping = false;
        UpdateReaction("Questioning");
        //Type the answer 1
        StartCoroutine(TypeText("A) " + answers[((GameData.questionsAnswered * 4) + 0)], typingSpeed, answer1Text));
        while (!doneTyping)
        {
            yield return null;
        }
        doneTyping = false;

        //Type the answer 2
        StartCoroutine(TypeText("B) " + answers[((GameData.questionsAnswered * 4) + 1)], typingSpeed, answer2Text));
        while (!doneTyping)
        {
            yield return null;
        }
        doneTyping = false;

        //Type the answer 3
        StartCoroutine(TypeText("C) " + answers[((GameData.questionsAnswered * 4) + 2)], typingSpeed, answer3Text));
        while (!doneTyping)
        {
            yield return null;
        }
        doneTyping = false;

        //Type the answer 4
        StartCoroutine(TypeText("D) " + answers[((GameData.questionsAnswered * 4) + 3)], typingSpeed, answer4Text));
        while (!doneTyping)
        {
            yield return null;
        }

        GameObject[] playerss = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in playerss)
        {
            LobbyPlayer lobbyScript = player.GetComponent<LobbyPlayer>();

            //supposed to be 15!!
            lobbyScript.StartTimer(15, gameObject, "EndATriviaQuestion");
            selectable = true;
            lockInButton.SetActive(true);
        }
    }

    public IEnumerator TypeText(string textToType, float typingSpeed, TextMeshProUGUI uiText)
    {
        string displayedText = "";

        foreach (char c in textToType)
        {
            displayedText += c;
            uiText.text = displayedText;
            yield return new WaitForSeconds(typingSpeed);
        }
        doneTyping = true;
        yield return null;
    }

    public IEnumerator EndATriviaQuestion()
    {
        lockInButton.SetActive(false);
        selectable = false;
        Debug.Log("EndQuestion");
        List<string> tieBreakerList = new List<string>(){};
        List<string> noneSelectedList = new List<string>(){};
        bool answerChosen = false;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject answer in answersList)
        {
            Answer answerScript = answer.GetComponent<Answer>();
            GameObject[] playerss = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in playerss)
            {
                PlayerTrivia playerScript = player.GetComponent<PlayerTrivia>();
                if (playerScript.playerAnswer == answer)
                {
                    answerScript.votes++;
                }
            }
            switch(answerScript.votes)
            {
                case 0:
                    noneSelectedList.Add(answers[((GameData.questionsAnswered * 4) + answerScript.answerInt) - 1]);
                    break;
                case 1:
                    tieBreakerList.Add(answers[((GameData.questionsAnswered * 4) + answerScript.answerInt) - 1]);
                    break;
                case 2:
                    chosenAnswer = answers[((GameData.questionsAnswered * 4) + answerScript.answerInt) - 1];
                    answerChosen = true;
                    break;
                case 3:
                    chosenAnswer = answers[((GameData.questionsAnswered * 4) + answerScript.answerInt) - 1];
                    answerChosen = true;
                    break;
            }
        }
        explanationBoxObj.SetActive(true);
        if ( (tieBreakerList.Count > 0) && (!answerChosen))
        {
            //GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                player.GetComponent<LobbyPlayer>().GenerateRandomNumber(3);
            }
            doneTyping = false;
            UpdateReaction("Neutral");
            StartCoroutine(TypeText("Looks like you guys couldn't agree on an answer! I... guess i'm gonna pick one of you at random, then. Good luck!", typingSpeed, explanationBox));
            while (!doneTyping)
            {
                yield return null;
            }
            doneTyping = false;
            yield return new WaitForSeconds(0.5f);
            chosenAnswer = tieBreakerList[randomAnswer];
            answerChosen = true;
        }
        else if ((noneSelectedList.Count > 0) && (!answerChosen))
        {
            //GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                player.GetComponent<LobbyPlayer>().GenerateRandomNumber(noneSelectedList.Count);
            }
            doneTyping = false;
            UpdateReaction("Neutral");
            StartCoroutine(TypeText("Looks like you guys couldn't agree on an answer! I... guess i'm gonna pick one of you at random, then. Good luck!", typingSpeed, explanationBox));
            while (!doneTyping)
            {
                yield return null;
            }
            doneTyping = false;
            yield return new WaitForSeconds(0.5f);
            chosenAnswer = noneSelectedList[randomAnswer];
            answerChosen = true;
        }

        Debug.Log("chosen answer = " + chosenAnswer);

        float points = 0;
        Lobby.instance.players[0].gameObject.GetComponent<LobbyPlayer>().GenerateRandomNumber(3);
        yield return new WaitForSeconds(0.5f);
        if (correctAnswers.Contains(chosenAnswer))
        {
            UpdateReaction("Happy");
            points = 375;
            explanationBoxObj.SetActive(true);
            doneTyping = false;
            explanationBox.text = "";
            StartCoroutine(TypeText(correctAnswers[GameData.questionsAnswered] + correctAnswersResponses[0], typingSpeed, explanationBox));
            while (!doneTyping)
            {
                yield return null;
            }
            doneTyping = false;
            yield return new WaitForSeconds(0.5f);
            explanationBoxObj.SetActive(false);
            explanationBox.text = "";
        }
        else
        {
            explanationBoxObj.SetActive(true);
            doneTyping = false;
            string wrongAnswerResponse = "";
            switch(1)
            {
                case 1:
                    wrongAnswerResponse = ("Ooh, that is unfortunately incorrect. The right answer was " + correctAnswers[GameData.questionsAnswered] + ". Moving on:");
                    break;
                // case 2:
                //     wrongAnswerResponse = ("Wrong! The answer I was looking for was" + correctAnswers[GameData.questionsAnswered] + ". Shrug it off, moving on:");
                //     break;
                // case 3:
                //     wrongAnswerResponse = ("Nope! The correct answer was actually" + correctAnswers[GameData.questionsAnswered] + ".");
                //     break;
            }
            UpdateReaction("Sad");
            StartCoroutine(TypeText(wrongAnswerResponse, typingSpeed, explanationBox));
            while (!doneTyping)
            {
                yield return null;
            }
            doneTyping = false;
            yield return new WaitForSeconds(0.5f);
            explanationBoxObj.SetActive(false);
            explanationBox.text = "";
        }

        float targetNumber = points + GameData.totalScore;
        float currentValue = GameData.totalScore;
        GameData.totalScore += points;

        scoreText.gameObject.SetActive(true);

        if (points == 0)
        {
            scoreText.text = GameData.totalScore.ToString();
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

        GameData.questionsAnswered++;
        chosenAnswer = null;
        GameObject[] players2 = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players2)
        {
            PlayerTrivia playerScript = player.GetComponent<PlayerTrivia>();
            playerScript.ResetAnswers();
        }
        questionText.text = "";
        answer1Text.text = "";
        answer2Text.text = "";
        answer3Text.text = "";
        answer4Text.text = "";
        explanationBoxObj.SetActive(false);
        yield return new WaitForSeconds(1f);
        if ( (GameData.questionsAnswered != 3) && (GameData.questionsAnswered != 6) && (GameData.questionsAnswered != 9))
        {
            StartCoroutine(Trivia.instance.MakeAQuestion());
        }
        //SUPPOSED TO BE 3
        if (GameData.questionsAnswered == 3)
        {
            UpdateReaction("Neutral");
            answersObjects.SetActive(false);
            explanationBoxObj.SetActive(true);
            doneTyping = false;
            StartCoroutine(TypeText("Welcome, contestants, to your first minigame!", typingSpeed, explanationBox));
            while (!doneTyping)
            {
                yield return null;
            }
            doneTyping = false;
            yield return new WaitForSeconds(2f);

            StartCoroutine(TypeText("You're gonna be coloring three drawings, getting points for speed and accuracy.", typingSpeed, explanationBox));
            while (!doneTyping)
            {
                yield return null;
            }
            doneTyping = false;
            yield return new WaitForSeconds(2f);

            StartCoroutine(TypeText("You can left click to add your color, and right click to erase.", typingSpeed, explanationBox));
            while (!doneTyping)
            {
                yield return null;
            }
            doneTyping = false;
            yield return new WaitForSeconds(2f);

            StartCoroutine(TypeText("Here's the twist: You've only got 10 seconds to memorize a drawing before the 30-second coloring phase begins.", typingSpeed, explanationBox));
            while (!doneTyping)
            {
                yield return null;
            }
            doneTyping = false;
            yield return new WaitForSeconds(2f);

            StartCoroutine(TypeText("Ready? Go!", typingSpeed, explanationBox));
            while (!doneTyping)
            {
                yield return null;
            }
            doneTyping = false;
            yield return new WaitForSeconds(1f);;
            explanationBoxObj.SetActive(false);

            //RUN THE PAINT GAME
            if (IsServer)
            {
                yield return new WaitForSeconds(1f);
                string m_SceneName = "PaintMinigame";
                var status = NetworkManager.SceneManager.LoadScene(m_SceneName, LoadSceneMode.Single);
                if (status != SceneEventProgressStatus.Started)
                {
                    Debug.LogWarning($"Failed to load {m_SceneName} " +
                            $"with a {nameof(SceneEventProgressStatus)}: {status}");
                }
            }
        }
        //SUPPOSED TO BE 6
        if (GameData.questionsAnswered == 6)
        {
            UpdateReaction("Neutral");
            answersObjects.SetActive(false);
            //RUN THE MAZE GAME
            explanationBoxObj.SetActive(true);
            doneTyping = false;
            StartCoroutine(TypeText("I think it's time for our final minigame, folks!", typingSpeed, explanationBox));
            while (!doneTyping)
            {
                yield return null;
            }
            doneTyping = false;
            yield return new WaitForSeconds(2f);

            StartCoroutine(TypeText("In this one, you'll all work together to escape a maze.", typingSpeed, explanationBox));
            while (!doneTyping)
            {
                yield return null;
            }
            doneTyping = false;
            yield return new WaitForSeconds(2f);

            StartCoroutine(TypeText("One of you, the overseer, needs to help guide the other players to their respective exit.", typingSpeed, explanationBox));
            while (!doneTyping)
            {
                yield return null;
            }
            doneTyping = false;
            yield return new WaitForSeconds(2f);

            StartCoroutine(TypeText("The other players will then have to find their exits with the overseer's help.", typingSpeed, explanationBox));
            while (!doneTyping)
            {
                yield return null;
            }
            doneTyping = false;
            yield return new WaitForSeconds(2f);

            StartCoroutine(TypeText("Keep in mind: The color of the player needs to match the color of the exit.", typingSpeed, explanationBox));
            while (!doneTyping)
            {
                yield return null;
            }
            doneTyping = false;
            yield return new WaitForSeconds(2f);

            StartCoroutine(TypeText("And oh yeah, use WASD or the arrow keys to move!", typingSpeed, explanationBox));
            while (!doneTyping)
            {
                yield return null;
            }
            doneTyping = false;
            yield return new WaitForSeconds(2f);
            explanationBoxObj.SetActive(false);

            //RUN THE PAINT GAME
            if (IsServer)
            {
                yield return new WaitForSeconds(1f);
                string m_SceneName = "MazeMinigame";
                var status = NetworkManager.SceneManager.LoadScene(m_SceneName, LoadSceneMode.Single);
                if (status != SceneEventProgressStatus.Started)
                {
                    Debug.LogWarning($"Failed to load {m_SceneName} " +
                            $"with a {nameof(SceneEventProgressStatus)}: {status}");
                }
            }
        }

        if (GameData.questionsAnswered == 9)
        {
            UpdateReaction("Neutral");
            answersObjects.SetActive(false);
            //RUN THE OUTRO 
            explanationBoxObj.SetActive(true);
            doneTyping = false;
            explanationBox.fontSize = 21;
            StartCoroutine(TypeText("And that about wraps up our gameshow! I'm now gonna total up the amount of points you all recieved, which will contribute to your color team's total points.", typingSpeed, explanationBox));
            while (!doneTyping)
            {
                yield return null;
            }
            doneTyping = false;
            yield return new WaitForSeconds(2f);
            explanationBox.text = "";
            explanationBox.fontSize = 25;
            //scoreText.rectTransform.position = new Vector3(0, 0, 0);
            scoreText.fontSize = 100;
            explanationBoxObj.SetActive(true);
            UpdateReaction("Happy");
            doneTyping = false;
            StartCoroutine(TypeText("Let's see here... looks like you guys earned... " + GameData.totalScore + " Points! Not bad!", typingSpeed, explanationBox));
            while (!doneTyping)
            {
                yield return null;
            }
            doneTyping = false;
            yield return new WaitForSeconds(2f);

            StartCoroutine(TypeText("Thank you for dropping by!", typingSpeed, explanationBox));
            while (!doneTyping)
            {
                yield return null;
            }
            doneTyping = false;
            yield return new WaitForSeconds(2f);
            explanationBoxObj.SetActive(false);
            resetButton.SetActive(true);
        }
        yield return null;
    }

    public void UpdateReaction(string reaction)
    {
        if (GameData.team == "Red")
        {
            if (reaction == "Happy")
            {
                backgroundSR.sprite = redHappy;
            }
            if (reaction == "Neutral")
            {
                backgroundSR.sprite = redNeutral;
            }
            if (reaction == "Questioning")
            {
                backgroundSR.sprite = redQuestioning;
            }
            if (reaction == "Sad")
            {
                backgroundSR.sprite = redSad;
            }
        }
        if (GameData.team == "Blue")
        {
            if (reaction == "Happy")
            {
                backgroundSR.sprite = blueHappy;
            }
            if (reaction == "Neutral")
            {   
                backgroundSR.sprite = blueNeutral;
            }
            if (reaction == "Questioning")
            {
                backgroundSR.sprite = blueQuestioning;
            }
            if (reaction == "Sad")
            {
                backgroundSR.sprite = blueSad;
            
            }
        }
        if (GameData.team == "Yellow")
        {
            if (reaction == "Happy")
            {
                backgroundSR.sprite = yellowHappy;
            }
            if (reaction == "Neutral")
            {
                backgroundSR.sprite = yellowNeutral;
            }
            if (reaction == "Questioning")
            {   
                backgroundSR.sprite = yellowQuestioning;
            }
            if (reaction == "Sad")
            {
                backgroundSR.sprite = yellowSad;
            }
        }
    }

    private void UpdateSlider(float previous, float current)
    {
        timeSlider.value = Mathf.Lerp(30, 0, current);
    }

    void Awake()
    {
        //DontDestroyOnLoad(gameObject);
        instance = this;
        questions = new List<string>(){};
        answersList = new List<GameObject>(){};
        correctAnswers = new List<string>(){};
        correctAnswersResponses = new List<string>(){};
        wrongAnswersResponses = new List<string>(){};
        answers = new List<string>(){};
        progress.OnValueChanged += UpdateSlider;

        //First Trivia Round
        questions.Add("Who is the original creator of Super Mario Bros.?");
        questions.Add("What city does 'Grand Theft Auto 5' take place in?");
        questions.Add("Which of these Pokemon is NOT a starter?");

        //Second Trivia Round
        questions.Add("Which of the following characters is NOT a playable fighter in the original 'Super Smash Bros.' game for the Nintendo 64?");
        questions.Add("What's the most sold game of 2023?");
        questions.Add("Which of these isn't a Wii Sports Resort game?");

        //Third Trivia Round
        questions.Add("Who won Game Of The Year in 2022?");
        questions.Add("How many stars are there to collect in Super Mario 64?");
        questions.Add("In Minecraft, which is of the following ores is the rarest?");

        //Round 1 Question 1
        answers.Add("Satoru Iwata");
        answers.Add("Shigeru Miyamoto");
        answers.Add("Masahiro Sakurai");
        answers.Add("Hidetaka Miyazaki");

        //Round 1 Question 2
        answers.Add("Liberty City");
        answers.Add("Vice City");
        answers.Add("Los Santos");
        answers.Add("San Andreas");

        //Round 1 Question 3
        answers.Add("Eevee");
        answers.Add("Cyndaquil");
        answers.Add("Sobble");
        answers.Add("Chikorita");

        //Round 2 Question 1
        answers.Add("Pikachu");
        answers.Add("Jigglypuff");
        answers.Add("Captain Falcon");
        answers.Add("Bowser");

        //Round 2 Question 2
        answers.Add("Minecraft");
        answers.Add("Hogwarts Legacy");
        answers.Add("Spider Man 2");
        answers.Add("Baldur's Gate 3");

        //Round 2 Question 3
        answers.Add("Frisbee");
        answers.Add("Archery");
        answers.Add("Canoeing");
        answers.Add("Volleyball");

        //Round 3 Question 1
        answers.Add("God of War");
        answers.Add("Elden Ring");
        answers.Add("Horizon Forbidden West");
        answers.Add("A Plague Tale: Requiem");

        //Round 3 Question 2
        answers.Add("100");
        answers.Add("99");
        answers.Add("120");
        answers.Add("111");

        //Round 3 Question 3
        answers.Add("Emerald Ore");
        answers.Add("Redstone Ore");
        answers.Add("Diamond Ore");
        answers.Add("Ancient Debris");

        //Correct Answers Round 1
        correctAnswers.Add("Shigeru Miyamoto");
        correctAnswers.Add("Los Santos");
        correctAnswers.Add("Eevee");

        //Correct Answers Round 2
        correctAnswers.Add("Bowser");
        correctAnswers.Add("Hogwarts Legacy");
        correctAnswers.Add("Volleyball");

        //Correct Answers Round 3
        correctAnswers.Add("Elden Ring");
        correctAnswers.Add("120");
        correctAnswers.Add("Emerald Ore");

        correctAnswersResponses.Add(" is correct! Next up:");
        //correctAnswersResponses.Add("Great job, that's right! Next up:");
        //correctAnswersResponses.Add("Absolutely correct! Next up:");
    }
}
