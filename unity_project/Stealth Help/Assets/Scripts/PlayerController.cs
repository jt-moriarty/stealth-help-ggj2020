using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class PlayerController : MonoBehaviour
{
    public float MOVEMENT_BASE_SPEED = 1.0f;
    public float GRAB_RANGE = 1f;

    public Vector2 movementDirection;
    public Vector2 facingDirection = Vector2.down;
    public float movementSpeed;

    public Rigidbody2D rb;
    public CircleCollider2D coll;

    List<RaycastHit2D> seenHits = new List<RaycastHit2D>();

    public KickObject held;
    public Transform originalHeldParent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInputs();
        Move();
    }

    void ProcessInputs() {
        movementDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        movementSpeed = Mathf.Clamp(movementDirection.magnitude, 0.0f, 1.0f);
        movementDirection.Normalize();
        if (movementDirection != Vector2.zero) {
            facingDirection = movementDirection;
        }

        if (Input.GetButtonDown("Grab") && held == null) {
            Debug.Log("Try Grab!");
            PickupObject();
        }
        else if (Input.GetButtonUp("Grab") && held != null) {
            Debug.Log("Try Release!");
            DropObject();
        }
    }

    void PickupObject() {
        Vector2 castOrigin = new Vector2(transform.position.x, transform.position.y) + coll.offset;
        Vector2 castDirection = facingDirection;
        int layerMask = ~LayerMask.GetMask("Player");

        //Vector2 origin, float radius, Vector2 direction, float distance = Mathf.Infinity, int layerMask = DefaultRaycastLayers
        RaycastHit2D hit = Physics2D.CircleCast(castOrigin, coll.radius + GRAB_RANGE, Vector2.zero, 0f, layerMask);//Physics2D.Raycast(castOrigin, castDirection, GRAB_RANGE, layerMask);
        //Debug.DrawRay(castOrigin, castDirection, Color.blue, 2f);

        if (hit && hit.collider != null) {
            //Debug.Log("hit " + hit + " hit.collider " + hit.collider.gameObject.name);
        }
        if (hit && hit.collider.gameObject.CompareTag("Kick") && !hit.collider.isTrigger) {
            held = hit.collider.GetComponent<KickObject>();
            originalHeldParent = held.transform.parent;
            held.transform.SetParent(transform);
            held.rb.isKinematic = true;
            held.coll.enabled = false;
        }
    }

    void DropObject() {
        if (held != null) {
            held.transform.SetParent(originalHeldParent);
            held.rb.isKinematic = false;
            held.coll.enabled = true;
        }
        held = null;
        originalHeldParent = null;
    }

    void Move () {
        rb.velocity = movementDirection * movementSpeed * MOVEMENT_BASE_SPEED;
    }

    public void GameOver () {
        enabled = false;
        rb.velocity = Vector2.zero;
    }

    public bool CheckSeenByLight (Light2D light) {
        seenHits.Clear();
        Vector2 castOrigin = new Vector2(transform.position.x, transform.position.y) + coll.offset;
        Vector2 castDestination = new Vector2(light.transform.position.x, light.transform.position.y);
        float castRadius = coll.radius; //TODO: decrease for more leniancy?
        ContactFilter2D contactFilter2D = new ContactFilter2D();
        contactFilter2D.NoFilter();
        contactFilter2D.useTriggers = false;

        int layerMask = ~LayerMask.GetMask("Bounds");
        contactFilter2D.SetLayerMask(layerMask);
        int numHits = Physics2D.CircleCast(castOrigin, castRadius, castDestination - castOrigin, contactFilter2D, seenHits, light.pointLightOuterRadius);
        bool seen = false;
        float closestDistance = light.pointLightOuterRadius;
        Collider2D closestCollider = null;
        for (int i = 0; i < numHits; i++) {
            //If the door is in our hit list AND 
            //there's no obstacle closer AND 
            //the dist is less than the outer radius of the light AND 
            //the angle is correct. (for partially open door. TODO

            if (seenHits[i].collider.gameObject.CompareTag("Player")) {
                //ignore
            }
            else if (seenHits[i].collider.gameObject.CompareTag("Door") && seenHits[i].distance < closestDistance) {
                seen = true;
                closestDistance = seenHits[i].distance;
            }
            else if (seenHits[i].distance < closestDistance) {
                seen = false;
                closestDistance = seenHits[i].distance;
                closestCollider = seenHits[i].collider;
            }
        }
        if (seen) {
            Debug.DrawLine(castOrigin, castDestination, Color.red, 1f);
        }
        else if (closestCollider != null) {
            Debug.DrawLine(castOrigin, closestCollider.gameObject.transform.position, Color.green, 1f);
        }
        return seen;
    }
}
