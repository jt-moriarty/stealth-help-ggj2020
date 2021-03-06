﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Door : MonoBehaviour
{
    //TODO: Sweep cast for game over.

    public Animator animControl;
    public Light2D openLight;
    public Light2D closedLight;
    public PlayerController player;
    public GameController gameController;

    public bool doorLit = false;

    // Start is called before the first frame update
    void Start () {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    public void Open () {
        animControl.SetTrigger("Open");
        animControl.ResetTrigger("Close");
    }

    public void Close () {
        animControl.SetTrigger("Close");
        animControl.ResetTrigger("Open");
    }

    public IEnumerator StartLightUnderDoor (float tweenTime) {
        doorLit = true;
        float startTime = Time.time;
        while (Time.time - startTime < tweenTime) {
            float t = (Time.time - startTime) / tweenTime;
            closedLight.intensity = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }
    }

    public IEnumerator EndLightUnderDoor (float tweenTime) {
        float startTime = Time.time;
        while (Time.time - startTime < tweenTime) {
            float t = (Time.time - startTime) / tweenTime;
            closedLight.intensity = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }
        doorLit = false;
    }

    //START
    //0,0.275,0
    //0,0,-125
    //innerAngle 0
    //outerAngle 14
    //innerRadius 5
    //outerRadius 10

    //END
    //0,0.275,0
    //0,0,-180
    //innerAngle 127
    //outerAngle 141
    //innerRadius 5
    //outerRadius 10

    public IEnumerator OpenLight (float tweenTime) {
        float startTime = Time.time;
        while (Time.time - startTime < tweenTime) {
            float t = (Time.time - startTime) / tweenTime;
            openLight.transform.localRotation = Quaternion.Slerp(Quaternion.Euler(0, 0, -125), Quaternion.Euler(0,0,-180), t);
            openLight.pointLightInnerRadius = Mathf.Lerp(0f, 7.5f, t);
            openLight.pointLightOuterRadius = openLight.pointLightInnerRadius + Mathf.Lerp(0f, 2.5f, t);
            openLight.pointLightInnerAngle = Mathf.Lerp(0f, 127f, t);
            openLight.pointLightOuterAngle = openLight.pointLightInnerAngle + 14f;

            if (player.CheckSeenByLight(openLight)) {
                gameController.GameOver(false);
            }
            yield return null;
        }
    }

    public IEnumerator CloseLight (float tweenTime) {
        float startTime = Time.time;
        while (Time.time - startTime < tweenTime) {

            float t = (Time.time - startTime) / tweenTime;
            openLight.transform.localRotation = Quaternion.Slerp(Quaternion.Euler(0, 0, -180), Quaternion.Euler(0, 0, -125), t);
            openLight.pointLightInnerRadius = Mathf.Lerp(7.5f, 0f, t);
            openLight.pointLightOuterRadius = openLight.pointLightInnerRadius + Mathf.Lerp(2.5f, 0f, t);
            openLight.pointLightInnerAngle = Mathf.Lerp(127f, 0f, t);
            openLight.pointLightOuterAngle = openLight.pointLightInnerAngle + 14f;

            if (player.CheckSeenByLight(openLight)) {
                gameController.GameOver(false);
            }
            yield return null;
        }
    }

    public IEnumerator DoorOpenCheck (float openTime) {
        float startTime = Time.time;
        while (Time.time - startTime < openTime) {
            if (player.CheckSeenByLight(openLight)) {
                gameController.GameOver(false);
            }
            yield return null;
        }
    }

    public IEnumerator OpenSequence (float underLightStartTime, bool fake, float delayBeforeOpen, float toOpenTime, float stayOpenTime, float toCloseTime, float endUnderLightDelay, float underLightEndTime) {
        yield return StartCoroutine(StartLightUnderDoor(underLightStartTime)); // start the light under the door
        if (!fake) {
            yield return new WaitForSeconds(delayBeforeOpen); // wait a litte bit.
            Open(); // play open door animation.
            yield return StartCoroutine(OpenLight(toOpenTime)); // door light open.
            yield return StartCoroutine(DoorOpenCheck(stayOpenTime));
            Close(); // play close door animation.
            yield return StartCoroutine(CloseLight(toCloseTime)); // door light close.
        }
        yield return new WaitForSeconds(endUnderLightDelay); // delay before light under door goes away.
        yield return StartCoroutine(EndLightUnderDoor(underLightEndTime)); // end the light under the door.
    }
}
