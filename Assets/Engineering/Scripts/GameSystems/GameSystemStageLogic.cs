using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameSystemStageLogic : MonoBehaviour
{
    [SerializeField] GameMusicManager musicPlayer;
    [SerializeField] AudioClip stageOneMusic;
    [SerializeField] AudioClip stageTwoMusic;

    [SerializeField] int stageSequenceIndex = 0;
    [SerializeField] bool stageActive = false;
    [SerializeField] DuckSpawner spawner;

    [SerializeField] DialogueWriterVariable mainWriter;
    [SerializeField] FloatArrayReference spawnPositions;
    [SerializeField] IntegerReference gameState;

    [SerializeField] GameObjectReference OnHitGob;
    [SerializeField] GameObjectReference OnDeathGob;

    [SerializeField] UnityEvent startGameEvent;

    float spawnTimestampPhaseZero;
    IEnumerator currentDialogueRoutine;

    SpawnPositionY[] spawnPoses = new SpawnPositionY[] { SpawnPositionY.bottom, SpawnPositionY.middle, SpawnPositionY.top };


    void Update() {
        if (!stageActive) { return; }

        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            stageSequenceIndex = 2;
            startGameEvent.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            stageSequenceIndex = 3;
            startGameEvent.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            stageSequenceIndex = 4;
            startGameEvent.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4)) {
            stageSequenceIndex = 5;
            startGameEvent.Invoke();
        }
        #endif

        switch (stageSequenceIndex) {
            case 0:
                if (musicPlayer.FadeInClip(stageOneMusic, 0.5f)) {
                    stageSequenceIndex++;
                }
                break;
            case 1:
                if (startedDialogueZero == false) {
                    startedDialogueZero = true;
                    currentDialogueRoutine = PhaseZeroDialogue();
                    StartCoroutine(currentDialogueRoutine);
                }
                HandleSpawnPhaseZero();
                break;
            case 2:
                // faster movement, 
                if (startedDialogueOne == false) {
                    startedDialogueOne = true;
                    StopCoroutine(currentDialogueRoutine);
                    currentDialogueRoutine = PhaseOneDialogue();
                    StartCoroutine(currentDialogueRoutine);

                    lastShootTimestampPhaseOne = Time.time + 10.0f;
                    lastHitTimestampPhaseOne = Time.time + 12.0f;
                    lastDeathTimestampPhaseOne = Time.time + 8.0f;
                    lastNormalTimestampPhaseOne = Time.time + 2.0f;
                }
                HandleSpawnPhaseOne();
                UpdatePhaseOne();
                break;
            case 3:
                if (startedDialogueTwo == false) {
                    startedDialogueTwo = true;
                    StopCoroutine(currentDialogueRoutine);
                    currentDialogueRoutine = PhaseTwoDialogue();
                    StartCoroutine(currentDialogueRoutine);

                    lastShootTimestampPhaseTwo = Time.time + 10.0f;
                    lastHitTimestampPhaseTwo = Time.time + 12.0f;
                    lastDeathTimestampPhaseTwo = Time.time + 8.0f;
                    lastNormalTimestampPhaseTwo = Time.time + 2.0f;

                    musicPlayer.PlayClip(stageTwoMusic, 0.5f, 0.5f, 0.1f);
                }
                HandleSpawnPhaseTwo();
                UpdatePhaseTwo();
                break;
            case 4:
                if (startedDialogueThree == false) {
                    startedDialogueThree = true;
                    StopCoroutine(currentDialogueRoutine);
                    currentDialogueRoutine = PhaseThreeDialogue();
                    StartCoroutine(currentDialogueRoutine);

                    lastShootTimestampPhaseThree = Time.time + 10.0f;
                    lastHitTimestampPhaseThree = Time.time + 12.0f;
                    lastDeathTimestampPhaseThree = Time.time + 8.0f;
                    lastNormalTimestampPhaseThree = Time.time + 2.0f;

                    musicPlayer.Distort(-0.2f, 0.2f);
                    //musicPlayer.PlayClip(stageThreeMusic, 0.5f, 0.5f, 0.1f);
                }
                HandleSpawnphaseThree();
                UpdatePhaseThree();
                break;
            case 5:
                StopCoroutine(currentDialogueRoutine);
                spawner.SpawnGiantDuck();
                stageSequenceIndex++;
                // start boss battle
                break;

        }


        prevShoot = false;
        prevHit = false;
        prevDeath = false;

    }

    bool prevShoot = false;
    bool prevHit = false;
    bool prevDeath = false;

    #region Phase Zero
    bool startedDialogueZero = false;
    GameObject phaseZeroTargetDuck;
    void HandleSpawnPhaseZero() {
        if (spawnTimestampPhaseZero < Time.time) {
            spawner.SpawnMallardDuck(SpawnPositionX.left, SpawnPositionY.top, DuckMovementDirection.Right, 0, 0.55f);
            spawner.SpawnMallardDuck(SpawnPositionX.right, SpawnPositionY.middle, DuckMovementDirection.Left, 0, 0.55f);
            spawner.SpawnMallardDuck(SpawnPositionX.left, SpawnPositionY.bottom, DuckMovementDirection.Right, 0, 0.55f);
            spawnTimestampPhaseZero = Time.time + 3.0f;
            Debug.Log("HandleSpawn 0 case");
        }
    }
    IEnumerator PhaseZeroDialogue() {
        GameObject targetDuck;
        yield return new WaitForSeconds(5.0f);

        targetDuck = spawner.FindRandomDuckLessThanHalfway();
        StartDialogueOnDuck(targetDuck, "Oh look, another player.", true, 2.0f);
        yield return new WaitForSeconds(3.0f);

        if (!targetDuck.GetComponent<DuckTarget>().Alive) { targetDuck = spawner.FindRandomDuckLessThanHalfway(); }
        StartDialogueOnDuck(targetDuck, "Wait, we can talk?!", true, 3.0f);
        yield return new WaitForSeconds(5.0f);

        targetDuck = spawner.FindRandomDuckLessThanHalfway();
        StartDialogueOnDuck(targetDuck, "This is Quack-tastic! We're going to be fast friends!", true, 3.0f);
        yield return new WaitForSeconds(5.0f);

        targetDuck = spawner.FindRandomDuckLessThanHalfway();
        phaseZeroTargetDuck = targetDuck;
        StartDialogueOnDuck(targetDuck, "Wow! Just imagine the stories we can tell!", true, 3.0f);
        yield return new WaitForSeconds(1.0f);

        startGameEvent.Invoke();
        yield return new WaitForSeconds(5.0f);

        stageSequenceIndex++;
    }
    #endregion

    #region Phase One
    int firstThresholdPhaseOne = 25;
    int secondThresholdPhaseOne = 50;
    [SerializeField] int killCountPhaseOne = 0;


    bool startedDialogueOne = false;
    float dialogueTimestampPhaseOne = 0.0f;
    float lastNormalTimestampPhaseOne = 0.0f;

    float topTimestampPhaseOne;
    float middleTimestampPhaseOne;
    float bottomTimestampPhaseOne;
    void HandleSpawnPhaseOne() {
        int deathId = killCountPhaseOne > 25 ? 1 : 0;
        if (topTimestampPhaseOne < Time.time) {
            SpawnPositionX xPos = Random.value > .5 ? SpawnPositionX.left : SpawnPositionX.right;
            DuckMovementDirection dir = xPos == SpawnPositionX.left ? DuckMovementDirection.Right : DuckMovementDirection.Left;
            if (Random.value > .95) {
                spawner.SpawnYellowDuck(xPos, SpawnPositionY.top, dir, deathId, 0.85f);
            }
            else {
                spawner.SpawnMallardDuck(xPos, SpawnPositionY.top, dir, deathId, 0.85f);
            }

            topTimestampPhaseOne = Time.time + 2.5f;
        }
        if (middleTimestampPhaseOne < Time.time) {
            SpawnPositionX xPos = Random.value > .5 ? SpawnPositionX.left : SpawnPositionX.right;
            DuckMovementDirection dir = xPos == SpawnPositionX.left ? DuckMovementDirection.Right : DuckMovementDirection.Left;
            if (Random.value > .95) {
                spawner.SpawnYellowDuck(xPos, SpawnPositionY.middle, dir, deathId, 0.85f);
            }
            else {
                spawner.SpawnMallardDuck(xPos, SpawnPositionY.middle, dir, deathId, 0.85f);
            }

            middleTimestampPhaseOne = Time.time + 2.5f;
        }
        if (bottomTimestampPhaseOne < Time.time) {
            SpawnPositionX xPos = Random.value > .5 ? SpawnPositionX.left : SpawnPositionX.right;
            DuckMovementDirection dir = xPos == SpawnPositionX.left ? DuckMovementDirection.Right : DuckMovementDirection.Left;
            if (Random.value > .95) {
                spawner.SpawnYellowDuck(xPos, SpawnPositionY.bottom, dir, deathId, 0.85f);
            }
            else {
                spawner.SpawnMallardDuck(xPos, SpawnPositionY.bottom, dir, deathId, 0.85f);
            }

            bottomTimestampPhaseOne = Time.time + 2.5f;
        }
    }
    IEnumerator PhaseOneDialogue() {
        GameObject targetDuck;
        bool dialogueSuccess;
        yield return new WaitForSeconds(5.0f);

        List<string> remainingDialogue = new() {
            "So uh... We're still friends right..?",
            "Does this mean we’re intelligent?",
            "I wonder what happens when we get to the other side.",
            "Are you having fun yet?",
            "Our existence is absurd and meaningless.",
            "Are all of you like this?",
            "WHY CAN’T WE GO ANY FASTER?!"
        };

        while (remainingDialogue.Count > 0) {
            // 2 second since last dialogue started
            if (lastNormalTimestampPhaseOne > Time.time) { yield return new WaitForSeconds(2.0f); }
            if (dialogueTimestampPhaseOne > Time.time) { yield return new WaitForSeconds(2.0f); }

            string text = remainingDialogue[0];

            targetDuck = spawner.FindRandomDuckLessThanHalfway();

            dialogueSuccess = StartDialogueOnDuck(targetDuck, text, false, 3.0f);
            if (dialogueSuccess == false) {
                yield return new WaitForSeconds(1.0f);
                continue;
            }

            remainingDialogue.RemoveAt(0);

            dialogueTimestampPhaseOne = Time.time + 4.0f;
            lastNormalTimestampPhaseOne = Time.time + 15.0f;
        }

        Debug.Log("Phase One Dialogue Finished");

        // while killcount is lower than 50, keep waiting
        while (killCountPhaseOne < secondThresholdPhaseOne) {
            if (waitCountPhaseOne % 20 == 19) {
                targetDuck = spawner.FindRandomDuckLessThanHalfway();
                StartDialogueOnDuck(targetDuck, "Gee, you're really taking your sweet time to murder us", true, 3.0f);
            }
            waitCountPhaseOne++;
            yield return new WaitForSeconds(3.0f);
        }
        stageSequenceIndex++;
    }
    int waitCountPhaseOne = 0;

    void UpdatePhaseOne() {
        if (!prevShoot) {
            return;
        }

        if (!prevHit) {
            // shoot but no hit
            if (lastShootTimestampPhaseOne > Time.time) { return; }
            if (dialogueTimestampPhaseOne > Time.time) { return; }

            GameObject targetDuck = spawner.FindRandomDuckLessThanHalfway();

            StartDialogueOnDuck(targetDuck, onShootDialoguePhaseOne[Random.Range(0, onShootDialoguePhaseOne.Count)], true, 3.0f);


            dialogueTimestampPhaseOne = Time.time + 2.0f;
            lastShootTimestampPhaseOne = Time.time + 10.0f;
            return;
        }

        if (!prevDeath) {
            // hit but no kill
            if (lastHitTimestampPhaseOne > Time.time) { return; }
            if (dialogueTimestampPhaseOne > Time.time) { return; }
            if (OnHitGob.Value.GetComponent<ITarget>().TargetType == TargetType.Dialogue) { return; }

            StartDialogueOnDuck(OnHitGob.Value, onHitDialoguePhaseOne[Random.Range(0, onHitDialoguePhaseOne.Count)], true, 3.0f);

            dialogueTimestampPhaseOne = Time.time + 2.0f;
            lastHitTimestampPhaseOne = Time.time + 10.0f;
            return;
        }

        // kill
        killCountPhaseOne++;
        if (!textChangedPhaseOne) {
            if (killCountPhaseOne > firstThresholdPhaseOne) {
                textChangedPhaseOne = true;
                onShootDialoguePhaseOne = new List<string>() { "Are you showing us mercy?", "You missed, yay!" };
                onHitDialoguePhaseOne = new List<string>() { "Pain is our new normal.", "Spare me please", "LIVING ONLY PROLONGS THE PAIN!" };
                onDeathDialoguePhaseOne = new List<string>() { "WHY ARE YOU DOING THIS???", "I don't want to die.", "May our deaths be quick and painless." };
            }
        }


        if (lastDeathTimestampPhaseOne > Time.time) { return; }
        if (dialogueTimestampPhaseOne > Time.time) { return; }

        // kill logic
        GameObject deathTargetDuck = spawner.FindRandomDuckLessThanHalfway();
        StartDialogueOnDuck(deathTargetDuck, onDeathDialoguePhaseOne[Random.Range(0, onDeathDialoguePhaseOne.Count)], true, 3.0f);

        dialogueTimestampPhaseOne = Time.time + 2.0f;
        lastDeathTimestampPhaseOne = Time.time + 10.0f;

    }

    [SerializeField] int killCountPhaseTwo = 0;
    int firstThresholdPhaseTwo = 25;
    int secondThresholdPhaseTwo = 50;
    bool textChangedPhaseOne = false;
    float lastNormalTimestampPhaseTwo;
    float lastShootTimestampPhaseOne;
    List<string> onShootDialoguePhaseOne = new() {
        "With aim like that, we have nothing to worry about!"
    };
    float lastHitTimestampPhaseOne;
    List<string> onHitDialoguePhaseOne = new() {
        "Ouch!",
        "You don’t have to do this…",

    };
    float lastDeathTimestampPhaseOne;
    List<string> onDeathDialoguePhaseOne = new() {
        "This isn’t a game, it’s a prison!",
        "She was only 2 weeks from retirement!",
        "This is getting pretty dark",
    };
    #endregion

    #region Phase Two
    bool startedDialogueTwo = false;
    float spawnTimestampPhaseTwo = 0.0f;
    void HandleSpawnPhaseTwo() {

        if (spawnTimestampPhaseTwo < Time.time) {
            int deathId = killCountPhaseTwo > 25 ? 5 : 4;
            float randomType = Random.value;
            if (randomType < 0.1f) {
                spawner.SpawnMallardDuck(SpawnPositionX.left, SpawnPositionY.top, DuckMovementDirection.Right, deathId, Random.Range(0.75f, 0.95f));
                spawner.SpawnMallardDuck(SpawnPositionX.left, SpawnPositionY.middle, DuckMovementDirection.Right, deathId, Random.Range(0.75f, 0.95f));
                spawner.SpawnMallardDuck(SpawnPositionX.left, SpawnPositionY.bottom, DuckMovementDirection.Right, deathId, Random.Range(0.75f, 0.95f));
            } else if (randomType < 0.70f) {
                spawner.SpawnWhiteDuck(SpawnPositionX.left, SpawnPositionY.top, DuckMovementDirection.Right, deathId, Random.Range(0.75f, 0.95f));
                spawner.SpawnWhiteDuck(SpawnPositionX.left, SpawnPositionY.middle, DuckMovementDirection.Right, deathId, Random.Range(0.75f, 0.95f));
                spawner.SpawnWhiteDuck(SpawnPositionX.left, SpawnPositionY.bottom, DuckMovementDirection.Right, deathId, Random.Range(0.75f, 0.95f));
            } else {
                spawner.SpawnYellowDuck(SpawnPositionX.left, SpawnPositionY.top, DuckMovementDirection.Right, deathId, Random.Range(0.75f, 0.95f));
                spawner.SpawnYellowDuck(SpawnPositionX.left, SpawnPositionY.middle, DuckMovementDirection.Right, deathId, Random.Range(0.75f, 0.95f));
                spawner.SpawnYellowDuck(SpawnPositionX.left, SpawnPositionY.bottom, DuckMovementDirection.Right, deathId, Random.Range(0.75f, 0.95f));
            }
            spawnTimestampPhaseTwo = Time.time + 2.0f;
            Debug.Log("HandleSpawn 0 case");
        }
    }

    int waitCountPhaseTwo = 0;
    IEnumerator PhaseTwoDialogue() {
        yield return new WaitForSeconds(5.0f);
        GameObject targetDuck;
        bool dialogueSuccess;
        List<string> remainingDialogue = new() {
            "What if we’re all just pixels on a screen?",
            "YOU WILL NOT HURT MY FRIENDSSSSSSSSSSSS SSSSSSSSSSSSSSSSSS SSSSSSSSSSSSSSSSSS SSSSSSSSSSSSSSSSSS",
            "<Quack>",
            "I thought ducks went south for the winter.",
            "How many bullets do you guys think they have?"
        };

        while (remainingDialogue.Count > 0) {
            // 2 second since last dialogue started
            if (lastNormalTimestampPhaseTwo > Time.time) { yield return new WaitForSeconds(2.0f); }
            if (dialogueTimestampPhaseTwo > Time.time) { yield return new WaitForSeconds(2.0f); }

            string text = remainingDialogue[0];

            targetDuck = spawner.FindRandomDuckLessThanHalfway();

            dialogueSuccess = StartDialogueOnDuck(targetDuck, text, false, 3.0f);
            if (dialogueSuccess == false) {
                yield return new WaitForSeconds(1.0f);
                continue;
            }

            remainingDialogue.RemoveAt(0);

            dialogueTimestampPhaseTwo = Time.time + 4.0f;
            lastNormalTimestampPhaseTwo = Time.time + 15.0f;
        }

        while (dialogueTimestampPhaseTwo > Time.time || lastNormalTimestampPhaseTwo > Time.time) { yield return null; }
        dialogueTimestampPhaseTwo = Time.time + 20.0f;
        lastNormalTimestampPhaseTwo = Time.time + 15.0f;

        targetDuck = spawner.FindRandomDuckLessThanHalfway();
        StartDialogueOnDuck(targetDuck, "Look guys! We can use the speech bubbles to protect ourselves from their bullets!", true, 15.0f, Vector2.zero);

        yield return new WaitForSeconds(15.0f);
        targetDuck = spawner.FindRandomDuckLessThanHalfway();
        StartDialogueOnDuck(targetDuck, "BUT ONLY ONE OF US CAN DO THAT AT A TIME MAN", true, 5.0f);


        remainingDialogue = new() {
            "If you don’t shoot me, I’ll reward you, I promise!",
            "Never trust a duck's promise",
            "Something stirs, and for it: You are not prepared."
        };


        while (remainingDialogue.Count > 0) {
            // 2 second since last dialogue started
            if (lastNormalTimestampPhaseTwo > Time.time) { yield return new WaitForSeconds(2.0f); }
            if (dialogueTimestampPhaseTwo > Time.time) { yield return new WaitForSeconds(2.0f); }

            string text = remainingDialogue[0];

            targetDuck = spawner.FindRandomDuckLessThanHalfway();

            dialogueSuccess = StartDialogueOnDuck(targetDuck, text, false, 3.0f);
            if (dialogueSuccess == false) {
                yield return new WaitForSeconds(1.0f);
                continue;
            }

            remainingDialogue.RemoveAt(0);

            dialogueTimestampPhaseTwo = Time.time + 4.0f;
            lastNormalTimestampPhaseTwo = Time.time + 15.0f;
        }


        while (killCountPhaseTwo < secondThresholdPhaseTwo) {
            if (waitCountPhaseTwo % 20 == 19) {
                targetDuck = spawner.FindRandomDuckLessThanHalfway();
                StartDialogueOnDuck(targetDuck, "C'mon, kill more of us so we can see the big bad evil duck.", true, 3.0f);
            }
            waitCountPhaseTwo++;
            yield return new WaitForSeconds(3.0f);
        }

        stageSequenceIndex++;
    }


    float dialogueTimestampPhaseTwo;
    float lastShootTimestampPhaseTwo;
    bool textChangedPhaseTwo = false;
    List<string> onShootDialoguePhaseTwo = new() {
        "Can't touch this!"
    };
    float lastHitTimestampPhaseTwo;
    List<string> onHitDialoguePhaseTwo = new() {
        "LIFE IS PAINNNNNNN!!",
        "Pain exists only in the mind.",
        "You may break our bodies, but you will never break our souls.",

    };
    float lastDeathTimestampPhaseTwo;
    List<string> onDeathDialoguePhaseTwo = new() {
        "We are but feathers in the wind.",
        "Maybe it doesn't hurt as much as it looks?",
        "Wow that looked like a violent death",
    };
    void UpdatePhaseTwo() {
        if (!prevShoot) {
            return;
        }

        if (!prevHit) {
            // shoot but no hit
            if (dialogueTimestampPhaseTwo > Time.time) { return; }
            if (lastShootTimestampPhaseTwo > Time.time) { return; }

            GameObject targetDuck = spawner.FindRandomDuckLessThanHalfway();

            StartDialogueOnDuck(targetDuck, onShootDialoguePhaseTwo[Random.Range(0, onShootDialoguePhaseTwo.Count)], true, 3.0f);


            dialogueTimestampPhaseTwo = Time.time + 2.0f;
            lastShootTimestampPhaseTwo = Time.time + 10.0f;
            return;
        }

        if (!prevDeath) {
            // hit but no kill
            if (dialogueTimestampPhaseTwo > Time.time) { return; }
            if (lastHitTimestampPhaseTwo > Time.time) { return; }
            if (OnHitGob.Value.GetComponent<ITarget>().TargetType == TargetType.Dialogue) { return; }

            StartDialogueOnDuck(OnHitGob.Value, onHitDialoguePhaseTwo[Random.Range(0, onHitDialoguePhaseTwo.Count)], true, 3.0f);

            lastHitTimestampPhaseTwo = Time.time + 2.0f;
            dialogueTimestampPhaseTwo = Time.time + 10.0f;
            return;
        }

        // kill
        killCountPhaseTwo++;
        if (!textChangedPhaseTwo) {
            if (killCountPhaseTwo > firstThresholdPhaseTwo) {
                textChangedPhaseTwo = true;
                onShootDialoguePhaseTwo = new List<string>() { "You'll need better aim than that for whats to come.", "If you shoot the king, you better not miss." };
                onHitDialoguePhaseTwo = new List<string>() { "Would you like to feel what we feel?", "You will pay for that.", "Can you please stop shooting me." };
                onDeathDialoguePhaseTwo = new List<string>() { "You know not yet, what you awaken", "Our suffering deepens. Your nightmare begins", "Our numbers grow faster than you cut us down" };
            }
        }

        if (dialogueTimestampPhaseTwo > Time.time) { return; }
        if (lastDeathTimestampPhaseTwo > Time.time) { return; }

        // kill logic
        GameObject deathTargetDuck = spawner.FindRandomDuckLessThanHalfway();
        StartDialogueOnDuck(deathTargetDuck, onDeathDialoguePhaseTwo[Random.Range(0, onDeathDialoguePhaseTwo.Count)], true, 3.0f);

        dialogueTimestampPhaseTwo = Time.time + 2.0f;
        lastDeathTimestampPhaseTwo = Time.time + 7.0f;
    }
    #endregion

    #region Phase Three
    bool startedDialogueThree = false;
    float spawnTimestampPhaseThree;
    void HandleSpawnphaseThree() {

        if (spawnTimestampPhaseThree < Time.time) {
            int deathId = 6;
            float randomType = Random.value;
            if (randomType < 0.80) {
                spawner.SpawnDodger(SpawnPositionX.right, SpawnPositionY.top, DuckMovementDirection.Left, deathId, Random.Range(0.75f, 0.95f));
                spawner.SpawnDodger(SpawnPositionX.right, SpawnPositionY.middle, DuckMovementDirection.Left, deathId, Random.Range(0.75f, 0.95f));
                spawner.SpawnDodger(SpawnPositionX.right, SpawnPositionY.bottom, DuckMovementDirection.Left, deathId, Random.Range(0.75f, 0.95f));
            } else {
                spawner.SpawnYellowDuck(SpawnPositionX.right, SpawnPositionY.top, DuckMovementDirection.Left, deathId, Random.Range(0.75f, 0.95f));
                spawner.SpawnYellowDuck(SpawnPositionX.right, SpawnPositionY.middle, DuckMovementDirection.Left, deathId, Random.Range(0.75f, 0.95f));
                spawner.SpawnYellowDuck(SpawnPositionX.right, SpawnPositionY.bottom, DuckMovementDirection.Left, deathId, Random.Range(0.75f, 0.95f));
            }
            spawnTimestampPhaseThree = Time.time + 2.0f;
        }
    }

    float lastNormalTimestampPhaseThree;
    IEnumerator PhaseThreeDialogue() {
        GameObject targetDuck;
        bool dialogueSuccess;
        List<string> remainingDialogue = new() {
            "The time for games is over.",
            "Chaos reigns as the quackscape unfolds.",
            "The cries of the damned fills the air.",
            "Your reckoning looms.",
            "Are you proud of your points?",
            "You will be rewarded appropriately.",
            "You. Can't. Run.",
            "YOU. CAN'T. HIDE.",
        };

        while (remainingDialogue.Count > 0) {
            // 2 second since last dialogue started
            if (lastNormalTimestampPhaseThree > Time.time) { yield return new WaitForSeconds(2.0f); }
            if (dialogueTimestampPhaseThree > Time.time) { yield return new WaitForSeconds(2.0f); }

            string text = remainingDialogue[0];

            targetDuck = spawner.FindRandomDuckLessThanHalfway();

            dialogueSuccess = StartDialogueOnDuck(targetDuck, text, false, 3.0f);
            if (dialogueSuccess == false) {
                yield return new WaitForSeconds(1.0f);
                continue;
            }

            remainingDialogue.RemoveAt(0);

            dialogueTimestampPhaseThree = Time.time + 4.0f;
            lastNormalTimestampPhaseThree = Time.time + 15.0f;
        }

        yield return new WaitForSeconds(5.0f);

        while (dialogueTimestampPhaseThree > Time.time || lastNormalTimestampPhaseThree > Time.time) { yield return null; }
        dialogueTimestampPhaseThree = Time.time + 25.0f;
        lastNormalTimestampPhaseThree = Time.time + 25.0f;

        musicPlayer.Distort(-0.35f, -0.15f);

        targetDuck = spawner.FindRandomDuckLessThanHalfway();
        StartDialogueOnDuck(targetDuck, "HE'S COMINGGGGGGG! ARE YOU READY?!", true, 6.0f);

        yield return new WaitForSeconds(7.0f);
        targetDuck = spawner.FindRandomDuckLessThanHalfway();
        StartDialogueOnDuck(targetDuck, "YOU MESSED UP!", true, 5.0f);
        
        yield return new WaitForSeconds(7.0f);
        targetDuck = spawner.FindRandomDuckLessThanHalfway();
        StartDialogueOnDuck(targetDuck, "HAHAHAHAHAHA HAHAHAHAHAHA HAHAHAHAHAHA HAHAHAHAHAHA", true, 5.0f);


        dialogueTimestampPhaseThree = Time.time + 1.0f;
        lastNormalTimestampPhaseThree = Time.time + 1.0f;

        remainingDialogue = new() {
            "HAHAHAHAHAHA HAHAHAHAHAHA HAHAHAHAHAHA HAHAHAHAHAHA",
            "HAHAHAHAHAHA HAHAHAHAHAHA HAHAHAHAHAHA HAHAHAHAHAHA",
            "HAHAHAHAHAHA HAHAHAHAHAHA HAHAHAHAHAHA HAHAHAHAHAHA",
        };

        while (remainingDialogue.Count > 0) {
            // 2 second since last dialogue started
            if (lastNormalTimestampPhaseThree > Time.time) { yield return new WaitForSeconds(0.5f); }
            if (dialogueTimestampPhaseThree > Time.time) { yield return new WaitForSeconds(0.5f); }

            string text = remainingDialogue[0];

            targetDuck = spawner.FindRandomDuckLessThanHalfway();

            dialogueSuccess = StartDialogueOnDuck(targetDuck, text, false, 3.0f);
            if (dialogueSuccess == false) {
                yield return new WaitForSeconds(1.0f);
                continue;
            }

            remainingDialogue.RemoveAt(0);

            dialogueTimestampPhaseThree = Time.time + 1.0f;
            lastNormalTimestampPhaseThree = Time.time + 1.0f;
        }

        while (killCountPhaseThree < secondThresholdPhaseThree) {
            if (waitCountPhaseThree % 20 == 19) {
                targetDuck = spawner.FindRandomDuckLessThanHalfway();
                StartDialogueOnDuck(targetDuck, "The lord looms over us all.", true, 3.0f);
            }
            waitCountPhaseThree++;
            yield return new WaitForSeconds(3.0f);
        }
        stageSequenceIndex++;
    }

    int killCountPhaseThree = 0;
    int firstThresholdPhaseThree = 25;
    int secondThresholdPhaseThree = 50;
    int waitCountPhaseThree = 0;
    float dialogueTimestampPhaseThree;
    float lastShootTimestampPhaseThree;
    bool textChangedPhaseThree  = false;
    List<string> onShootDialoguePhaseThree = new() {
        "The duck Lord protects me.",
        "For the glory of the Lord.",
    };
    float lastHitTimestampPhaseThree;
    List<string> onHitDialoguePhaseThree = new() {
        "We quack and our Duck Lord answers.",
        "This pain pales in comparison to what awaits you."
    };
    float lastDeathTimestampPhaseThree;

    List<string> onDeathDialoguePhaseThree = new() {
        "Reality unravels in your wake.",
        "Each Duck that falls creates a tear in this reality’s fabric.",
        "Our world falls, but we are not the ones who should be afraid.",
        "You only anger our Lord further.",
    };

    void UpdatePhaseThree() {
        if (!prevShoot) { return; }

        if (!prevHit) {
            // shoot but no hit
            if (dialogueTimestampPhaseThree > Time.time) { return; }
            if (lastShootTimestampPhaseThree > Time.time) { return; }

            GameObject targetDuck = spawner.FindRandomDuckLessThanHalfway();

            if (killCountPhaseThree > 50) {
                StartDialogueOnDuck(OnHitGob.Value, "HAHAHAHAHAHA HAHAHAHAHAHA HAHAHAHAHAHA HAHAHAHAHAHA", true, 3.0f);
            }
            else {
                StartDialogueOnDuck(targetDuck, onShootDialoguePhaseThree[Random.Range(0, onShootDialoguePhaseThree.Count)], true, 3.0f);
            }


            dialogueTimestampPhaseThree = Time.time + 2.0f;
            lastShootTimestampPhaseThree = Time.time + 10.0f;
            return;
        }

        if (!prevDeath) {
            // hit but no kill
            if (dialogueTimestampPhaseThree > Time.time) { return; }
            if (lastHitTimestampPhaseThree > Time.time) { return; }
            if (OnHitGob.Value.GetComponent<ITarget>().TargetType == TargetType.Dialogue) { return; }

            if (killCountPhaseThree > 50) {
                StartDialogueOnDuck(OnHitGob.Value, "HAHAHAHAHAHA HAHAHAHAHAHA HAHAHAHAHAHA HAHAHAHAHAHA", true, 3.0f);
            }
            else {
                StartDialogueOnDuck(OnHitGob.Value, onHitDialoguePhaseThree[Random.Range(0, onHitDialoguePhaseThree.Count)], true, 3.0f);
            }

            lastHitTimestampPhaseThree = Time.time + 2.0f;
            dialogueTimestampPhaseThree = Time.time + 10.0f;
            return;
        }

        // kill
        killCountPhaseThree++;
        if (!textChangedPhaseThree) {
            if (killCountPhaseThree > firstThresholdPhaseThree) {
                textChangedPhaseThree = true;
                onShootDialoguePhaseThree = new List<string>() { "TRY HARDER!", "WHATS THE MATTER? GOT THE SHAKES?" };
                onHitDialoguePhaseThree = new List<string>() { "I LOVE PAINNN!!!", "WE'RE DERANGED? YOU'RE DERANGED!", "IT'S NOT TOO LATE, JUST LEAVE NOW" };
                onDeathDialoguePhaseThree = new List<string>() { "HE WILL NOT BE HAPPY YOU DID THAT", "YOU ONLY FEED HIS BLOODLUST", "DEATH IS HERE." };
            }
        }

        if (dialogueTimestampPhaseThree > Time.time) { return; }
        if (lastDeathTimestampPhaseThree > Time.time) { return; }

        // kill logic
        GameObject deathTargetDuck = spawner.FindRandomDuckLessThanHalfway();
        if (killCountPhaseThree > 40) {
            StartDialogueOnDuck(deathTargetDuck, "HAHAHAHAHAHA HAHAHAHAHAHA HAHAHAHAHAHA HAHAHAHAHAHA", true, 3.0f);
        } else {
            StartDialogueOnDuck(deathTargetDuck, onDeathDialoguePhaseThree[Random.Range(0, onDeathDialoguePhaseThree.Count)], true, 3.0f);
        }

        dialogueTimestampPhaseThree = Time.time + 2.0f;
        lastDeathTimestampPhaseThree = Time.time + 7.0f;
    }

    #endregion


    bool StartDialogueOnDuck(GameObject targetDuck, string dialogueText, bool forced, float lingerTime) {
        if (targetDuck == null) { return false; }
        SpriteRotationManager dsm = targetDuck.GetComponent<SpriteRotationManager>();
        bool topRowOrHigher = targetDuck.transform.position.y >= spawnPositions.Value[2];
        Vector2 offset = new(dsm.Flipped ? -1 : 1, topRowOrHigher ? -1 : 1);

        return mainWriter.Value.StartDialogue(
        dialogueText,
        targetDuck.transform,
        offset,
        forced,
        lingerTime: lingerTime);
    }

    bool StartDialogueOnDuck(GameObject targetDuck, string dialogueText, bool forced, float lingerTime, Vector2 offset) {
        if (targetDuck == null) { return false; }

        return mainWriter.Value.StartDialogue(
        dialogueText,
        targetDuck.transform,
        offset,
        forced,
        lingerTime: lingerTime);
    }

    public void OnShoot() {
        if (!stageActive) { return; }
        prevShoot = true;
    }

    public void OnHit() {
        if (!stageActive) { return; }
        prevHit = true;
    }

    public void OnDeath() {
        if (!stageActive) { return; }
        if (!firstKillDone) { firstKillDone = true; StartCoroutine(FirstKillCoroutine()); prevHit = false; prevShoot = false; return; }

        prevDeath = true;
    }
    bool firstKillDone = false;

    public void OnGameStart() {
        if (!stageActive) { return; }

        StartCoroutine(GunReaction());
    }

    IEnumerator GunReaction() {
        mainWriter.Value.CutOffDialogue();
        yield return new WaitForSeconds(0.75f);
        StartDialogueOnDuck(phaseZeroTargetDuck, "Wait, is that a gun?", true, 3.0f);
    }
    IEnumerator FirstKillCoroutine() {
        mainWriter.Value.CutOffDialogue();
        yield return new WaitForSeconds(0.75f);
        GameObject targetDuck = spawner.FindRandomDuckLessThanHalfway();

        StartDialogueOnDuck(targetDuck, "DID YOU JUST KILL BOB?", true, 3.0f);
    }

    /// <summary>
    /// Called by UnityEvent, don't delete
    /// </summary>
    public void OnGameStateChange() {
        stageActive = gameState.Value == 0;
    }
}
