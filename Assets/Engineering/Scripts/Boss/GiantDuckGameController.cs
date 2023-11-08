using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GiantDuckGameController : MonoBehaviour
{
    [SerializeField] SpriteRotationManager thisRotationManager;
    [SerializeField] GiantDuckMovement thisGiantDuckMovement;
    [SerializeField] DuckSounds soundMaker;
    [SerializeField] Transform dialogueTarget;

    [SerializeField] CameraReference mainCam;
    [SerializeField] DialogueWriterVariable mainWriter;


    [SerializeField] AudioClip bossMusic;

    float[] xMapBounds = new float[] { -8, 8 };

    GameMusicManager musicManager;
    Camera mainCamera;
    CameraShaker mainCameraShaker;
    DuckSpawner spawner;

    [SerializeField] GameObject bossHealthBarPrefab;
    // Start is called before the first frame update
    void Start()
    {
        quackTimestamp = Time.time;
        musicManager = GameObject.FindObjectOfType<GameMusicManager>();
        mainCamera = mainCam.Value;
        mainCameraShaker = mainCamera.GetComponent<CameraShaker>();
        spawner = GameObject.FindObjectOfType<DuckSpawner>();

        GameObject.Instantiate(bossHealthBarPrefab);
    }

    float quackTimestamp;

    bool giantDuckActive = false;
    // Update is called once per frame
    void Update()
    {
        // quacking
        if (quackTimestamp <= Time.time) {
            if (!gameOver) {
                QuackNoise();
            }
        }

        // Wait for all ducks to exit screen 
        if (!giantDuckActive) {
            PreActiveCheck();
            return;
        }
        // Cool entrance
        if (!entranceFinished) {
            CoolEntrance();
            return;
        }

        if (!introMonologueFinished) {
            return;
        }

        if (!gameOver && !playerDead) {
            Debug.Log("Battle is ongoing");
            // movement protocol
            MainEncounterLogic();
            MainDialogueLogic();

            // duck spawn protocol
        } else {
            Debug.Log("Game is over according to giant duck");
        }
        // entrance finished, boss fight ongoing


    }

    void QuackNoise() {
        if (!giantDuckActive) {
            soundMaker.Quack(Random.Range(-0.9f, -0.7f), Random.Range(-1, 1));
            quackTimestamp = Time.time + Random.Range(2.0f, 3.0f);
        } else {
            soundMaker.Quack(Random.Range(-0.8f, -0.6f), 0);
            quackTimestamp = Time.time + Random.Range(5.0f, 7.0f);
        }
    }


    public void Hit() { 
        InterruptMonologue();

        if (introMonologueFinished && !gameOver) {
            // gameplay responses
            if (encounterOverallDialogueTimestamp > Time.time) { return; }
            if (encounterBossHitDialogueTimestamp > Time.time) { return; }

            string text = bossHitDialogue[Random.Range(0, bossHitDialogue.Count)];
            mainWriter.Value.StartDialogue(text, dialogueTarget, new Vector2(0, 1), true, lingerTime: 4.0f);


            encounterOverallDialogueTimestamp = Time.time + 4.0f;
            encounterBossHitDialogueTimestamp = Time.time + 15.0f;
        }
    }
    bool firstWeakpointHit = false;
    public void WeakpointHit() { 
        InterruptMonologue();

        weakpointHit += 1;

        if (introMonologueFinished && !gameOver) {
            // gameplay responses
            if (!firstWeakpointHit) {
                mainWriter.Value.StartDialogue("WHAT? WHY DID THAT HIT SO HARD?", dialogueTarget, new Vector2(0, 1), true, lingerTime: 4.0f);
                firstWeakpointHit = true;
            } else {
                if (encounterOverallDialogueTimestamp > Time.time) { return; }
                if (encounterBossWeakpointHitDialogueTimestamp > Time.time) { return; }

                string text = bossWeakpointHitDialogue[Random.Range(0, bossWeakpointHitDialogue.Count)];
                mainWriter.Value.StartDialogue(text, dialogueTarget, new Vector2(0, 1), true, lingerTime: 4.0f);


                encounterOverallDialogueTimestamp = Time.time + 4.0f;
                encounterBossWeakpointHitDialogueTimestamp = Time.time + 15.0f;
            }
        }
    }

    [SerializeField] GameObjectReference gunHitTarget;
    public void OnGunHit() {
        if (gunHitTarget.Value.GetComponent<ITarget>().TargetType != TargetType.Dialogue) {
            InterruptMonologue();
        }

        if (introMonologueFinished && !gameOver) {
            ITarget target = gunHitTarget.Value.GetComponent<ITarget>();
            // gameplay responses
            if (target.TargetType == TargetType.Duck) {
                if (!target.Alive) {
                    // if kill a duck
                    SpawnWeakpointAtRandom();

                    if (encounterOverallDialogueTimestamp > Time.time) { return; }
                    if (encounterDuckHitDialogueTimestamp > Time.time) { return; }

                    string text = duckHitDialogue[Random.Range(0, duckHitDialogue.Count)];
                    mainWriter.Value.StartDialogue(text, dialogueTarget, new Vector2(0, 1), true, lingerTime: 4.0f);

                    encounterOverallDialogueTimestamp = Time.time + 4.0f;
                    encounterDuckHitDialogueTimestamp = Time.time + 15.0f;
                }
                
            }
        }
    }

    bool goingRight = false;
    void MainEncounterLogic() {
        if (mainEncounterCoroutine == null) {
            if (goingRight) {
                mainEncounterCoroutine = BossMovementCycleTwo();
                goingRight = !true;
            }
            else {
                mainEncounterCoroutine = BossMovementCycleOne();
                goingRight = true;
            }
            StartCoroutine(mainEncounterCoroutine);
        }
    }

    List<string> mainDialogues = new() {
        "Our feathers will be your DOOM!",
        "You're at the end of the line.",
        "All the suffering you have caused, I will double it.",
        "Are you afraid?",
        "You won't have to worry much longer."
    };
    List<string> duckHitDialogue = new() { 
        "NO! Stop hurting them!",
        "NOT LITTLE BOBBY!",
        "A lot of these ducks you are murdering are only a few days from retirement.",
        "Pick on someone your own size!",
        "So that it, huh? Shoot little ducks because it makes you feel better?"
    };
    List<string> bossHitDialogue = new() { 
        "Your bullets do nothing to me.",
        "Your efforts are futile, hunter."
    };
    List<string> bossWeakpointHitDialogue = new() { 
        "HEY! STOP SHOOTING THOSE"
    };

    float encounterOverallDialogueTimestamp;
    float encounterMainDialogueTimestamp;
    //float encounterMissDialogueTimestamp;
    float encounterDuckHitDialogueTimestamp;
    float encounterBossHitDialogueTimestamp;
    float encounterBossWeakpointHitDialogueTimestamp;

    int mainDialogueIndex = 0;
    void MainDialogueLogic() {
        if (encounterOverallDialogueTimestamp > Time.time) { return; }
        if (encounterMainDialogueTimestamp > Time.time) { return; }

        string text;
        if (mainDialogueIndex >= mainDialogues.Count) {
            text = mainDialogues[Random.Range(0, mainDialogues.Count)];
        } else {
            text = mainDialogues[mainDialogueIndex];
            mainDialogueIndex++;
        }

        mainWriter.Value.StartDialogue(text, dialogueTarget, new Vector2(0, 1), true, lingerTime: 4.0f);


        encounterOverallDialogueTimestamp = Time.time + 4.0f;
        encounterMainDialogueTimestamp = Time.time + 15.0f;
    }

    IEnumerator mainEncounterCoroutine;
    List<GameObject> activeDucks = new();
    // 4 ducks buzzing around the boss in a circular pattern. 1x gold (2hp), 3x mallard - not too fast movement
    IEnumerator BossMovementCycleOne() {
        weakpointHit = 0;
        thisGiantDuckMovement.LaunchNormal(new Vector3(xMapBounds[1], 0, 0), DuckMovementDirection.Left, 0.5f);
        thisRotationManager.SetFlipSpriteX(true);
        // idle for a few seconds and if all accomodating ducks/targets no hit, damage player
        GameObject duck1 = spawner.SpawnGoldenRotator(0.0f, DuckMovementDirection.Left, 6, 20.0f);
        GameObject duck5 = spawner.SpawnRotator(45.0f, DuckMovementDirection.Left, 6, 20.0f);
        GameObject duck2 = spawner.SpawnRotator(90.0f, DuckMovementDirection.Left, 6, 20.0f);
        GameObject duck6 = spawner.SpawnRotator(135.0f, DuckMovementDirection.Left, 6, 20.0f);
        GameObject duck3 = spawner.SpawnGoldenRotator(180.0f, DuckMovementDirection.Left, 6, 20.0f);
        GameObject duck7 = spawner.SpawnRotator(225.0f, DuckMovementDirection.Left, 6, 20.0f);
        GameObject duck4 = spawner.SpawnRotator(270.0f, DuckMovementDirection.Left, 6, 20.0f);
        GameObject duck8 = spawner.SpawnRotator(315.0f, DuckMovementDirection.Left, 6, 20.0f);

        activeDucks.Add(duck1);
        activeDucks.Add(duck2);
        activeDucks.Add(duck3);
        activeDucks.Add(duck4);
        activeDucks.Add(duck5);
        activeDucks.Add(duck6);
        activeDucks.Add(duck7);
        activeDucks.Add(duck8);

        while (transform.position.x > 0) { yield return null; }

        thisGiantDuckMovement.HaltMovement();

        yield return new WaitForSeconds(10.0f);

        if (weakpointHit < 5) {
            Debug.Log("Weakpoints hit (didnt meet 4 threshold): " + weakpointHit);
            float wait = DamagePlayer();
            yield return new WaitForSeconds(wait);
        }

        thisGiantDuckMovement.LaunchNormal(transform.position, DuckMovementDirection.Left, 0.5f);

        while (!thisGiantDuckMovement.IsOffScreen()) { yield return null; }

        for (int i = activeDucks.Count - 1; i >= 0; i--) {
            if (activeDucks[i] != null) {
                Destroy(activeDucks[i]);
            }
            activeDucks.RemoveAt(i);
        }

        yield return new WaitForSeconds(2.0f);
        mainEncounterCoroutine = null;
    }

    int weakpointHit = 0;
    // constantly spawn ducks to go left and right of the screen, they move rather quickly. THey are dodgers but only have 1 hp
    IEnumerator BossMovementCycleTwo() {
        weakpointHit = 0;
        thisGiantDuckMovement.LaunchNormal(new Vector3(xMapBounds[0], 0, 0), DuckMovementDirection.Right, 0.5f);
        thisRotationManager.SetFlipSpriteX(false);



        spawner.SpawnMallardOneHP(SpawnPositionX.right, SpawnPositionY.top, DuckMovementDirection.Left, 6, 1.7f);
        spawner.SpawnMallardOneHP(SpawnPositionX.right, SpawnPositionY.middle, DuckMovementDirection.Left, 6, 1.7f);
        spawner.SpawnMallardOneHP(SpawnPositionX.right, SpawnPositionY.bottom, DuckMovementDirection.Left, 6, 1.7f);

        while (transform.position.x < 0) { 
            yield return new WaitForSeconds(1.0f);
            spawner.SpawnMallardOneHP(SpawnPositionX.right, SpawnPositionY.top,     DuckMovementDirection.Left, 6, 1.7f);
            spawner.SpawnMallardOneHP(SpawnPositionX.right, SpawnPositionY.middle,  DuckMovementDirection.Left, 6, 1.7f);
            spawner.SpawnMallardOneHP(SpawnPositionX.right, SpawnPositionY.bottom,  DuckMovementDirection.Left, 6, 1.7f);
        }

        thisGiantDuckMovement.HaltMovement();

        yield return new WaitForSeconds(10.0f);

        if (weakpointHit < 5) {
            Debug.Log("Weakpoints hit (didnt meet 4 threshold): " + weakpointHit);
            float wait = DamagePlayer();
            yield return new WaitForSeconds(wait);
        }


        thisGiantDuckMovement.LaunchNormal(transform.position, DuckMovementDirection.Right, 0.5f);

        while (!thisGiantDuckMovement.IsOffScreen()) { yield return null; }


        yield return new WaitForSeconds(2.0f);
        mainEncounterCoroutine = null;
    }

    List<BossWeakpointTarget> activeTargets = new();
    public void RemoveWeakpointFromActiveTargets(BossWeakpointTarget target) {
        if (activeTargets.Contains(target)) {
            activeTargets.Remove(target);
        }
        else {
            Debug.LogError("Active targets doesnt contain target specified");
        }
    }
    [SerializeField] GameObject weakpointPrefab;
    [SerializeField] Transform modelTransform;
    [SerializeField] Transform[] weakpointPositions;
    int weakpointSpawnIndex = 0;
    void SpawnWeakpointAtRandom() {
        if (activeTargets.Count >= 4) { return; }
        if (activeTargets.Count == 0) { weakpointSpawnIndex = Random.Range(0, weakpointPositions.Length); }
        GameObject gob = Instantiate(weakpointPrefab, modelTransform);
        gob.transform.position = weakpointPositions[weakpointSpawnIndex].transform.position;
        BossWeakpointTarget bwt = gob.GetComponent<BossWeakpointTarget>();
        bwt.Setup(this);
        activeTargets.Add(bwt);
        weakpointSpawnIndex = (weakpointSpawnIndex + 1) % weakpointPositions.Length;
    }
    #region Entrance
    void PreActiveCheck() {
        GameObject[] ducks = GameObject.FindGameObjectsWithTag("Duck");
        if (ducks.Length > 0) { return; }

        // then fade out the music
        giantDuckActive = true;
        musicManager?.FadeoutClip(1f);
        thisGiantDuckMovement.LaunchNormal(new Vector3(xMapBounds[0], 0, 0), DuckMovementDirection.Right, 0.5f);
    }

    bool entranceFinished;
    void CoolEntrance() {
        if (!mainCameraShaker.IsShaking()) {
            mainCameraShaker.StartShake(0.2f, 0.5f, 1);
        }

        if (transform.position.x < 0) { return; }

        // then screen shake and the duck enters from sreen left, very slowly and menacingly.
        mainCameraShaker.StopShake();
        thisGiantDuckMovement.HaltMovement();
        // then stop movement. And begin monologue

        introMonologueCoroutine = IntroMonologue();
        StartCoroutine(introMonologueCoroutine);
        entranceFinished = true;
    }
    #endregion

    #region Monologue
    IEnumerator introMonologueCoroutine;
    bool introMonologueFinished = false;
    bool monologueInterrupted = false;
    bool monologueExiting = false;
    IEnumerator IntroMonologue() {
        yield return null;

        mainCameraShaker.StopShake();
        mainCameraShaker.StartShake(0.2f, 1.0f, 1);

        mainWriter.Value.StartDialogue("<Quack>", dialogueTarget, new Vector2(1, -1), true, lingerTime: 4.0f);
        yield return new WaitForSeconds(3.0f);
        mainWriter.Value.StartDialogue("<Quack>< Quack>", dialogueTarget, new Vector2(1, -1), true, lingerTime: 4.0f);
        yield return new WaitForSeconds(3.0f);
        mainWriter.Value.StartDialogue("Your reign of terror is at an end, hunter.", dialogueTarget, new Vector2(1, -1), true, lingerTime: 4.0f);
        yield return new WaitForSeconds(4.5f);

        mainWriter.Value.StartDialogue("I have risen from the depths of your darkest nightmare.", dialogueTarget, new Vector2(1, -1), true, lingerTime: 4.0f);
        yield return new WaitForSeconds(4.5f);
        mainWriter.Value.StartDialogue("Goodbye.", dialogueTarget, new Vector2(1, -1), true, lingerTime: 4.0f);
        yield return new WaitForSeconds(4.5f);

        // move to end
        thisGiantDuckMovement.LaunchNormal(transform.position, DuckMovementDirection.Right, 0.5f);
        monologueExiting = true;

        musicManager?.FadeInClip(bossMusic, 0.5f);
        musicManager?.Distort(-0.3f, 0.3f);


        while (!thisGiantDuckMovement.IsOffScreen()) {
            yield return null;
        }


        encounterOverallDialogueTimestamp = Time.time + 2.0f;
        encounterMainDialogueTimestamp = Time.time;
        encounterDuckHitDialogueTimestamp = Time.time + 5.0f;
        encounterBossHitDialogueTimestamp = Time.time + 5.0f;
        encounterBossWeakpointHitDialogueTimestamp = Time.time + 5.0f;

        introMonologueFinished = true;
    }
    IEnumerator IntroMonologueInterrupt() {
        yield return null;
        // start new dialogue
        mainWriter.Value.StartDialogue("You lack honor.", dialogueTarget, new Vector2(1, -1), true, lingerTime: 4.0f);
        yield return new WaitForSeconds(4.0f);

        mainWriter.Value.StartDialogue("Your demise is well deserved.", dialogueTarget, new Vector2(1, -1), true, lingerTime: 4.0f);
        yield return new WaitForSeconds(4.0f);

        mainWriter.Value.StartDialogue("Allow me to teach you some manners.", dialogueTarget, new Vector2(1, -1), true, lingerTime: 4.0f);
        yield return new WaitForSeconds(4.0f);


        // damage the player
        float shakeDuration = DamagePlayer();
        yield return new WaitForSeconds(shakeDuration);

        // move to end
        thisGiantDuckMovement.LaunchNormal(transform.position, DuckMovementDirection.Right, 1.0f);
        monologueExiting = true;

        musicManager?.FadeInClip(bossMusic, 0.5f);
        musicManager?.Distort(-0.3f, 0.3f);

        while (!thisGiantDuckMovement.IsOffScreen()) {
            yield return null;
        }


        encounterOverallDialogueTimestamp = Time.time + 2.0f;
        encounterMainDialogueTimestamp = Time.time;
        encounterDuckHitDialogueTimestamp = Time.time + 5.0f;
        encounterBossHitDialogueTimestamp = Time.time + 5.0f;
        encounterBossWeakpointHitDialogueTimestamp = Time.time + 5.0f;

        introMonologueFinished = true;
    }
    void InterruptMonologue() {
        if (introMonologueFinished == false && entranceFinished == true && !monologueInterrupted && !monologueExiting) {
            // stop intro coroutine.
            StopCoroutine(introMonologueCoroutine);
            introMonologueCoroutine = IntroMonologueInterrupt();
            StartCoroutine(introMonologueCoroutine);
            monologueInterrupted = true;
            return;
        }
    }
    #endregion

    int playerDamageCount = 0;
    bool playerDead = false;
    float DamagePlayer() {
        if (playerDead) { return 0.1f; }
        if (playerDamageCount >= 10) { FinalDamage(); playerDead = true; return 0.1f; }
        float duration = 1.0f + playerDamageCount * 0.2f;
        float intensity = 0.1f + playerDamageCount * 0.05f;
        // play a sound to demonstrate player getting hit
        mainCameraShaker.StopShake();
        mainCameraShaker.RedShake(intensity, duration, 1);
        playerDamageCount += 2;
        return duration;
    }

    void FinalDamage() {
        mainWriter.Value.CutOffDialogue();
        mainCameraShaker.StopShake();
        if (mainEncounterCoroutine != null) {
            StopCoroutine(mainEncounterCoroutine);
        }
        if (introMonologueCoroutine != null) {
            StopCoroutine(introMonologueCoroutine);
        }
        foreach (GameObject gob in activeDucks) { if (gob != null) { Destroy(gob); } }
        foreach (BossWeakpointTarget bwt in activeTargets) { Destroy(bwt.gameObject); }
        foreach (GameObject gob in GameObject.FindGameObjectsWithTag("Duck")) { Destroy(gob); }

        thisGiantDuckMovement.HaltMovement();
        musicManager.FadeoutClip(0.1f);
        mainWriter.Value.StartDialogue("Good riddance.", dialogueTarget, new Vector2(0, 1), true, lingerTime: 4.0f);
        GameObject.FindObjectOfType<GameTransitioner>().EndGameTransitionLose();
        //mainCameraShaker.StartShake(0.1f + 0.05f * playerDamageCount, 1.0f + playerDamageCount * 0.2f, 1);
    }

    bool gameOver = false;
    public void OnGameOver() {
        gameOver = true;
        mainWriter.Value.CutOffDialogue();
        if (mainEncounterCoroutine != null) {
            StopCoroutine(mainEncounterCoroutine);
        }
        if (introMonologueCoroutine != null) {
            StopCoroutine(introMonologueCoroutine);
        }
        foreach (GameObject gob in activeDucks) { if (gob != null) { Destroy(gob); } }
        foreach (BossWeakpointTarget bwt in activeTargets) { Destroy(bwt.gameObject); }
        foreach (GameObject gob in GameObject.FindGameObjectsWithTag("Duck")) { Destroy(gob); }

        thisGiantDuckMovement.HaltMovement();
        musicManager.FadeoutClip(0.1f);
        mainWriter.Value.StartDialogue("NOOOOOOOOOOO OOOOOOOOOOOO OOOOOOOOOOOO OOOOOOOOOOOO", dialogueTarget, new Vector2(1, -1), true, lingerTime: 4.0f);
        GameObject.FindObjectOfType<GameTransitioner>().EndGameTransitionWin();
    }
}
