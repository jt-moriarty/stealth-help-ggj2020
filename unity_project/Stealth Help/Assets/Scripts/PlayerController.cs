using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class PlayerController : MonoBehaviour
{
    public float MOVEMENT_BASE_SPEED = 1.0f;
    public float GRAB_RANGE = 1f;
    public float HOLD_RANGE = 3f;

    public float SPOTLIGHT_DISTANCE_FUZZ = -0.75f;
    public float SIGHT_RADIUS_ADJUST = -0.25f;

    public Vector2 movementDirection;
    public Vector2 facingDirection = Vector2.down;
    public float movementSpeed;

    public Transform grabPivot;

    public Vector2 seenDirection = Vector2.zero;

    public Rigidbody2D rb;
    public CircleCollider2D coll;

    List<RaycastHit2D> seenHits = new List<RaycastHit2D>();

    public KickObject held;
    public Transform originalHeldParent;
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInputs();
        Move();
        Animate();
    }

    void LateUpdate () {
        MoveHeldObject();
    }

    void MoveHeldObject() {
        if (held != null) {
            Vector3 facingDir = new Vector3(facingDirection.x, facingDirection.y, 0f);
            facingDir.Normalize();
            held.transform.position = grabPivot.transform.position + (facingDir * HOLD_RANGE);
        }
    }

    void Animate() {
        if (movementDirection != Vector2.zero) {
            animator.SetFloat("Horizontal", movementDirection.x);
            animator.SetFloat("Vertical", movementDirection.y);

            //Check if pushing.
            Vector2 castOrigin = new Vector2(transform.position.x, transform.position.y) + coll.offset;
            Vector2 castDirection = facingDirection;
            int layerMask = ~LayerMask.GetMask("Player");

            //Vector2 origin, float radius, Vector2 direction, float distance = Mathf.Infinity, int layerMask = DefaultRaycastLayers
            RaycastHit2D hit = Physics2D.CircleCast(castOrigin, coll.radius, facingDirection, 0.1f, layerMask);

            if (hit && hit.collider.gameObject.CompareTag("PushPull")) {
                animator.SetBool("Pushing", true);
            }
            else {
                animator.SetBool("Pushing", false);
            }
        }
        else {
            animator.SetBool("Pushing", false);
        }

        animator.SetFloat("Speed", movementSpeed);

        if (movementDirection != Vector2.zero) {

        }
    }

    void ProcessInputs() {
        movementDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        movementSpeed = Mathf.Clamp(movementDirection.magnitude, 0.0f, 1.0f);
        movementDirection.Normalize();
        if (movementDirection != Vector2.zero) {
            facingDirection = movementDirection;
        }

        if (Input.GetButtonDown("Grab") && held == null) {
            //Debug.Log("Try Grab!");
            PickupObject();
        }
        else if (Input.GetButtonUp("Grab") && held != null) {
            //Debug.Log("Try Release!");
            DropObject();
        }
    }

    void PickupObject() {
        Vector2 castOrigin = new Vector2(transform.position.x, transform.position.y) + coll.offset;
        Vector2 castDirection = facingDirection;
        int layerMask = ~LayerMask.GetMask("Player");

        //Vector2 origin, float radius, Vector2 direction, float distance = Mathf.Infinity, int layerMask = DefaultRaycastLayers
        RaycastHit2D hit = Physics2D.CircleCast(castOrigin, coll.radius, facingDirection, GRAB_RANGE, layerMask);//Physics2D.Raycast(castOrigin, castDirection, GRAB_RANGE, layerMask);
        //Debug.DrawRay(castOrigin, castDirection, Color.blue, 2f);

        if (hit && hit.collider != null) {
            //Debug.Log("hit " + hit + " hit.collider " + hit.collider.gameObject.name);
        }
        if (hit && hit.collider.gameObject.CompareTag("Kick") && !hit.collider.isTrigger) {
            held = hit.collider.GetComponent<KickObject>();
            originalHeldParent = held.transform.parent;
            held.transform.SetParent(transform);
            held.rb.velocity = Vector2.zero;
            held.rb.isKinematic = true;
            held.coll.enabled = false;
            held.spriteRenderer.sortingOrder += 1;
        }
    }

    void DropObject() {
        if (held != null) {
            held.transform.SetParent(originalHeldParent);
            held.rb.isKinematic = false;
            held.rb.velocity = Vector2.zero;
            held.coll.enabled = true;
            held.spriteRenderer.sortingOrder -= 1;
        }
        held = null;
        originalHeldParent = null;
    }

    void Move () {
        rb.velocity = movementDirection * movementSpeed * MOVEMENT_BASE_SPEED;
    }

    public void GameOver (bool win) {
        enabled = false;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        animator.SetTrigger(win ? "Victory" : "Death");
        animator.SetFloat("Horizontal", seenDirection.x);
        animator.SetFloat("Vertical", seenDirection.y);
    }

    private string[] seenLayerNames = new string[] { "Default" };

    public bool CheckSeenByLight (Light2D light) {
        seenHits.Clear();
        Vector2 castOrigin = new Vector2(transform.position.x, transform.position.y) + coll.offset;
        Vector2 castDestination = new Vector2(light.transform.position.x, light.transform.position.y);
        float castRadius = coll.radius + SIGHT_RADIUS_ADJUST; //TODO: decrease for more leniancy?
        ContactFilter2D contactFilter2D = new ContactFilter2D();
        contactFilter2D.NoFilter();
        contactFilter2D.useTriggers = false;

        int layerMask = LayerMask.GetMask(seenLayerNames);//~LayerMask.GetMask("Bounds");
        contactFilter2D.SetLayerMask(layerMask);
        int numHits = Physics2D.CircleCast(castOrigin, castRadius, castDestination - castOrigin, contactFilter2D, seenHits, light.pointLightOuterRadius);
        bool seen = false;
        float closestDistance = light.pointLightOuterRadius + SPOTLIGHT_DISTANCE_FUZZ;
        RaycastHit2D closestHit = new RaycastHit2D();
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
                closestHit = seenHits[i];
            }
            else if (seenHits[i].distance < closestDistance) {
                seen = false;
                closestDistance = seenHits[i].distance;
                closestHit = seenHits[i];
            }
        }
        
        Vector2 lightForward = Vector2.zero;
        lightForward.x = light.transform.up.x;
        lightForward.y = light.transform.up.y;

        Vector2 parentForward = Vector2.zero;
        parentForward.x = light.transform.parent.up.x;
        parentForward.y = light.transform.parent.up.y;
        parentForward = -parentForward;

        // destination - source
        Vector2 playerDirection = -(castDestination - castOrigin);

        // Last check if the angle frees us
        //TODO: add angle checking.
        if (seen) {

            //Debug.Log("ANGLE " + Vector2.Angle(playerDirection, lightForward));
            //Debug.Log("FORWARD ANGLE " + Vector2.Angle(playerDirection, parentForward));
            //Debug.Log("pointLightOuterAngle " + light.pointLightOuterAngle);
            //Debug.Log("pointLightOuterAngle " + light.pointLightInnerAngle);

            //UnityEditor.EditorApplication.isPaused = true;
            if (Vector2.Angle(playerDirection, lightForward) > (light.pointLightOuterAngle / 2f)) {
                seen = false;
                Debug.DrawRay(light.transform.position, lightForward, Color.cyan);
                Debug.DrawRay(light.transform.position, playerDirection, Color.cyan);
            }
            else {
                Debug.DrawRay(light.transform.position, lightForward, Color.red);
                Debug.DrawRay(light.transform.position, playerDirection, Color.red);
            }
        }


        //seen = false;
        if (seen) {
            //Debug.DrawLine(castOrigin, castDestination, Color.red, 1f);
            seenDirection = castDestination - castOrigin;
            seenDirection.Normalize();
        }
        /*else if (numHits > 0 && closestDistance != light.pointLightOuterRadius) {
            Debug.DrawLine(castOrigin, closestHit.point, Color.green, 1f);
        }*/
        return seen;
    }
}
