using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class ControlPointLight2D : MonoBehaviour
{
    public Light2D controlLight;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //controlLight.pointLightOuterRadius = Mathf.Lerp();
        //transform.RotateAround(transform.position, Vector3.forward, 30f * Time.deltaTime);
        //controlLight.pointLightOuterRadius = Mathf.PingPong(Time.time, 9f);
    }
}
