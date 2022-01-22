using UnityEditor;
using UnityEngine;
using System.Reflection;
using System;
using System.Linq;

[CustomEditor(typeof(PawnBase))]
public class PawnBaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Generate Color"))
        {
            var types = Assembly.GetAssembly(typeof(ControllerBase)).GetTypes().Where(t => t.IsSubclassOf(typeof(ControllerBase)));
            foreach (Type t in types) {
                Debug.Log(t);
            }
        }
    }
}
