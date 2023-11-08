using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunVisual : MonoBehaviour
{
    [SerializeField] RectTransform gunRect;
    [SerializeField] Image gunImage;
    [SerializeField] Vector2 maxDriftVector;
    [SerializeField] Vector2 basePosition;
    [SerializeField] float lerpSpeed = 3.0f;
    public bool gunactive = false;
    
    private void Update() {
        Vector2 baseRectPos = basePosition * new Vector2(Screen.width, Screen.height);

        if (reloadTimestamp > Time.time) {
            baseRectPos += new Vector2(0, -300.0f);
        }

        // Get the mouse position in screen space.
        Vector3 mousePosition = Input.mousePosition;
        //float xmaxDrift = maxDrift * (Screen.width  / baseResolution.x);
        //float yMaxDrift = maxDrift * (Screen.height / baseResolution.y);
        float xmaxDrift = maxDriftVector.x * Screen.width;
        float yMaxDrift = maxDriftVector.y * Screen.height;
        float xDrift = Mathf.InverseLerp(0, Screen.width, mousePosition.x);
        float yDrift = Mathf.InverseLerp(0, Screen.height, mousePosition.y);
        Vector2 drift = new(xmaxDrift * (xDrift - 0.5f), yMaxDrift * (yDrift - 0.5f));
       
        gunRect.position = Vector3.Lerp(gunRect.position, baseRectPos + drift, lerpSpeed * Time.deltaTime);

    }

    public void SetGunActive(bool value) {
        transform.position = transform.position + new Vector3(0, -300.0f, 0);
        gunactive = value;
        gunImage.enabled = value;
        Cursor.visible = false;
    }

    float reloadTimestamp = 0;
    public void Reload(float reloadTIme) {
        reloadTimestamp = Time.time + reloadTIme;
    }
}
