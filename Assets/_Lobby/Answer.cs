using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Answer : MonoBehaviour
{
       public int answerInt;
       public int votes;
       public GameObject redSticker, blueSticker, yellowSticker;

       void Start()
       {
            Trivia.instance.answersList.Add(gameObject);
       }
}
