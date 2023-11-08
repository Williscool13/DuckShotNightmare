using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DuckTarget : MonoBehaviour, ITarget, IHealth, IDuck {
    [SerializeField] Animator duckAnimator;
    [SerializeField] Collider2D mainCollider;
    [SerializeField] AudioSource duckSource;
    [SerializeField] SpriteRotationManager thisDuckSpriteManager;
    [SerializeField] DuckSounds duckSounds;



    [SerializeField] DuckTargetPart tailPosition;
    [SerializeField] DuckTargetPart bodyPosition;
    [SerializeField] DuckTargetPart headPosition;
    [SerializeField] DuckMovement duckMovement;

    [SerializeField] GameObject duckModelParent;
    [SerializeField] GameObject bulletHole;

    [SerializeField] GameObjectVariable deathEventGameObject;
    [SerializeField] GameEvent deathEvent;

    bool alive = true;
    public bool Alive { get { return alive; } }
    public TargetType TargetType { get { return TargetType.Duck; } }

    public void Hit(int damage, Vector2 impactPosition) {
        float closestPos = float.MaxValue;
        DuckTargetPart closestDuckPart = null;
        foreach (DuckTargetPart duckPart in new DuckTargetPart[] { headPosition, bodyPosition, tailPosition}) {
            float currDist = Vector2.Distance(impactPosition, duckPart.transform.position);
            if (currDist < closestPos) {
                closestPos = currDist;
                closestDuckPart = duckPart;
            }
        }

        // spawn bullet hole 
        Vector2 pos = closestDuckPart.GetClosestUnoccupiedLocalPosition(impactPosition);
        Debug.Log("[DuckTarget] spawning bullet hole");
        GameObject gob = Instantiate(bulletHole, duckModelParent.transform);
        gob.GetComponent<SpriteRotationManager>().SetFlipSpriteX(thisDuckSpriteManager.Flipped);
        gob.transform.localPosition = pos;


        int partMult = closestDuckPart.DuckPart switch {
            DuckPart.Head => 2,
            DuckPart.Body => 1,
            DuckPart.Tail => 1,
            _ => 1,
        };

        Damage(damage * partMult);
        if (CurrentHealth <= 0) {
            // death sound
            DeathNoise();

            duckAnimator.SetTrigger("DeathTrigger");
            deathEventGameObject.Value = this.gameObject;
            deathEvent.Raise();

            mainCollider.enabled = false;
            alive = false;
        } else {
            // hit sound
            duckSounds.Hit();
        }
    }


    [SerializeField] IntegerReference comboCount;
    void DeathNoise() {
        float pitchOffset = Mathf.Min(comboCount.Value / 2 * 0.05f, 1f);
        duckSounds.Death(pitchOffset);

    }

    public int CurrentHealth { get; set; }
    void IHealth.Setup(int maxHealth) { this.CurrentHealth = maxHealth; }
    public void Damage(int val) { this.CurrentHealth -= val; }
    public void Heal() { throw new System.NotImplementedException(); }


    public DuckType DuckType { get; set; }

    [SerializeField] Sprite yellowDuckSprite;
    [SerializeField] Sprite whiteDuckSprite;
    void IDuck.Setup(DuckType duckType) { 
        this.DuckType = duckType; 
        switch (duckType) {
            case DuckType.Mallard:
                break;
            case DuckType.Yellow:
                thisDuckSpriteManager.SetFrontFace(yellowDuckSprite);
                break;
            case DuckType.White:
                thisDuckSpriteManager.SetFrontFace(whiteDuckSprite);
                break;
            case DuckType.Dodger:
                thisDuckSpriteManager.SetFrontFace(yellowDuckSprite);
                thisDuckSpriteManager.SetFrontColor(new Color(0, 1, 0));
                break;
        }
    }

}


public enum TargetType
{
    Duck,
    Dialogue,
    BossDuck,
    BossWeakpoint
}
public interface ITarget {
    public TargetType TargetType { get; }
    public bool Alive { get; }
    public void Hit(int damage, Vector2 impactPosition);
}


public interface IDuck
{
    public void Setup(DuckType duckType);
    public DuckType DuckType { get; set; }
}

public interface IHealth { 
    public int CurrentHealth { get; set; }

    public void Setup(int maxHealth);

    public void Damage(int val);

    public void Heal();

}