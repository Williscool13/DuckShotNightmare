using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FloatArrayVariable", menuName = "Scriptable Object/FloatArray Variable")]
public class FloatArrayVariable : ScriptableObject
{

    [SerializeField] private float[] value;

    public float[] Value {
        get { return value; }
        set { this.value = value; }
    }
}
