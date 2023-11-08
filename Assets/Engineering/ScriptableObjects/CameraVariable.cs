using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "CameraVariable", menuName = "Scriptable Object/Camera Variable")]
public class CameraVariable : ScriptableObject {

    [SerializeField] private Camera value;

    public Camera Value {
        get { return value; }
        set { this.value = value; }
    }
}