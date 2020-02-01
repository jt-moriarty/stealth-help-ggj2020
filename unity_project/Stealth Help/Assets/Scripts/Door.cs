using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Door : MonoBehaviour
{
    public Animator animControl;
    public Light2D controlLight;

    // Start is called before the first frame update
    void Start () {
        
    }

    public void Open () {
        animControl.SetTrigger("Open");
        animControl.ResetTrigger("Close");
        StartCoroutine(OpenLight(2f));
    }

    public void Close () {
        animControl.SetTrigger("Close");
        animControl.ResetTrigger("Open");
        StartCoroutine(CloseLight(1f));
    }

    public IEnumerator OpenLight (float tweenTime) {
        float startTime = Time.time;
        while (Time.time - startTime < tweenTime) {
            float t = (Time.time - startTime) / tweenTime;
            controlLight.transform.localRotation = Quaternion.Slerp(Quaternion.Euler(0, 0, -1), Quaternion.Euler(0,0,-180), t);
            yield return null;
        }
    }

    public IEnumerator CloseLight (float tweenTime) {
        float startTime = Time.time;
        while (Time.time - startTime < tweenTime) {
            float t = (Time.time - startTime) / tweenTime;
            controlLight.transform.localRotation = Quaternion.Slerp(Quaternion.Euler(0, 0, -180), Quaternion.Euler(0, 0, -1), t);
            yield return null;
        }
    }
}
