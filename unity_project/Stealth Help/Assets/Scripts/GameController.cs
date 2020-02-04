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

    public PlayerController player;
    public GameUIController uiController;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        uiController = GameObject.FindGameObjectWithTag("UIController").GetComponent<GameUIController>();
        doors = FindObjectsOfType<Door>();
        pushPullObjects = FindObjectsOfType<PushPullObject>();
        kickObjects = FindObjectsOfType<KickObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Grab") && !gameInProgress) {
            StopAllCoroutines();
            StartCoroutine(StartGame(180f));
        }
        if (gameOver && Input.GetKeyDown(KeyCode.R)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        if (gameOver && Input.GetKeyDown(KeyCode.Q)) {
            Application.Quit();
        }
    }

    public void GameOver (bool win) {
        if (!gameOver) {
            //Debug.Log("GAME OVER");
            gameOver = true;
            player.GameOver(win);
            uiController.ShowGameOver(win);
            StopAllCoroutines();
            /*for (int i = 0; i < doors.Length; i++) {
                doors[i].StopAllCoroutines();
            }*/
        }
    }

    IEnumerator StartGame (float gameLength) {
        player.enabled = true;
        gameOver = false;
        gameInProgress = true;
        uiController.HideIntro();

        float nextDoorOpenTime = Random.Range(2.5f, 5f);
        float lastDoorOpenTime = Time.time;
        int nextDoor = Random.Range(0, doors.Length);
        int currentDoor = -1;
        float startTime = Time.time;
        while (!gameOver) {
            float timeRemaining = Mathf.Clamp(gameLength - (Time.time - startTime), 0f, gameLength);
            uiController.TimerText(timeRemaining);
            bool gameWin = true;
            /*int remainingPushPull = 0;
            for (int i = 0; i < pushPullObjects.Length; i++) {
                if (!pushPullObjects[i].inPlace) {
                    gameWin = false;
                    remainingPushPull++;
                }
            }*/
            int remainingKick = 0;
            for (int i = 0; i < kickObjects.Length; i++) {
                if (!kickObjects[i].inPlace) {
                    gameWin = false;
                    remainingKick++;
                }
            }

            //Debug.Log("remainingPushPull " + remainingPushPull + " remainingKick " + remainingKick);

            if (gameWin) {
                GameOver(true);
            }
            else if (timeRemaining <= 0f) {
                GameOver(false);
            }
            // TODO: base this on partially player activity / noise level instead of movement.

            if (nextDoor == currentDoor || doors[nextDoor].doorLit) {
                nextDoor = Random.Range(0, doors.Length);
                nextDoorOpenTime = Random.Range(5f, 10f);
            }

            //Debug.Log("Current: " + (Time.time - lastDoorOpenTime) + " nextDoorOpen: " + nextDoorOpenTime);
            if (nextDoor != currentDoor && !doors[nextDoor].doorLit) {
                if (Time.time - lastDoorOpenTime > nextDoorOpenTime) {
                    bool fake = Random.Range(0f, 1f) > 0.75f;
                    Debug.Log("NEXT DOOR IS " + nextDoor + " FAKE? " + fake);
                    //underLightStartTime, fake, delayBeforeOpen, toOpenTime, stayOpenTime, toCloseTime, endUnderLightDelay, underLightEndTime
                    StartCoroutine(doors[nextDoor].OpenSequence(1f, fake, Random.Range(5f, 8f), 1f, 2f, 0.5f, 1f, 1f));
                    currentDoor = nextDoor;
                    lastDoorOpenTime = Time.time;
                }
            }

            yield return null;
        }
    }
}
