using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHoleTimeDecay : MonoBehaviour
{
    [SerializeField] float duration = 10.0f;
    float endTime = 0;
    void Start() { endTime = Time.time + duration; }
    void FixedUpdate() { if (endTime < Time.time) { Destroy(this.gameObject); } }
}
