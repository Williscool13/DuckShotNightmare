using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteRotationManager : MonoBehaviour
{
    [SerializeField] CameraReference cam;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Sprite baseModel;
    [SerializeField] Sprite behindModel;

    bool backFacing = false;
    bool flipped = false;
    public bool Flipped { get { return flipped; } }

    void FixedUpdate() {
        Vector3 forward = spriteRenderer.transform.forward;
        if (flipped) { forward *= -1; }

        float angle = Vector3.Angle(forward, cam.Value.transform.forward);
        angle = DegreesWithin360(angle);

        bool backShowing = angle > 90.0f && angle < 270.0f;

        if (backShowing) {
            if (!backFacing) {
                // Set the sprite to the backSprite
                spriteRenderer.sprite = behindModel;
                spriteRenderer.color = Color.white;
                backFacing = true;
            }
        }
        else {
            if (backFacing) {
                // Set the sprite to the frontSprite
                spriteRenderer.sprite = baseModel;
                spriteRenderer.color = this.frontColor;
                backFacing = false;
            }
        }
    }

    public float DegreesWithin360(float degrees) {
        while (degrees < 0) {
            degrees += 360.0f;
        }

        if (degrees > 360.0f) {
            degrees %= 360.0f;
        }

        return degrees;
    }


    public void SetFlipSpriteX(bool val) {
        flipped = val;
        if (val) {
            spriteRenderer.transform.eulerAngles = new Vector3(0, 180.0f, 0);
        } else {
            spriteRenderer.transform.eulerAngles = new Vector3(0, 0f, 0);
        }
    }


    public void SetFrontFace(Sprite sprite) {
        this.baseModel = sprite;
        if (!backFacing) { spriteRenderer.sprite = baseModel; }
    }


    public void SetBackFace(Sprite sprite) {
        this.behindModel = sprite;
        if (backFacing) { spriteRenderer.sprite = behindModel; }
    }

    Color frontColor = Color.white;
    public void SetFrontColor(Color color) {
        this.frontColor = color;
        if (!backFacing) { spriteRenderer.color = this.frontColor; }
       
    }
}


