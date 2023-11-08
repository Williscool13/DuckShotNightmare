using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuckSpawner : MonoBehaviour
{
    [SerializeField] GameObject duck;
    [SerializeField] GameObject giantDuck;
    [SerializeField] GameObject cloud;
    [SerializeField] FloatArrayReference rowPositionsSO;
    [SerializeField] FloatArrayReference xBounds;
    [SerializeField] IntegerReference bossMaxHealth;
    public GameObject SpawnDuck(SpawnPositionX posX, SpawnPositionY posY, DuckType duckType,
        int duckHealth, int duckMovementType,
        int movementAnimId, int deathId,
        DuckMovementDirection direction, float velocityMult, bool dodger, float tetherOffset) {
     


        // spawn duck
        GameObject _duck = Instantiate(duck);

        DuckMovement dmovement = _duck.GetComponent<DuckMovement>();
        switch (duckMovementType) {
            case 0:
                float positionX = posX switch {
                    SpawnPositionX.left => xBounds.Value[0],
                    SpawnPositionX.right => xBounds.Value[1],
                    _ => xBounds.Value[0]
                };
                float positionY = posY switch {
                    SpawnPositionY.top => rowPositionsSO.Value[2],
                    SpawnPositionY.middle => rowPositionsSO.Value[1],
                    SpawnPositionY.bottom => rowPositionsSO.Value[0],
                    _ => rowPositionsSO.Value[0],
                };
                dmovement.Launch(new Vector2(positionX, positionY), direction, velocityMult, 0);
                break;
            case 1:
                dmovement.LaunchTetheredTopoint(tetherOffset, direction, velocityMult, 1, GameObject.FindObjectOfType<GiantDuckGameController>().gameObject.transform.GetChild(0).GetChild(1).gameObject);
                break;

        }



        

        Animator danimator = _duck.GetComponent<Animator>();
        danimator.SetInteger("Movement", movementAnimId);
        danimator.SetInteger("Death", deathId);
        danimator.SetTrigger("MovementTrigger");

        bool flipped = direction == DuckMovementDirection.Left;
        SpriteRotationManager dspritemanager = _duck.GetComponent<SpriteRotationManager>();
        if (flipped) { dspritemanager.SetFlipSpriteX(true); }

               
        DuckTarget dtarget = _duck.GetComponent<DuckTarget>();
        (dtarget as IHealth)?.Setup(duckHealth);
        (dtarget as IDuck)?.Setup(duckType);

        if (dodger) {
            DuckDodger ddodger = _duck.GetComponent<DuckDodger>();
            ddodger.StartDodging();
        }

        // spawn cloud
        GameObject _cloud = Instantiate(cloud);
        CloudMovement cmovement = _cloud.GetComponent<CloudMovement>();
        cmovement.AttachDuck(_duck.transform);
        dmovement.AttachCloud(cmovement);

        return _duck;
    }

    public void SpawnMallardDuck(SpawnPositionX posX, SpawnPositionY posY, DuckMovementDirection direction, int deathId, float velocityMult) {
        SpawnDuck(posX, posY, DuckType.Mallard, 2, 0, 0, deathId, direction, velocityMult, false, 0);
    }

    public void SpawnYellowDuck(SpawnPositionX posX, SpawnPositionY posY, DuckMovementDirection direction, int deathId, float velocityMult) {
        SpawnDuck(posX, posY, DuckType.Yellow, 4, 0, 0, deathId, direction, velocityMult, false, 0);
    }

    public void SpawnWhiteDuck(SpawnPositionX posX, SpawnPositionY posY, DuckMovementDirection direction, int deathId, float velocityMult) {
        SpawnDuck(posX, posY, DuckType.White, 6, 0, 0, deathId, direction, velocityMult, false, 0);
    }

    public void SpawnDodger(SpawnPositionX posX, SpawnPositionY posY, DuckMovementDirection direction, int deathId, float velocityMult) {
        SpawnDuck(posX, posY, DuckType.Dodger, 3, 0, 0, deathId, direction, velocityMult, true, 0);
    }


    public void SpawnMallardOneHP(SpawnPositionX posX, SpawnPositionY posY, DuckMovementDirection direction, int deathId, float velocityMult) {
        SpawnDuck(posX, posY, DuckType.Mallard, 1, 0, 0, deathId, direction, velocityMult, false, 0);
    }

    public void SpawnDodgerOneHP(SpawnPositionX posX, SpawnPositionY posY, DuckMovementDirection direction, int deathId, float velocityMult) {
        SpawnDuck(posX, posY, DuckType.Dodger, 1, 0, 0, deathId, direction, velocityMult, true, 0);
    }

    public GameObject SpawnRotator(float offsetDegrees, DuckMovementDirection moveDirection, int deathId, float velocityMult) {
        return SpawnDuck(SpawnPositionX.left, SpawnPositionY.top, DuckType.Mallard, 1, 1, 0, deathId, moveDirection, velocityMult, false, offsetDegrees);
    }
    public GameObject SpawnGoldenRotator(float offsetDegrees, DuckMovementDirection moveDirection, int deathId, float velocityMult) {
        return SpawnDuck(SpawnPositionX.left, SpawnPositionY.top, DuckType.Yellow, 2, 1, 0, deathId, moveDirection, velocityMult, false, offsetDegrees);
    }

    public void SpawnGiantDuck() {
        // spawn duck
        GameObject _duck = Instantiate(giantDuck);
        _duck.transform.position = new Vector3(-10, -10, -15);


        Animator danimator = _duck.GetComponent<Animator>();
        danimator.SetTrigger("MovementTrigger");

        GiantDuckTarget dtarget = _duck.GetComponent<GiantDuckTarget>();
        (dtarget as IHealth)?.Setup(bossMaxHealth.Value);
        (dtarget as IDuck)?.Setup(DuckType.Boss);
    }

    public GameObject FindRandomDuckLessThanHalfway() {
        GameObject[] gobs = GameObject.FindGameObjectsWithTag("Duck");
        List<GameObject> validDucks = new();

        foreach (GameObject gob in gobs) {
            if (gob.GetComponent<DuckTarget>().Alive) {
                DuckMovement dmovement = gob.GetComponent<DuckMovement>();
                DuckMovementDirection direction = dmovement.DuckMovementDirection;

                float distFromEnd = 0;
                if (direction == DuckMovementDirection.Left) {
                    distFromEnd = gob.transform.position.x - xBounds.Value[0];
                } else {
                    distFromEnd = xBounds.Value[1] - gob.transform.position.x;
                }
                if (distFromEnd > Mathf.Abs(xBounds.Value[0] - xBounds.Value[1]) / 2) {
                    validDucks.Add(gob);
                }
            }
        }

        if (validDucks.Count > 0) {
            int randDuck = Random.Range(0, validDucks.Count);
            GameObject chosenDuck = validDucks[randDuck];


            return chosenDuck;
        }
        else {
            Debug.Log("[Dialogue Tester] No ducks less than halfway to end on screen");
            return null;
        }
    }

}

public enum SpawnPositionX { left, right }
public enum SpawnPositionY { top, middle, bottom }

public enum DuckType { Mallard, Yellow, White, Dodger, Boss }