using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameObjectVariable", menuName = "Scriptable Object/Game Object Variable")]
public class GameObjectVariable : ScriptableObject
{
    [SerializeField] private GameObject value;

    public GameObject Value {
        get { return value; }
        set { this.value = value; }
    }
}
