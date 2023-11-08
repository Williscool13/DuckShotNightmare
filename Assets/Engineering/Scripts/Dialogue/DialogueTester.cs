using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTester : MonoBehaviour
{
    [SerializeField] DialogueWriterVariable writer;
    [SerializeField] FloatArrayReference spawnPositions;

    [SerializeField] string dialogueText;
    [SerializeField] bool forced = false;
    [SerializeField] float dialogueTime = 1.0f;

    [SerializeField] string dialogueText2;
    [SerializeField] bool forced2 = false;
    [SerializeField] float dialogueTime2 = 1.0f;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) {
            GameObject[] gobs = GameObject.FindGameObjectsWithTag("Duck");

            Debug.Log($"[Dialogue Tester] Found { gobs.Length } gobs");

            List<GameObject> aliveDucks = new();

            foreach (GameObject gob in gobs) {
                if (gob.GetComponent<DuckTarget>().Alive) {
                    aliveDucks.Add(gob);
                }
            }

            if (aliveDucks.Count > 0) {
                int randDuck = Random.Range(0, aliveDucks.Count);
                GameObject chosenDuck = aliveDucks[randDuck];

                SpriteRotationManager dsm = chosenDuck.GetComponent<SpriteRotationManager>();

                bool topRow = chosenDuck.transform.position.y >= spawnPositions.Value[2];

                Vector2 offset = new(dsm.Flipped ? -1 : 1, topRow ? -1 : 1);

                

                if (writer.Value.StartDialogue(
                dialogueText,
                chosenDuck.transform,
                offset,
                forced,
                lingerTime: dialogueTime)) {
                    Debug.Log("[Dialogue Tester] Started dialogue with living duck");
                }
                else {
                    Debug.Log("[Dialogue Tester] Tried to start dialogue with living duck, but dialogue system already running");
                }
            }
            else {
                Debug.Log("[Dialogue Tester] No ducks on screen");
            }
            
        }

        if (Input.GetKeyDown(KeyCode.W)) {
            GameObject[] gobs = GameObject.FindGameObjectsWithTag("Duck");

            Debug.Log($"[Dialogue Tester] Found { gobs.Length } gobs");

            List<GameObject> aliveDucks = new();

            foreach (GameObject gob in gobs) {
                if (gob.GetComponent<DuckTarget>().Alive) {
                    aliveDucks.Add(gob);
                }
            }

            if (aliveDucks.Count > 0) {
                int randDuck = Random.Range(0, aliveDucks.Count);
                GameObject chosenDuck = aliveDucks[randDuck];

                SpriteRotationManager dsm = chosenDuck.GetComponent<SpriteRotationManager>();

                bool topRow = chosenDuck.transform.position.y >= spawnPositions.Value[2];

                Vector2 offset = new(dsm.Flipped ? -1 : 1, topRow ? -1 : 1);



                if (writer.Value.StartDialogue(
                dialogueText2,
                chosenDuck.transform,
                offset,
                forced2,
                lingerTime: dialogueTime2)) {
                    Debug.Log("[Dialogue Tester] Started dialogue with living duck");
                }
                else {
                    Debug.Log("[Dialogue Tester] Tried to start dialogue with living duck, but dialogue system already running");
                }
            }
            else {
                Debug.Log("[Dialogue Tester] No ducks on screen");
            }

        }
    }
}
