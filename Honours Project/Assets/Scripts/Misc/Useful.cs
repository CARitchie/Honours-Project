using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Useful
{
    // Determine whether two points are within a specified range of one another
    public static bool Close(Vector3 point1, Vector3 point2, float closeness)
    {
        float distance = (point1 - point2).sqrMagnitude;        // Using sqrMagnitude is faster than finding the square root

        return distance <= closeness * closeness;
    }

    // Function to destroy any gameobject and save any hitmarkers that may have become a child of said gameobject
    public static void DestroyGameObject(GameObject gameObject)
    {
        HitMarker[] markers = gameObject.GetComponentsInChildren<HitMarker>(true);
        foreach (HitMarker marker in markers)
        {
            marker.SaveFromDeath();
        }

        Object.Destroy(gameObject);
    }
}
