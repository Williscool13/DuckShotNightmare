using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DuckDodger : MonoBehaviour
{
    [SerializeField] CameraReference mainCam;
    [SerializeField] FloatReference bulletRadius;
    [SerializeField] float dodgeCooldown = 3.5f;

    float dodgeCooldownTimestamp;
    void FixedUpdate() {
        if (dodgeCooldownTimestamp > Time.time) { return; }

        Vector2 mousePosition = mainCam.Value.ScreenToWorldPoint(Input.mousePosition);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(mousePosition, bulletRadius.Value);
        Debug.Log($"Checking if thing { colliders.Length }");
        for (int i = 0; i < colliders.Length; i++) {
            if (colliders[0].transform.root.gameObject == this.gameObject) {
                dodgeCooldownTimestamp = Time.time + dodgeCooldown;
                Debug.Log("Dodging");
                StartCoroutine(Dodging());
                return;
            }
        }
    }

    public void StartDodging() {
        this.enabled = true;
    }

    IEnumerator Dodging() {
        int xDir = Random.value > 0.5 ? 1 : -1;

        float maxDist = 1.0f;
        float speed = 10.0f;
        while (maxDist > 0) {
            float dist = speed * Time.deltaTime;

            maxDist -= dist;
            transform.Translate(new Vector2(dist * xDir, 0));
            yield return null;
        }

    }
}
