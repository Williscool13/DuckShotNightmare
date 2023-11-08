using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IntegerVariable", menuName = "Scriptable Object/Integer Variable")]
public class IntegerVariable : ScriptableObject
{

    [SerializeField] private int value;

    public int Value {
        get { return value; }
        set { this.value = value; }
    }
}