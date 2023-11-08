using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ScoreSystem : MonoBehaviour
{
    [SerializeField] TextMeshPro scoreVisualTitle;
    [SerializeField] TextMeshPro scoreVisualValue;
    [SerializeField] TextMeshPro comboVisualTitle;
    [SerializeField] TextMeshPro comboVisualValue;

    [SerializeField] IntegerVariable comboValue;
    [SerializeField] IntegerVariable scoreValue;
    [SerializeField] IntegerReference maxAmmo;
    [SerializeField] GameObjectReference killedUnit;
    [SerializeField] GameObjectReference hitUnit;

    int score = 0;
    int combo = 1;
    bool prevShoot = false;
    bool prevHit = false;
    bool prevReload = false;
    bool duckKill = false;
    bool bossWeakpointHit = false;
    DuckType duckKillType = DuckType.Mallard;

    private void Start() {
        comboValue.Value = 0;
        scoreValue.Value = 0;
    }
    void Update() {
        scoreVisualTitle.enabled = score > 0;
        scoreVisualValue.enabled = score > 0;
        comboVisualTitle.enabled = combo > 1;
        comboVisualValue.enabled = combo > 1;

        if (prevShoot) {
            if (prevHit) {
                if (duckKill) {
                    Debug.Log("Duck killed!");
                    score += 3 * combo;
                    combo += 2;
                }
                else if (bossWeakpointHit) {
                    Debug.Log("Weakpoint Killed!");
                    score += 3 * combo;
                    combo += 2;
                }
                else {
                    score += 1 * combo;
                    combo += 1;
                }
            } else {
                // miss shot
                combo = Mathf.Max(1, combo - Mathf.Max(combo / 2, 6));
            }
        }


        if (prevReload) {
            prevReload = false;
            combo = Mathf.Max(1, combo - Mathf.Max(combo / 2, 6));
        }

        scoreVisualValue.text = score.ToString();
        comboVisualValue.text = combo.ToString() + "x";
        scoreValue.Value = score;
        comboValue.Value = combo;

        prevShoot = false;
        prevHit = false;
        duckKill = false;
        bossWeakpointHit = false;
    }


    public void OnShoot() {
        prevShoot = true;
    }

    public void OnHit() {
        ITarget unit = hitUnit.Value.GetComponent<ITarget>();
        if (unit == null) { return; }

        if (unit.TargetType == TargetType.Duck || unit.TargetType == TargetType.BossDuck) {
            prevHit = true;
        }
    }

    public void OnReload() {
        prevReload = true;
    }


    public void OnDuckDeath() {
        IDuck duck = killedUnit.Value.GetComponent<IDuck>();
        if (duck == null) { return; }

        duckKill = true;
        duckKillType = duck.DuckType;
    }

    public void OnBossWeakpointHit() {
        Debug.Log("Score system got the message");
        bossWeakpointHit = true;
    }
}
