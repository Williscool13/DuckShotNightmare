using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiantDuckMovement : MonoBehaviour
{
    float[] xMapBounds = new float[] { -8, 8 };
    public bool IsOffScreen() {
        if (movementDirection == DuckMovementDirection.Left) {
            return transform.position.x < xMapBounds[0];
        } else {
            return transform.position.x > xMapBounds[1];
        }
    }

    DuckMovementDirection movementDirection;


    void Update()
    {
        if (halted) { return; }
        if (!moving) { return; } 
        
        if (IsOffScreen()){ moving = false; }

        switch (movementId) {
            case 0:
                Vector2 nextDir = (int)movementDirection * Time.deltaTime * new Vector2(1, 0);
                transform.Translate(nextDir);
                break;
            default:
                break;
        }


        
    }
    bool moving;
    bool halted;
    int movementId = -1;
    public void LaunchNormal(Vector2 source, DuckMovementDirection dir, float speed) {
        transform.position = source;
        movementDirection = dir;
        movementId = 0;
        moving = true;
        halted = false;
    }

    public void HaltMovement() {
        halted = true;
        movementId = 0;

    }
}
