using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FloatVariable", menuName = "Scriptable Object/Float Variable")]
public class FloatVariable : ScriptableObject {

    [SerializeField] private float value;

    public float Value {
        get { return value; }
        set { this.value = value; }
    }
}