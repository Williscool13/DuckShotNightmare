using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioClipGroupReference", menuName = "Scriptable Object/Audio Clip Group Reference")]
public class AudioClipGroupReference : ScriptableObject
{
    [SerializeField] private AudioClip[] clips;

    public AudioClip[] Value {
        get { return clips; }
    }
}
