using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BossWeakpointTarget : MonoBehaviour, ITarget
{
    public TargetType TargetType => TargetType.BossWeakpoint;

    public bool Alive { get { return alive; } }

    bool alive = true;


    [SerializeField] UnityEvent OnWeakpointHit;
    [SerializeField] IntegerReference comboCount;
    [SerializeField] AudioPlayer audioPlayer;

    [SerializeField] SpriteRenderer thisSpriteRenderer;
    [SerializeField] Collider2D thisCollider;
    public void Hit(int damage, Vector2 impactPosition) {
        // spawn bullet hole at position of impact , clamped to distance from center magnitude

        // play sound pitched to combo
        float pitchOffset = Mathf.Min(comboCount.Value / 2 * 0.05f, 1f);
        audioPlayer.PlayRandomClip(pitchOffset);

        alive = false;

        // hide interface;
        thisSpriteRenderer.enabled = false;
        thisCollider.enabled = false;

        OnWeakpointHit.Invoke();

        parentController.RemoveWeakpointFromActiveTargets(this);
        // delete after sound play
        StartCoroutine(DestroyAfterDelay());
    }

    GiantDuckGameController parentController;
    public void Setup(GiantDuckGameController parentController) {
        this.parentController = parentController;
    }
    IEnumerator DestroyAfterDelay() {
        yield return new WaitForSeconds(5.0f);
    }
}
