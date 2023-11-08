using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GiantDuckTarget : MonoBehaviour, ITarget, IHealth, IDuck
{
    public TargetType TargetType { get { return TargetType.BossDuck; } }
    public bool Alive { get { return alive; } }
    public int CurrentHealth { get; set; }
    public DuckType DuckType { get { return DuckType.Boss; } set { } }
    void IHealth.Setup(int maxHealth) { this.CurrentHealth = maxHealth; giantDuckHealth.Value = CurrentHealth; }
    void IDuck.Setup(DuckType duckType) { this.DuckType = duckType; }


    public void Damage(int val) { this.CurrentHealth -= val; }
    public void Heal() { throw new System.NotImplementedException(); }

    [SerializeField] DuckTargetPart headPosition;
    [SerializeField] DuckTargetPart bodyPosition;
    [SerializeField] DuckTargetPart bodyPosition2;
    [SerializeField] DuckTargetPart bodyPosition3;
    [SerializeField] DuckTargetPart tailPosition;

    [SerializeField] DuckSounds duckSounds;


    [SerializeField] GameObject bulletHole;
    [SerializeField] Transform duckModelParent;

    [SerializeField] Animator duckAnimator;
    [SerializeField] SpriteRotationManager thisDuckSpriteManager;
    [SerializeField] Collider2D mainCollider;

    [SerializeField] IntegerVariable giantDuckHealth;
    [SerializeField] IntegerReference comboCount;

    [SerializeField] UnityEvent gameOverEvent;
    bool alive = true;


    public void Hit(int damage, Vector2 impactPosition) {
        float closestPos = float.MaxValue;
        DuckTargetPart closestDuckPart = null;
        foreach (DuckTargetPart duckPart in new DuckTargetPart[] { headPosition, bodyPosition, bodyPosition2, bodyPosition3, tailPosition }) {
            float currDist = Vector2.Distance(impactPosition, duckPart.transform.position);
            if (currDist < closestPos) {
                closestPos = currDist;
                closestDuckPart = duckPart;
            }
        }

        // spawn bullet hole 
        Vector2 pos = closestDuckPart.GetClosestPosition(impactPosition);
        Debug.Log("[DuckTarget] spawning bullet hole");
        GameObject gob = Instantiate(bulletHole,  duckModelParent.transform);
        gob.GetComponent<SpriteRotationManager>().SetFlipSpriteX(thisDuckSpriteManager.Flipped);
        BulletHoleScaleOffset off = gob.AddComponent<BulletHoleScaleOffset>();
        gob.transform.localScale = gob.transform.localScale * 4;
        off.parentParentScale = true;

        if (CurrentHealth > 10) { gob.AddComponent<BulletHoleTimeDecay>(); }

        gob.transform.localPosition = pos;


        int partMult = closestDuckPart.DuckPart switch {
            DuckPart.Head => 2,
            DuckPart.Body => 1,
            DuckPart.Tail => 1,
            _ => 1,
        };

        Damage(damage * partMult);
        giantDuckHealth.Value = CurrentHealth;
        giantDuckController.Hit();

        if (CurrentHealth <= 0) {
            if (alive) {
                // death sound
                DeathNoise();

                gameOverEvent.Invoke();
                duckAnimator.SetTrigger("DeathTrigger");

                mainCollider.enabled = false;
                alive = false;
            }
        }
        else {
            // hit sound
            duckSounds.Hit();
        }
    }


    void DeathNoise() {
        float pitchOffset = Mathf.Min(comboCount.Value / 2 * 0.05f, 1f);
        duckSounds.Death(pitchOffset);

    }

    int weakPointMultiplier = 20;
    [SerializeField] GiantDuckGameController giantDuckController;
    public void OnWeakpointHit() {
        Damage(1 * weakPointMultiplier);
        giantDuckHealth.Value = CurrentHealth;
        giantDuckController.WeakpointHit();

        if (CurrentHealth <= 0) {
            if (alive) {
                // death sound
                DeathNoise();

                gameOverEvent.Invoke();
                duckAnimator.SetTrigger("DeathTrigger");

                mainCollider.enabled = false;
                alive = false;
            }
        }
    }
}
