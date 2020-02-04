using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickTarget : MonoBehaviour
{
    public int type = 0;

    public Sprite[] fillSprites;
    public int[] fillThresholds;

    public SpriteRenderer spriteRenderer;

    public Transform kickTarget;

    public GameController gameController;

    public float pickupTweenTime = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        int numKicksInPlace = 0;
        for (int i = 0; i < gameController.kickObjects.Length; i++) {
            if (gameController.kickObjects[i].inPlace) {
                numKicksInPlace++;
            }
        }

        /*float progress = (float)numKicksInPlace / (float)gameController.kickObjects.Length;
        Debug.Log("progress " + progress);
        Debug.Log("fillSprites.Length " + fillSprites.Length);
        Debug.Log("Mathf.FloorToInt(progress * fillSprites.Length) " + Mathf.FloorToInt(progress * fillSprites.Length));
        */
        int index = 0;
        for (int i = 0; i < fillThresholds.Length; i++) {
            if (numKicksInPlace >= fillThresholds[i]) {
                index++;
            }
        }
        spriteRenderer.sprite = fillSprites[index];
    }

    public IEnumerator PickupKickObject (KickObject obj) {
        // turn off the rigidbody and collider.
        obj.rb.isKinematic = true;
        obj.coll.enabled = false;
        obj.rb.velocity = Vector2.zero;
        obj.spriteRenderer.sortingOrder += 100;

        float startTime = Time.time;
        Vector3 start = obj.transform.position;
        Color startColor = obj.spriteRenderer.color;
        Color endColor = obj.spriteRenderer.color;
        endColor.a = 0f;
        while (Time.time - startTime < pickupTweenTime) {
            float t = (Time.time - startTime) / pickupTweenTime;
            obj.transform.position = Vector3.Lerp(start, kickTarget.position, t);
            obj.spriteRenderer.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }
        obj.transform.position = kickTarget.position;
        obj.inPlace = true;
        obj.gameObject.SetActive(false);
    }
}
