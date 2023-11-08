using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuckMovement : MonoBehaviour
{
    [SerializeField] FloatArrayReference xBounds;

    int movementStyle = 0;

    public DuckMovementDirection DuckMovementDirection { get { return duckDir; } }

    CloudMovement attachedCloud;
    public void AttachCloud(CloudMovement cloud) { this.attachedCloud = cloud; }

    private void Update() {
        if (stopMoving) { return; }
        switch (movementStyle) {
            case 0:
                Vector2 nextDir = (int)duckDir * speedMult * Time.deltaTime * GetStyleVelocity(movementStyle);
                transform.Translate(nextDir);
                break;
            case 1:

                baseTimestamp += speedMult * Time.deltaTime;
                float angle = baseTimestamp;
                
                Vector2 offset = new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad)) * tetherRadius;
                transform.position = (Vector2)tetherPivot.transform.position + offset;
                break;

        }
        if (duckDir == DuckMovementDirection.Left) {
            if (transform.position.x < xBounds.Value[0]) {
                Destroy(this.gameObject);
            }
        } else {
            if (transform.position.x > xBounds.Value[1]) {
                Destroy(this.gameObject);
            }
        }
    }

    public void Launch(Vector2 source, DuckMovementDirection dir, float speedMult, int movementStyle) {
        transform.position = source;

        this.speedMult = speedMult;
        this.duckDir = dir;
        this.movementStyle = movementStyle;
        baseTimestamp = Time.time;
    }
    GameObject tetherPivot;
    float tetherRadius = 2.5f;
    public void LaunchTetheredTopoint(float degreeOffset, DuckMovementDirection dir, float speedMult, int movementStyle, GameObject pivotPoint) {
        //transform.position = (Vector2)pivotPoint.transform.position + direction * 2.0f;

        this.speedMult = speedMult;
        this.duckDir = dir;
        this.movementStyle = movementStyle;
        this.tetherPivot = pivotPoint;

        baseTimestamp = 0.0f + degreeOffset;
    }

    // called by animation events (violent spiral and flip and disappear)
    bool stopMoving = false;
    public void NullParent() {
        if (attachedCloud != null) {
            switch (movementStyle) {
                case 0:
                    attachedCloud.Detach(new Vector2(1, 0) * (int)duckDir * speedMult);
                    break;
                case 1:
                    Destroy(attachedCloud.gameObject);
                    break;

            }
        }

        stopMoving = true;
        //modelPivot.transform.parent = null;
    }



    DuckMovementDirection duckDir;
    float speedMult = 1;
    float baseTimestamp = 0;
    Vector2 GetStyleVelocity(int style) {
        switch (style) {
            //  default movement
            case 1:
                return new Vector2(0, 0);
            case 0:
            default:
                return new Vector2(1, 0);


        }
    }

}

public enum DuckMovementType
{
    Default = 0,
    Sin = 1,
}

public enum DuckMovementDirection
{
    Left = -1,
    Right = 1
}