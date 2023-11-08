using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueBubbleTracker : MonoBehaviour
{
    Transform targetObject; 
    Vector2 offset = new(0f, 0f);
    float lerpSpeed = 5f; 

    [SerializeField] Vector2 clampPadding = new(0.5f, 0.5f);

    [SerializeField] SpriteRenderer dialogueBubbleBackground;

    [SerializeField] CameraReference cam;



    private void Update() {
        if (targetObject != null) { MoveToPosition(); } 
    }

    void MoveToPosition() {
        Vector2 trueOffset = GenerateOffsetFromTarget();
        Vector2 desiredPosition = (Vector2)targetObject.position + trueOffset;
        Vector2 clampedPosition = ClampToScreen(desiredPosition);

        transform.position = Vector2.Lerp(transform.position, clampedPosition, lerpSpeed * Time.deltaTime);
    }


    private Vector2 ClampToScreen(Vector2 position) {
        Vector2 size = dialogueBubbleBackground.bounds.size;

        Vector2 min = (Vector2)cam.Value.ViewportToWorldPoint(Vector2.zero) + size / 2;
        Vector2 max = (Vector2)cam.Value.ViewportToWorldPoint(Vector2.one)  - size / 2;

        position.x = Mathf.Clamp(position.x, min.x  + clampPadding.x, max.x - clampPadding.x);
        position.y = Mathf.Clamp(position.y, min.y  + clampPadding.y, max.y - clampPadding.y);

        return position;
    }

    Vector2 GenerateOffsetFromTarget() {
        Vector2 sizeOffset = dialogueBubbleBackground.bounds.size / 2 * offset;
        return sizeOffset + offset * 0.5f;
    }
    public void Follow(Transform target, Vector2 offset, float lerpSpeed) {
        transform.position = target.position; 
        this.targetObject = target;
        this.offset = offset;
        this.lerpSpeed = lerpSpeed;
    }

    public void StopFollowing() {
        this.targetObject = null;
        this.offset = Vector2.zero;
    }
}