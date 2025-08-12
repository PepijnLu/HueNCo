using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class PlayerTrivia : NetworkBehaviour
{
    List<GameObject> questionReferences;
    public GameObject playerAnswer;
    public readonly NetworkVariable<bool> lockedIn = new(writePerm: NetworkVariableWritePermission.Owner);
    // Start is called before the first frame update
    void Start()
    {
        CustomStart();
    }

    void CustomStart()
    {
        questionReferences = new List<GameObject>() {};
        lockedIn.OnValueChanged += CheckIfLockedIn;
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
                if (questionReferences.Count > 0) 
                {
                    playerAnswer = questionReferences[0];
                    if ((IsOwner) && (Trivia.instance.selectable) && (lockedIn.Value == false))
                    {
                        SentAnswerToServerRpc(gameObject.GetComponent<LobbyPlayer>().playerInt, (playerAnswer.GetComponent<Answer>().answerInt - 1));
                    }
                }
            }

            if (Input.GetMouseButtonUp(1))
            {
                if (questionReferences.Count > 0) 
                {
                    playerAnswer = null;
                }
            }
        }
    }

    public void LockIn()
    {
        if ((IsOwner) && (playerAnswer != null))
        {
            lockedIn.Value = true;
            Trivia.instance.lockInButton.SetActive(false);
        }
    }
    public void LockOut()
    {
        if (IsOwner)
        {
            lockedIn.Value = false;
        }
    }
    private void CheckIfLockedIn(bool previous, bool current)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        int playersLockedIn = 0;
        foreach(GameObject player in players)
        {
            if (player.GetComponent<PlayerTrivia>().lockedIn.Value == true)
            {
                playersLockedIn++;
            }
        }
        if (playersLockedIn == 3)
        {
            Trivia.instance.answeredEarly = true;
            Debug.Log("Answered Early");
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Answer")
        {
            questionReferences.Insert(0, collider.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Answer")
        {
            questionReferences.Remove(collider.gameObject);
        }
    }

    public void ResetAnswers()
    {
        if (IsServer && IsOwner)
        {
            SentAnswerToServerRpc(5, 0);
        }
    }

    [ServerRpc]
    private void SentAnswerToServerRpc(int playerIndex, int answerIndex)
    {
        SentAnswerToClientRpc(playerIndex, answerIndex);
    }

    [ClientRpc]
    private void SentAnswerToClientRpc(int playerIndex, int answerIndex)
    {
        if (playerIndex != 5)
        {
            Lobby.instance.players[playerIndex - 1].gameObject.GetComponent<PlayerTrivia>().playerAnswer = Trivia.instance.answersList[answerIndex];
        }
        switch (playerIndex)
        {
            case 1:
                foreach (GameObject obj in Trivia.instance.answersList)
                {
                    Answer answerScript = obj.GetComponent<Answer>();
                    answerScript.redSticker.SetActive(false);
                }
                Trivia.instance.answersList[answerIndex].GetComponent<Answer>().redSticker.SetActive(true);
                break;
            case 2:
                foreach (GameObject obj in Trivia.instance.answersList)
                {
                    Answer answerScript = obj.GetComponent<Answer>();
                    answerScript.blueSticker.SetActive(false);
                }
                Trivia.instance.answersList[answerIndex].GetComponent<Answer>().blueSticker.SetActive(true);
                break;
            case 3:
                foreach (GameObject obj in Trivia.instance.answersList)
                {
                    Answer answerScript = obj.GetComponent<Answer>();
                    answerScript.yellowSticker.SetActive(false);
                }
                Trivia.instance.answersList[answerIndex].GetComponent<Answer>().yellowSticker.SetActive(true);
                break;
            case 5:
                foreach (GameObject obj in Trivia.instance.answersList)
                {
                    Answer answerScript = obj.GetComponent<Answer>();
                    answerScript.blueSticker.SetActive(false);
                    answerScript.yellowSticker.SetActive(false);
                    answerScript.redSticker.SetActive(false);
                    answerScript.votes = 0;
                }
                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                foreach (GameObject player in players)
                {
                    player.GetComponent<PlayerTrivia>().playerAnswer = null;
                }
                break;
        }
    }
}
