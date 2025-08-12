using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject serverGO;
    public GameObject clientGO;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("GameManager Run");
        if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null)
        {
            Debug.Log("Server Build");
            Instantiate(serverGO);
        }
        else
        {
            Debug.Log("Client Build");
            Instantiate(clientGO);
        }
    }
}
