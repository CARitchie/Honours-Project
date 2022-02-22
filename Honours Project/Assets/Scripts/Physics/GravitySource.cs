using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitySource : MonoBehaviour
{
    [SerializeField] float influenceRange;

    public float Influence { get { return influenceRange * influenceRange; } }

    public virtual Vector3 GetNorthDirection(Transform player)
    {
        return Vector3.zero;
    }

    public virtual Vector3 GetVelocity()
    {
        return GetComponentInChildren<Rigidbody>().velocity;
    }
}
