using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject playerCamera;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, -27, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
