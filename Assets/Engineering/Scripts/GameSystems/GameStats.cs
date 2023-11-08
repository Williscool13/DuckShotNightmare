using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStats : MonoBehaviour
{

    [SerializeField] IntegerVariable shotCountVar;
    [SerializeField] IntegerVariable hitCountVar;
    [SerializeField] IntegerVariable killCountVar;
    [SerializeField] IntegerVariable highestComboCountVar;
    [SerializeField] IntegerVariable scoreCountVar;


    [SerializeField] IntegerVariable currComboCountVar;
    [SerializeField] IntegerVariable currScoreCountVar;

    private void Start() {
        shotCountVar.Value = 0;
        hitCountVar.Value = 0;
        killCountVar.Value = 0;
        highestComboCountVar.Value = 0;
        scoreCountVar.Value = 0;
    }

    private void Update() {
        if (currComboCountVar.Value > highestComboCountVar.Value) {
            highestComboCountVar.Value = currComboCountVar.Value;
        }
        scoreCountVar.Value = currScoreCountVar.Value;
    }

    int shotCount = 0;
    int hitCount = 0;
    int killCount = 0;
    public void OnShot() {
        shotCount++;
        shotCountVar.Value = shotCount;
    }

    public void OnHit() {
        hitCount++;
        hitCountVar.Value = hitCount;
    }

    public void OnKill() {
        killCount++;
        killCountVar.Value = killCount;
    }

}
