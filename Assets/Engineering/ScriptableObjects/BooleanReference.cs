using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BooleanReference
{
    public bool UseConstant = true;
    public bool ConstantValue;
    public BooleanVariable Variable;

    public bool Value {
        get {
            return UseConstant ? ConstantValue : Variable.Value;
        }
    }
}