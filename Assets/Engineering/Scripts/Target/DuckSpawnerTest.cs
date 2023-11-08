using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DuckSpawnerTest : MonoBehaviour
{
    [SerializeField] UnityEvent gameStartEvent;

    [SerializeField] int deathID;
    [SerializeField] int healthCount;
    [SerializeField] DuckSpawner spawner;

    [SerializeField] CameraReference mainCam;
    [SerializeField] float shakeIntensity = 0.5f;
    [SerializeField] int delayCount = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            gameStartEvent.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            spawner.SpawnDodger(SpawnPositionX.left, SpawnPositionY.bottom, DuckMovementDirection.Right, deathID, 0);
            //spawner.SpawnDuck(SpawnPositionX.left, SpawnPositionY.bottom, DuckType.Mallard, healthCount, 0, deathID, DuckMovementDirection.Right, 0, false);            
        }   
        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            spawner.SpawnGiantDuck();
            //spawner.SpawnDuck(SpawnPositionX.left, SpawnPositionY.bottom, DuckType.Mallard, healthCount, 0, deathID, DuckMovementDirection.Right, 0, false);            
        }   
        if (Input.GetKeyDown(KeyCode.Alpha4)) {
            mainCam.Value.GetComponent<CameraShaker>().StartShake(shakeIntensity, 2.0f, delayCount);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5)) {
            mainCam.Value.GetComponent<CameraShaker>().StopShake();
        }
    }
}
