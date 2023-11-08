using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameObjectReference {
    public bool UseConstant = true;
    public GameObject ConstantValue;
    public GameObjectVariable Variable;

    public GameObject Value {
        get {
            return UseConstant ? ConstantValue : Variable.Value;
        }
    }
}