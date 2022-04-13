using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Useful
{
    public static bool Close(Vector3 point1, Vector3 point2, float closeness)
    {
        float distance = (point1 - point2).sqrMagnitude;

        return distance <= closeness * closeness;
    }

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
