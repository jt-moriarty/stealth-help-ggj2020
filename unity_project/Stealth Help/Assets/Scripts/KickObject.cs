using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickObject : MonoBehaviour
{
    public Collider2D coll;
    public Rigidbody2D rb;

    public bool inPlace = false;

    public int type = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator TweenToTarget (Vector3 target, float time) {
        float startTime = Time.time;
        Vector3 start = transform.position;
        while (Time.time - startTime < time) {
            float t = (Time.time - startTime) / time;
            transform.position = Vector3.Lerp(start, target, t);
            yield return null;
        }
        gameObject.SetActive(false);
        transform.position = target;
        inPlace = true;
    }

    void OnTriggerEnter2D (Collider2D otherColl) {
        KickTarget target = otherColl.GetComponent<KickTarget>();
        if (target != null && target.type == type) {
            rb.isKinematic = true;
            coll.enabled = false;
            rb.velocity = Vector2.zero;
            Vector3 tweenTarget = otherColl.gameObject.transform.position;
            //tweenTarget.x += otherColl.offset.x;
            //tweenTarget.x += otherColl.offset.y;
            StartCoroutine(TweenToTarget(tweenTarget, 0.5f));
        }
    }
}
