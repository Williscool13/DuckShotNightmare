using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueWriterVariable", menuName = "Scriptable Object/DialogueWriter Variable")]
public class DialogueWriterVariable : ScriptableObject
{

    [SerializeField] private DialogueWriter value;

    public DialogueWriter Value {
        get { return value; }
        set { this.value = value; }
    }
}