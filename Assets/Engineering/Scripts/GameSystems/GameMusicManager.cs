using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMusicManager : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioClip[] musicClips;

    public void PlayClip(int index) {
        musicSource.Stop();
        musicSource.clip = musicClips[index];
        musicSource.time = 0;
        musicSource.Play();
    }
    
    public bool FadeInClip(AudioClip clip, float inRate) {
        if (transitioning) { Debug.Log("currently transitioning music already"); return false; }
        StartCoroutine(FadeIn(clip, inRate));

        return true;

    }

    public bool FadeoutClip(float outRate) {
        if (transitioning) { Debug.Log("currently transitioning music already"); return false; }

        StartCoroutine(FadeOut(outRate));

        return true;
    }

    public bool PlayClip(AudioClip clip, float transitionOutRate, float transitionInRate, float waitTimeBetween) {
        if (transitioning) { Debug.Log("currently transitioning music already"); return false; }
        StartCoroutine(ClipTransition(clip, transitionOutRate, transitionInRate, waitTimeBetween));

        return true;
    }

    bool distort = false;
    public void Distort(float min, float max) {
        distort = true;

        distortMin = min;
        distortMax = max;
    }

    float distortTimestamp;
    [SerializeField] float distortMin;
    [SerializeField] float distortMax;

    float targetPitch = 1.0f;
    private void Update() {
        if (!distort) { return; }

        if (distortTimestamp <= Time.time) {
            targetPitch = 1.0f + Random.Range(distortMin, distortMax);
            distortTimestamp = Time.time + 5.0f;
        }



        musicSource.pitch = Mathf.Lerp(musicSource.pitch, targetPitch, Time.deltaTime);
    }

    bool transitioning = false;
    IEnumerator ClipTransition(AudioClip clip, float transitionOutRate, float transitionInRate, float waitTimeBetween) {
        transitioning = true;

        float transitionVal = 1.0f;
        WaitForSeconds fixedOutTime =  new WaitForSeconds(Time.fixedDeltaTime * transitionOutRate);
        while (transitionVal > 0) {
            yield return fixedOutTime;

            transitionVal -= Time.fixedDeltaTime * transitionOutRate;
            musicSource.volume = transitionVal;
        }

        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.time = 0;

        yield return new WaitForSeconds(waitTimeBetween);
        musicSource.Play();

        WaitForSeconds fixedInTime = new WaitForSeconds(Time.fixedDeltaTime * transitionInRate);
        while (transitionVal < 1) {
            yield return fixedInTime;

            transitionVal += Time.fixedDeltaTime * transitionInRate;
            musicSource.volume = transitionVal;
        }

        transitioning = false;
    }
    IEnumerator FadeIn(AudioClip clip, float inRate) {
        transitioning = true;
        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.time = 0;
        musicSource.volume = 0;

        musicSource.Play();

        float transitionVal = 0f;
        WaitForSeconds fixedInTime = new WaitForSeconds(Time.fixedDeltaTime * inRate);
        while (transitionVal < 1) {
            yield return fixedInTime;

            transitionVal += Time.fixedDeltaTime * inRate;
            musicSource.volume = transitionVal;
        }
        transitioning = false;

    }
    IEnumerator FadeOut(float outRate) {
        transitioning = true;
        float transitionVal = 1.0f;
        WaitForSeconds fixedOutTime = new WaitForSeconds(Time.fixedDeltaTime * outRate);
        while (transitionVal > 0) {
            yield return fixedOutTime;

            transitionVal -= Time.fixedDeltaTime * outRate;
            musicSource.volume = transitionVal;
        }

        musicSource.Stop();
        transitioning = false;
    }
}
