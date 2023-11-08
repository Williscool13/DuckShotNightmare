using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanTexureMPBAdjuster : MonoBehaviour
{
    [SerializeField] Vector2 panDir;
    [SerializeField] Renderer thisRenderer;

    void OnValidate() {
        thisRenderer.sharedMaterial.SetVector("PanVector2", panDir);

    }
}
