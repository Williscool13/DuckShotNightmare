using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CameraReference
{
    public bool UseConstant = true;
    public Camera ConstantValue;
    public CameraVariable Variable;

    public Camera Value {
        get {
            return UseConstant ? ConstantValue : Variable.Value;
        }
    }
}