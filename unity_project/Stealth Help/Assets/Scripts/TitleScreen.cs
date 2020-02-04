using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
    private bool nextScene = false;

    public AudioSource menuTheme;
    public CanvasGroup startGroup;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        startGroup.alpha = Mathf.PingPong(Time.time * 1.5f, 1f);

        if (Input.GetButtonDown("Grab") && !nextScene) {
            nextScene = true;
            SceneManager.LoadScene("Main");
        }
    }
}
