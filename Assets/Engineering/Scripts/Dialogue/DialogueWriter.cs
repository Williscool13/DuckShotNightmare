using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DialogueWriter : MonoBehaviour
{
    [SerializeField] TextMeshPro mainText;
    [SerializeField] int maxCharPerRow;
    
    [SerializeField] DialogueWriterVariable mainDialogueWriter;
    [SerializeField] DialogueBubbleTracker thisBubbleTracker;
    [SerializeField] GameObject thisDialogueBubble;
    float charPrintCooldown = 0.04f;
    float spacePrintCooldown = 0.08f;
    float lingerTime = 0.0f;

    GameObject followedGob = null;
    bool dialogueWriting = false;
    bool dialogueLingering = false;
    string[] textArray;
    string currWord;
    int currLetterWordIndex = -1;
    int currRowCount;
    int currTextArrayIndex = 0;
    float printTimestamp = float.MinValue;

    float lingerTimestamp = float.MinValue;
    private void Start() {

        if (mainDialogueWriter.Value != null) { Destroy(this.gameObject); return; }

        mainDialogueWriter.Value = this;
    }
    // Update is called once per frame
    void Update()
    {
        if (dialogueWriting) {
            if (printTimestamp < Time.time) {
                GenerateText();
            }
        } else if (dialogueLingering) {
            if (lingerTimestamp < Time.time) {
                mainText.text = "";
                lingerTimestamp = float.MinValue;
                dialogueLingering = false;
                followedGob = null;
                DestroyAllBulletHoles();
            }
        } else {
            DestroyAllBulletHoles();
        }
    }

    void GenerateText() {
        string finalAppend = "";
        float _printCooldown;
        if (currLetterWordIndex + 1 >= currWord.Length) {
            // move on to next word
            currTextArrayIndex++;
            if (currTextArrayIndex == textArray.Length) { 
                dialogueWriting = false; 
                lingerTimestamp = Time.time + lingerTime; 
                dialogueLingering = true; 
                return; }

            currWord = textArray[currTextArrayIndex];
            if (currWord.Length > maxCharPerRow) { Debug.LogError($"Word: {currWord} is longer than you allow per row"); }
            // 1 is a space
            if (1 + currWord.Length + currRowCount > maxCharPerRow) {
                finalAppend += "\n";
                currRowCount = 0;
            } else {
                finalAppend += " ";
            }

            _printCooldown = spacePrintCooldown;
            currLetterWordIndex = -1;
        } else {
            _printCooldown = charPrintCooldown;

            currLetterWordIndex++;
            currRowCount++;
            char curr = currWord[currLetterWordIndex];
            finalAppend += curr;


        }
        printTimestamp = Time.time + _printCooldown;

        mainText.text += finalAppend;
    }

    void DestroyAllBulletHoles() {
        if (thisDialogueBubble.transform.childCount == 0) { return;}
        foreach (Transform tf in thisDialogueBubble.transform) {
            Destroy(tf.gameObject);
        }
    }

    void StopDialogue() {
        thisBubbleTracker.StopFollowing();
    }
    

    [SerializeField] GameObjectReference duckDeathObject;
    public void OnDuckDeath() {
        if (duckDeathObject.Value == followedGob) {
            mainText.text = "";
            lingerTimestamp = float.MinValue;
            dialogueWriting = false;
            dialogueLingering = false;
            followedGob = null;
            thisBubbleTracker.StopFollowing();
            DestroyAllBulletHoles();
        }
    }

    /// <summary>
    /// Returns true if the dialogue was started (only relevant if forced=false)
    /// </summary>
    /// <param name="text"></param>
    /// <param name="target"></param>
    /// <param name="bubbleOffset"></param>
    /// <param name="forced"></param>
    /// <returns></returns>
    public bool StartDialogue(string text, Transform target, Vector2 bubbleOffset, bool forced, float charPrintCooldown = 0.04f, float spacePrintCooldown = 0.08f, float lingerTime = 1.0f) {

        if ((dialogueWriting || dialogueLingering) && !forced) { Debug.Log("Dialogue system is currently writing already"); return false; }
        Debug.Log("Triggered dialogue with text " + text);
        lingerTimestamp = float.MinValue;
        dialogueLingering = false;
        DestroyAllBulletHoles();

        textArray = text.Split(' ');
        currTextArrayIndex = 0;
        currRowCount = 0;
        currLetterWordIndex = -1;
        currWord = textArray[currTextArrayIndex];

        mainText.text = "";

        followedGob = target.gameObject;
        thisBubbleTracker.Follow(target, bubbleOffset, 10.0f);

        dialogueWriting = true;

        this.charPrintCooldown = charPrintCooldown;
        this.spacePrintCooldown = spacePrintCooldown;
        this.lingerTime = lingerTime;


        return true;
    }

    public void CutOffDialogue() {
        if (mainText.text == "") { return; }
        mainText.text = mainText.text + "-";
        dialogueWriting = false;
        lingerTimestamp = Time.time + lingerTime;
        dialogueLingering = true;
    }
}
