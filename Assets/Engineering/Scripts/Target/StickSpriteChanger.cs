using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickSpriteChanger : MonoBehaviour
{
    [SerializeField] SpriteRotationManager manager;
    [SerializeField] Sprite[] sprites;

    // Start is called before the first frame update
    void Start() {
        int randIndex = Random.Range(0, sprites.Length);
        manager.SetFrontFace(sprites[randIndex]);
        manager.SetBackFace(sprites[randIndex]);
    }

}
