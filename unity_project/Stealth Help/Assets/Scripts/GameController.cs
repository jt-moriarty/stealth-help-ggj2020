using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public Door[] Doors;

    public bool gameOver = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U)) {
            StopAllCoroutines();
            StartCoroutine(StartGame());
        }
        /*if (Input.GetKeyDown(KeyCode.U)) {
            Doors[0].Open();
        }
        if (Input.GetKeyDown(KeyCode.J)) {
            Doors[0].Close();
        }
        if (Input.GetKeyDown(KeyCode.H)) {

        }
        if (Input.GetKeyDown(KeyCode.K)) {

        }*/
    }

    IEnumerator StartGame() {
        gameOver = false;
        float nextDoorOpen = Random.Range(5f, 20f);
        float lastDoorOpen = Time.time;
        int nextDoor = Random.Range(0, Doors.Length);
        float startTime = Time.time;
        while (!gameOver) {
            if (Time.time - lastDoorOpen > nextDoorOpen) {
                Doors[nextDoor].Open();
                yield return new WaitForSeconds(Random.Range(5f, 10f));
                Doors[nextDoor].Close();
                lastDoorOpen = Time.time;
                nextDoorOpen = Random.Range(5f, 20f);
                nextDoor = Random.Range(0, Doors.Length);
            }
            yield return null;
        }
    }
}
