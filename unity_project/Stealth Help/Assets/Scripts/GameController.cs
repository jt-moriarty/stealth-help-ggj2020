using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public Door[] Doors;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U)) {
            Doors[0].Open();
        }
        if (Input.GetKeyDown(KeyCode.J)) {
            Doors[0].Close();
        }
        if (Input.GetKeyDown(KeyCode.H)) {

        }
        if (Input.GetKeyDown(KeyCode.K)) {

        }
    }
}
