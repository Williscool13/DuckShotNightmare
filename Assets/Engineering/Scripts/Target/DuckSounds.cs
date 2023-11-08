using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuckSounds : MonoBehaviour
{
    [SerializeField] AudioSource duckSoundSource;

    [SerializeField] AudioClipGroupReference duckHitSounds;
    [SerializeField] AudioClipGroupReference duckDeathSounds;
    [SerializeField] AudioClipGroupReference duckQuackSounds;
    
    [SerializeField] AudioSource hitSource;
    [SerializeField] AudioSource deathSource;

    float baseHitPitch;
    float baseDeathPitch;
    private void Start() {
        baseHitPitch = hitSource.pitch;
        baseDeathPitch = deathSource.pitch;
    }

    public void Hit() {
        if (duckHitSounds.Value.Length == 0) { Debug.Log("No hit sounds attached"); return; }

        int randIndex = Random.Range(0, duckHitSounds.Value.Length);
        float randPitchDiff = Random.Range(-0.1f, 0.1f);
        hitSource.pitch = baseHitPitch + randPitchDiff;
        hitSource.PlayOneShot(duckHitSounds.Value[randIndex]);
    }

    public void Death(float pitchOffset) {
        
        if (duckDeathSounds.Value.Length == 0) { Debug.Log("No death sounds attached to duck"); return; }

        deathSource.pitch = baseDeathPitch + pitchOffset;
        int randIndex = Random.Range(0, duckDeathSounds.Value.Length);

        deathSource.clip = duckDeathSounds.Value[randIndex];
        deathSource.Stop();
        deathSource.Play();
        //duckSoundSource.PlayOneShot();

    }

    public void DeathRandomPitch(float pitchDiff) {
        Death(pitchDiff);
    }

    public void Quack(float pitchDiff, float panStereo) {
        if (duckQuackSounds.Value.Length == 0) { Debug.Log("No quack sounds attached"); return; }

        
        int randIndex = Random.Range(0, duckQuackSounds.Value.Length);
        duckSoundSource.pitch = 1.0f + pitchDiff;
        duckSoundSource.panStereo = panStereo;
        duckSoundSource.PlayOneShot(duckQuackSounds.Value[randIndex]);
    }



}
