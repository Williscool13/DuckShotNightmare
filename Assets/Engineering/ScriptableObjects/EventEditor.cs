
#if UNITY_EDITOR
// Code to include in the Unity Editor but exclude from the final build
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(GameEvent), editorForChildClasses: true)]
public class EventEditor : Editor
{
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        GUI.enabled = Application.isPlaying;

        GameEvent e = target as GameEvent;
        if (GUILayout.Button("Raise"))
            e.Raise();
    }
}
#endif
