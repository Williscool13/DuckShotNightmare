using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class IntegerReference
{
    public bool UseConstant = true;
    public int ConstantValue;
    public IntegerVariable Variable;

    public int Value {
        get {
            return UseConstant ? ConstantValue : Variable.Value;
        }
    }
}