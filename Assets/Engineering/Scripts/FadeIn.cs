using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeIn : MonoBehaviour
{
    // Start is called before the first frame update
    Color fadeColor;
    void Start()
    {
        fadeColor = fade.color;
    }

    [SerializeField] Image fade;
    // Update is called once per frame
    float alpha = 1;
    void Update()
    {
        alpha -= Time.deltaTime / 4;
        fadeColor.a = alpha;
        fade.color = fadeColor;
    }
}
