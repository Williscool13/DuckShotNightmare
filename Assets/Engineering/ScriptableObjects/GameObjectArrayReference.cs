using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameObjectArrayReference {
    public bool UseConstant = true;
    public GameObject[] ConstantValue;
    public GameObjectArrayVariable Variable;

    public GameObject[] Value {
        get {
            return UseConstant ? ConstantValue : Variable.Value;
        }
    }
}
