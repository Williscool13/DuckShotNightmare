using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudMovement : MonoBehaviour
{
    [SerializeField] Vector2 offsetFromDuck;
    [SerializeField] FloatArrayReference xBounds;
    Transform duck;

    bool detached = false;
    Vector2 detachedVel = Vector2.zero;

    private void FixedUpdate() {
        
        if (detached) { DetachLogic(); return; }

        if (duck == null) { Destroy(this.gameObject); return; }

        transform.position = (Vector2)duck.position + offsetFromDuck;
    }


    public void AttachDuck(Transform duck) {
        this.duck = duck;
    }

    public void Detach(Vector2 direction) {
        detachedVel = direction;
        detached = true;
        this.duck = null;
    }

    void DetachLogic() {
        transform.Translate(detachedVel * Time.fixedDeltaTime);

        if (detachedVel.x > 0) {
            if (transform.position.x > xBounds.Value[1]) {
                Destroy(this.gameObject);
            }
        }
        else if (detachedVel.x < 0) {
            if (transform.position.x < xBounds.Value[0]) {
                Destroy(this.gameObject);
            }
        }
    }

}
