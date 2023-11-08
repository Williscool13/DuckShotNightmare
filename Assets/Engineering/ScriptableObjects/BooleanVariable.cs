using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BooleanVariable", menuName = "Scriptable Object/Boolean Variable")]
public class BooleanVariable : ScriptableObject
{

    [SerializeField] private bool value;

    public bool Value {
        get { return value; }
        set { this.value = value; }
    }
}