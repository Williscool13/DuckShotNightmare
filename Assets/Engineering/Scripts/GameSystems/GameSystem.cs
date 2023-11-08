using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameSystem : MonoBehaviour
{
    //[SerializeField] LerpCameraToTarget cameraMover;
    [SerializeField] GameTransitioner gameTransitioner;

    // Start is called before the first frame update
    void Start() { 
        Application.targetFrameRate = Screen.currentResolution.refreshRate;

        inputValidTimestamp = Time.time + 1.0f;
    }

    float inputValidTimestamp;
    bool gameStarted = false;
    void Update()
    {
        if (inputValidTimestamp > Time.time) { return; }
        if (Input.GetMouseButtonDown(0)) {
            Cursor.visible = false;
            if (!gameStarted) {
                gameTransitioner.StartGameTransition();
                gameStarted = true;

            }
        }
    }
}
