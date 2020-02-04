using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    public Image timerBG;
    public Text timerText;

    public Image introBG;
    public Text introText;

    public Image gameOverBG;
    public Text gameOverText;

    // Start is called before the first frame update
    void Start()
    {
        HideGameOver();
    }

    public void HideIntro () {
        introBG.gameObject.SetActive(false);
    }

    public void HideGameOver () {
        gameOverBG.gameObject.SetActive(false);
    }

    public void ShowGameOver (bool win) {
        gameOverBG.gameObject.SetActive(true);
        gameOverText.text = win ? System.Text.RegularExpressions.Regex.Unescape("You Win!\nPress R to Restart\n or Q to Quit") : System.Text.RegularExpressions.Regex.Unescape("Game Over!\nPress R to Restart\n or Q to Quit");
    }

    public void TimerText (float seconds) {
        System.TimeSpan formattedTime = System.TimeSpan.FromSeconds(seconds);
        timerText.text = string.Format("{0:D2}:{1:D2}", formattedTime.Minutes, formattedTime.Seconds);
    }
}
