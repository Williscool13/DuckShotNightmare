using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FloatArrayReference
{
    public bool UseConstant = true;
    public float[] ConstantValue;
    public FloatArrayVariable Variable;

    public float[] Value {
        get {
            return UseConstant ? ConstantValue : Variable.Value;
        }
    }
}
