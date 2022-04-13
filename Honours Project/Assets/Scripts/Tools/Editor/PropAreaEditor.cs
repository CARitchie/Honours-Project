using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PropArea))]
public class PropAreaEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PropArea propArea = (PropArea)target;

        if(GUILayout.Button("Place Props"))
        {
            propArea.GenerateProps();
        }

        if(GUILayout.Button("Destroy Props"))
        {
            propArea.DestroyProps();
        }
    }
}
