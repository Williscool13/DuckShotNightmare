using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletVisual : MonoBehaviour
{
    [SerializeField] Image[] bullets;
    [SerializeField] Sprite silverBullet;
    [SerializeField] Sprite goldBullet;
    [SerializeField] IntegerReference gunBullets;
    [SerializeField] BooleanReference goldenBullets;

    int prevBulletCount = 0;
    bool prevGold = false;
    void Update()
    {
        if (!gunActive) { return; }
        if (prevBulletCount == gunBullets.Value && prevGold == goldenBullets.Value) { return; }

        for (int i = 0; i < gunBullets.Value; i++) {
            bullets[i].enabled = true;
        }

        for (int i = gunBullets.Value; i < bullets.Length; i++) {
            bullets[i].enabled = false;
        }

        Sprite targetSprite = goldenBullets.Value ? goldBullet : silverBullet;
        foreach(Image im in bullets) {
            im.sprite = targetSprite;   
        }
        

        prevBulletCount = gunBullets.Value;
        prevGold = goldenBullets.Value;
    }


    bool gunActive = false;
    public void GameStart() {
        gunActive = true;
    }
}
