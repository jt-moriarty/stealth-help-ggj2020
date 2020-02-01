using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Animator animControl;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Open () {
        animControl.SetTrigger("Open");
        animControl.ResetTrigger("Close");
    }

    public void Close () {
        animControl.SetTrigger("Close");
        animControl.ResetTrigger("Open");
    }
}
