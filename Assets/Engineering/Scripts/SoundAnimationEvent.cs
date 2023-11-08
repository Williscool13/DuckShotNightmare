using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundAnimationEvent : MonoBehaviour
{
    [SerializeField] AudioSource source;
    [SerializeField] AudioClip sound;
    public void PlaySound() {
        source.clip = sound;
        source.time = 0;
        source.Play();
    }

}
