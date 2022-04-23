using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// Tool was used to fill all of the cryo pods in the cryo bay with people
public class PodPersonSpawnControl : EditorWindow
{
    Transform parent;

    [MenuItem("Custom Tools/Pod People Spawner")]
    public static void ShowWindow()
    {
        GetWindow<PodPersonSpawnControl>("Pod Person Spawn Control");
    }

    private void OnGUI()
    {
        parent = (Transform)EditorGUILayout.ObjectField("Parent", parent, typeof(Transform), true);

        if (parent == null) return;

        if (GUILayout.Button("Spawn Pod People"))
        {
            SpawnPodPeople();
        }

        if (GUILayout.Button("Destroy Pod People"))
        {
            DeletePodPeople();
        }

        if (GUILayout.Button("Tidy Up"))
        {
            TidyUp();
        }
    }

    public void SpawnPodPeople()
    {
        PodPersonSpawner[] spawners = parent.GetComponentsInChildren<PodPersonSpawner>(true);       // Find all of the cryo pods where a person can be spawned
        if (spawners != null && spawners.Length > 0) spawners[0].CalculateWeights();                // Calculate the weighting for when choosing a random person
        foreach(PodPersonSpawner spawner in spawners)
        {
            spawner.SpawnPerson();
        }
    }

    public void DeletePodPeople()
    {
        PodPersonSpawner[] spawners = parent.GetComponentsInChildren<PodPersonSpawner>(true);
        foreach (PodPersonSpawner spawner in spawners)
        {
            spawner.DestroyPerson();
        }
    }

    public void TidyUp()
    {
        PodPersonSpawner[] spawners = parent.GetComponentsInChildren<PodPersonSpawner>(true);
        foreach (PodPersonSpawner spawner in spawners)
        {
            spawner.TidyUp();
        }
    }
}
