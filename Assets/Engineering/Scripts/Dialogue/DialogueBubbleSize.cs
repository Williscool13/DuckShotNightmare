using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DialogueBubbleSize : MonoBehaviour
{
    [SerializeField] TextMeshPro textComponent;  // Reference to the UI Text element.
    [SerializeField] float xPadding = 5.0f; // Padding to add around the text.
    [SerializeField] float yPadding = 5.0f; // Padding to add around the text.
    [SerializeField] float lerpSpeed = 5f; // Speed of the lerp transition.

    [SerializeField] SpriteRenderer bubbleSpriteRenderer;
    private Vector3 targetScale;

    private void Start() {
        // Get the SpriteRenderer component of the chat bubble (the parent).
        //bubbleSpriteRenderer = GetComponent<SpriteRenderer>();
        targetScale = bubbleSpriteRenderer.transform.localScale;

    }

    public void UpdateBubbleSize() {
        // Calculate the preferred size of the TextMeshProUGUI text.
        float textWidth = textComponent.preferredWidth;
        float textHeight = textComponent.preferredHeight;

        Vector3 bubbleSize;
        // Add padding to the calculated size.
        if (textWidth == 0 && textHeight == 0) {
            bubbleSize = Vector3.zero;
        } else {
            bubbleSize = new Vector3(textWidth + xPadding, textHeight + yPadding, 1f);  
        }

        // Set the target scale of the chat bubble.
        targetScale = bubbleSize;
    }

    private void Update() {
        UpdateBubbleSize();
        // Smoothly lerp the scale of the chat bubble towards the target scale.
        Vector3 currScale = bubbleSpriteRenderer.transform.localScale;
        if (Mathf.Approximately(targetScale.x, 0) && Mathf.Approximately(targetScale.y, 0) && Mathf.Approximately(currScale.x, 0) && Mathf.Approximately(currScale.y, 0)) { return; }
        bubbleSpriteRenderer.transform.localScale = Vector3.Lerp(bubbleSpriteRenderer.transform.localScale, targetScale, Time.deltaTime * lerpSpeed);
    }
}
