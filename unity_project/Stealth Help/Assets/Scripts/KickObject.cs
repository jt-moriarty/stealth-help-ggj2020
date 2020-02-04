using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickObject : MonoBehaviour
{
    public Collider2D coll;
    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;

    public bool inPlace = false;

    public int type = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnTriggerEnter2D (Collider2D otherColl) {
        KickTarget target = otherColl.GetComponent<KickTarget>();
        if (target != null && target.type == type) {
            target.StartCoroutine(target.PickupKickObject(this));
        }
    }
}
