using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "GameObjectArrayVariable", menuName = "Scriptable Object/GameObject Array Variable")]
public class GameObjectArrayVariable : ScriptableObject {

    [SerializeField] private GameObject[] value;

    public GameObject[] Value {
        get { return value; }
        set { this.value = value; }
    }
}
