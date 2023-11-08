using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairVisual : MonoBehaviour
{
    [SerializeField] RectTransform crosshairRect;
    [SerializeField] Image crosshairImage;

    [SerializeField] float minimumWidthProportion = 0.1f;

    [SerializeField] Slider reloadVisual;
    [SerializeField] FloatReference reloadTime;

    [SerializeField] FloatReference reloadTimestamp;

    public bool crosshairActive = false;
    void Update() {
        // Get the mouse position in screen space.
        Vector3 mousePosition = Input.mousePosition;

        // Set the crosshair's position to the mouse position.
        crosshairRect.position = mousePosition;


        float smaller = Mathf.Min(Screen.currentResolution.width, Screen.currentResolution.height);
        float size = minimumWidthProportion * smaller;
        crosshairRect.sizeDelta = new Vector2(size, size);



        reloadVisual.value = (reloadTimestamp.Value - Time.time) / reloadTime.Value;
    }

    public void SetCrosshairActive(bool value) {
        crosshairActive = value;
        crosshairImage.enabled = value;
    }
}
