using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{

    public Text timerText;
    public Text eventText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HideEventMessage () {
        eventText.enabled = false;
    }

    public void ShowEventMessage (string message) {
        eventText.text = message;
        eventText.enabled = true;
    }

    public void TimerText (float seconds) {
        System.TimeSpan formattedTime = System.TimeSpan.FromSeconds(seconds);
        timerText.text = string.Format("{0:D2}:{1:D2}", formattedTime.Minutes, formattedTime.Seconds);
    }
}
