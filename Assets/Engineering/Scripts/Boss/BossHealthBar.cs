using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    float baseWidth;
    void Start()
    {
        baseWidth = bossHealthBar.localScale.x;
    }

    [SerializeField] IntegerReference bossHealth;
    [SerializeField] IntegerReference bossMaxHeaLth;
    [SerializeField] TextMeshPro bossName;
    [SerializeField] Transform bossHealthBar;
    void Update() {
        bool healthActive = bossHealth.Value < bossMaxHeaLth.Value && bossHealth.Value > 0;
        bossName.enabled = healthActive;
        bossHealthBar.gameObject.SetActive(healthActive);

        SetHealthBarWidth((float)bossHealth.Value / (float)bossMaxHeaLth.Value);
    }

    void SetHealthBarWidth(float proportion) {
        float lerpedValue = Mathf.Lerp(bossHealthBar.localScale.x, proportion * baseWidth, Time.deltaTime * 5.0f);
        bossHealthBar.localScale = new Vector2(lerpedValue, bossHealthBar.localScale.y);
    }
}
