using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "My Assets/Prop Placer/Prop")]
public class PlaceableProp : ScriptableObject
{
    [SerializeField] GameObject prefab;
    [SerializeField] float radius = 1;
    [SerializeField] float spawnChance = 1;
    [SerializeField] float minScale = 1;
    [SerializeField] float maxScale = 1;

    public float GetWeight()
    {
        return spawnChance;
    }

    public void Spawn(Transform parent, Vector3 pos, Vector3 rot)
    {
        GameObject gameObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        Transform prop = gameObject.transform;

        prop.position = pos;

        Vector3 localRot = prop.localEulerAngles;
        localRot.y = Random.Range(0, 360);
        prop.localEulerAngles = localRot;

        prop.up = rot;

        prop.localScale = Random.Range(minScale, maxScale) * Vector3.one;

        prop.parent = parent;
    }
}
