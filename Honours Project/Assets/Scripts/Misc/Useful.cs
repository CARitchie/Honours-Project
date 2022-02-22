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
}
