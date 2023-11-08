using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{

    [SerializeField] AudioClipGroupReference audioClips;
    [SerializeField] AudioSource source;

    public void PlayRandomClip(float pitchDiff) {
        int count = audioClips.Value.Length;

        int randomSelected = Random.Range(0, count);

        source.pitch = 1.0f + pitchDiff;
        source.PlayOneShot(audioClips.Value[randomSelected]);
    }
}
