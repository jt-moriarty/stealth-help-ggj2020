using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public Door[] doors;
    public PushPullObject[] pushPullObjects;
    public KickObject[] kickObjects;


    public bool gameOver = false;
    public bool gameInProgress = false;

    public int numLitDoors = 0;

    public PlayerController player;
    public GameUIController uiController;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        uiController = GameObject.FindGameObjectWithTag("UIController").GetComponent<GameUIController>();
        doors = FindObjectsOfType<Door>();
        pushPullObjects = FindObjectsOfType<PushPullObject>();
        kickObjects = FindObjectsOfType<KickObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !gameInProgress) {
            Debug.Log("GAME START");
            StopAllCoroutines();
            StartCoroutine(StartGame(180f));
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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

    public void GameOver (bool win) {
        if (!gameOver) {
            Debug.Log("GAME OVER");
            gameOver = true;
            player.GameOver();
            uiController.ShowEventMessage(win ? "You Win!" : "Game Over!");
        }
    }

    IEnumerator StartGame (float gameLength) {
        player.enabled = true;
        gameOver = false;
        gameInProgress = true;
        uiController.HideEventMessage();
        //numLitDoors = 0;

        float nextDoorOpenTime = Random.Range(2.5f, 5f);
        float lastDoorOpenTime = Time.time;
        int nextDoor = Random.Range(0, doors.Length);
        float startTime = Time.time;
        while (!gameOver) {
            float timeRemaining = Mathf.Clamp(gameLength - (Time.time - startTime), 0f, gameLength);
            uiController.TimerText(timeRemaining);
            bool gameWin = true;
            for (int i = 0; i < pushPullObjects.Length; i++) {
                if (!pushPullObjects[i].inPlace) gameWin = false;
            }
            for (int i = 0; i < kickObjects.Length; i++) {
                if (!kickObjects[i].inPlace) gameWin = false;
            }

            if (gameWin) {
                GameOver(true);
            }
            else if (timeRemaining <= 0f) {
                GameOver(false);
            }
            // TODO: base this on partially player activity / noise level instead of movement.
            // No player movement = less likely.
            //numLitDoors = 0;
            /*for (int i = 0; i < doors.Length; i++) {
                if (doors[i].doorLit) {
                    numLitDoors++;
                }
            }
            if (Random.Range(0f, 1f) < (0.05f - (numLitDoors * 0.01f))) {
                nextDoor = Random.Range(0, doors.Length);
                if (!doors[nextDoor].doorLit) {
                    StartCoroutine(doors[nextDoor].StartLightUnderDoor(1f));
                    nextDoorWillOpen = Random.Range(0f, 1f) > 0.5f;
                }
                Debug.Log("DO LIGHT UNDER DOOR " + nextDoor);
            }
            if (numLitDoors > 0) {

            }*/
            //Debug.Log("Current: " + (Time.time - lastDoorOpenTime) + " nextDoorOpen: " + nextDoorOpenTime);
            if (Time.time - lastDoorOpenTime > nextDoorOpenTime) {
                bool fake = Random.Range(0f, 1f) > 0.75f;
                Debug.Log("NEXT DOOR IS " + nextDoor + " FAKE? " + fake);
                //underLightStartTime, fake, delayBeforeOpen, toOpenTime, stayOpenTime, toCloseTime, endUnderLightDelay, underLightEndTime
                StartCoroutine(doors[nextDoor].OpenSequence(1f, fake, Random.Range(5f, 10f), 1f, 1f, 1f, 1f, 1f));
                int currentDoor = nextDoor;
                while (nextDoor == currentDoor || doors[nextDoor].doorLit) {
                    Debug.Log("REASSIGN");
                    nextDoor = Random.Range(0, doors.Length);
                    yield return null;
                }
                Debug.Log("PAST LOOP");
                nextDoorOpenTime = Random.Range(5f, 10f);
                lastDoorOpenTime = Time.time;
            }

            yield return null;
        }
    }
}
