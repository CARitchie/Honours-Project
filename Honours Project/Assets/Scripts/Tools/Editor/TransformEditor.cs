using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;

// Used code found here: https://forum.unity.com/threads/extending-instead-of-replacing-built-in-inspectors.407612/
// to make sure the editor kept its original appearance

// Inspired by https://www.youtube.com/watch?v=5KfWWMY8Fho

[CustomEditor(typeof(Transform))]
public class TransformEditor : Editor
{
    //Unity's built-in editor
    Editor defaultEditor;
    Transform transform;
    PlanetGravity source;
    bool align = false;
    bool stick = false;

    void OnEnable()
    {
        //When this inspector is created, also create the built-in inspector
        defaultEditor = Editor.CreateEditor(targets, Type.GetType("UnityEditor.TransformInspector, UnityEditor"));
        transform = target as Transform;
    }

    void OnDisable()
    {
        //When OnDisable is called, the default editor we created should be destroyed to avoid memory leakage.
        //Also, make sure to call any required methods like OnDisable
        MethodInfo disableMethod = defaultEditor.GetType().GetMethod("OnDisable", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        if (disableMethod != null)
            disableMethod.Invoke(defaultEditor, null);
        DestroyImmediate(defaultEditor);
    }

    public override void OnInspectorGUI()
    {
        defaultEditor.OnInspectorGUI();

        source = (PlanetGravity)EditorGUILayout.ObjectField(source, typeof(PlanetGravity), true);

        if (source == null) return;

        if (GUILayout.Button("Align: " + (align ? "True" : "False")))
        {
            align = !align;
        }

        if (GUILayout.Button("Stick: " + (stick? "True" : "False")))
        {
            stick = !stick;
        }

        if (align)
        {
            Quaternion rot = Quaternion.FromToRotation(transform.up, (transform.position - source.transform.position));

            transform.rotation = rot * transform.rotation;      // Rotate the transform so that it is perpendicular to the planet's surface
        }

        if (stick)
        {
            Vector3 direction = (transform.position - source.transform.position).normalized;
            if(Physics.Raycast(source.transform.position + source.GetDistance() * direction, -direction, out RaycastHit hit,100, 1 << 8))       // Find a point on the ground
            {
                transform.position = hit.point;
            }
            else
            {
                transform.position = source.transform.position + source.GetDistance() * direction;
            }
        }
    }
}
