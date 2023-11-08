using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Gun : MonoBehaviour
{
    [SerializeField] Camera thisCamera;
    [SerializeField] LayerMask targetLayerMask;
    [SerializeField] AudioPlayer gunshotAudioPlayer;
    [SerializeField] AudioPlayer gunCockAudioPlayer;
    [SerializeField] AudioPlayer reloadAudioPlayer;
    [SerializeField] AudioPlayer quickReloadAudioPlayer;

    [SerializeField] FloatReference bulletRadius;

    [SerializeField] UnityEvent shootEvent;
    [SerializeField] UnityEvent reloadEvent;
    [SerializeField] UnityEvent hitEvent;
    [SerializeField] GameObjectVariable hitGameObject;

    [SerializeField] CrosshairVisual crosshairVisual;
    [SerializeField] GunVisual gunVisual;

    [SerializeField] float gunCockCooldown = 0.1f;


    [SerializeField] IntegerVariable currentAmmo;
    [SerializeField] IntegerReference maxAmmo;
    [SerializeField] FloatReference reloadTime;
    [SerializeField] FloatVariable reloadTimestamp;
    [SerializeField] GameObjectReference onDeathDuck;
    [SerializeField] BooleanVariable goldenBullet;

    bool gunActive = false;

    private void Start() {
        currentAmmo.Value = maxAmmo.Value;
        goldenBullet.Value = false;
        reloadTimestamp.Value = 0.0f;
    }



    private void Update() {
        if (!gunActive) { return; }
        if (Input.GetMouseButtonDown(0)) {
            Cursor.visible = false;
            if (CanShoot()) {
                currentAmmo.Value -= 1;

                Shoot();
                if (currentAmmo.Value == 0) {
                    Reload();
                }
                else {
                    Cock();
                }
            }
        }

            
        
    }
    [SerializeField] CameraShaker shaker;
    bool CanShoot() {
        return gunCockTimestamp < Time.time && reloadTimestamp.Value < Time.time && !shaker.IsShaking();
    }

    public void SetGunActive(bool active) {
        crosshairVisual.SetCrosshairActive(active);
        gunVisual.SetGunActive(active);
        gunActive = active;
    }

    float gunCockTimestamp = 0;
    void Shoot() {
        shootEvent.Invoke();

        Vector2 mousePosition = thisCamera.ScreenToWorldPoint(Input.mousePosition);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(mousePosition, bulletRadius.Value);

        if (colliders.Length > 0) {
            // only 1 target per bullet, can increase to 2 for future projectiles.
            ITarget firstTargetInterface = null;
            Collider2D firstTargetCollider = null;
            bool duckHit = false;
            bool bossTargetHit = false;
            bool dialogueHit = false;
            for (int i=0; i< colliders.Length; i++) {
                ITarget locTarget = colliders[i].transform.GetComponent<ITarget>();
                if (locTarget != null) {
                    if (locTarget.TargetType == TargetType.Dialogue) {
                        firstTargetInterface = locTarget;
                        firstTargetCollider = colliders[i];
                        break;
                    } else {
                        firstTargetInterface = locTarget;
                        firstTargetCollider = colliders[i];
                        bossTargetHit = true;
                    }
                    
                } 
                else {
                    ITarget target = colliders[i].transform.root.GetComponent<ITarget>();
                    if (target != null) {
                        // dialogue always take priority
                        switch (target.TargetType) {
                            case TargetType.Dialogue:
                                firstTargetInterface = target;
                                firstTargetCollider = colliders[i];
                                dialogueHit = true;
                                break;
                            case TargetType.Duck:
                                if (!duckHit) {
                                    firstTargetInterface = target;
                                    firstTargetCollider = colliders[i];
                                    duckHit = true;
                                }
                                continue;
                            case TargetType.BossDuck:
                                if (!duckHit && !bossTargetHit) {
                                    firstTargetInterface = target;
                                    firstTargetCollider = colliders[i];
                                }
                                continue;

                        }
                        if (dialogueHit) { 
                            break; 
                        }
                    }
                }
            }
            if (firstTargetInterface != null) {
                int damage = 1;
                if (goldenBullet.Value) { damage *= 2; }
                firstTargetInterface.Hit(damage, mousePosition);
                Debug.Log($"[Gun] Hit a target");

                hitGameObject.Value = firstTargetCollider.transform.root.gameObject;
                hitEvent.Invoke();

            } else {
                Debug.Log("[Gun] Hit ITargets, but didnt find a duck or dialogue");
            }
        }
        else {
            // No colliders found within the detection zone
            Debug.Log("[Gun] No colliders within the detection zone.");
        }

        gunshotAudioPlayer.PlayRandomClip(0);
    }

    public void OnDuckDeath() {
        if (onDeathDuck.Value.GetComponent<DuckTarget>().DuckType == DuckType.Yellow) {
            currentAmmo.Value = maxAmmo.Value;
            goldenBullet.Value = true;

            quickReloadAudioPlayer.PlayRandomClip(0);
        }
    }

    void Reload() {
        reloadEvent.Invoke();
        gunVisual.Reload(reloadTime.Value);
        reloadAudioPlayer.PlayRandomClip(0);
        reloadTimestamp.Value = Time.time + reloadTime.Value;

        //currentAmmo.Value = maxAmmo.Value;
        StartCoroutine(ReloadCoroutine());
    }

    IEnumerator ReloadCoroutine() {
        while (reloadTimestamp.Value > Time.time) { yield return null; }

        currentAmmo.Value = maxAmmo.Value;
        goldenBullet.Value = false;
    }


    void Cock() {
        gunCockAudioPlayer.PlayRandomClip(0);
        
        gunCockTimestamp = Time.time + gunCockCooldown;
    }
}
