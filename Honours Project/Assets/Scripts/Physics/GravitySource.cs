using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitySource : MonoBehaviour
{
    [SerializeField] string key;
    [SerializeField] float influenceRange;
    [SerializeField] bool hasAtmosphere;
    [SerializeField] float startFade;

    public float Influence { get { return influenceRange * influenceRange; } }

    // Function to find the direction north from the player's position
    public virtual Vector3 GetNorthDirection(Transform player)
    {
        return Vector3.zero;
    }

    public virtual Vector3 GetGravityDirection(Vector3 point)
    {
        return (transform.position - point).normalized;
    }

    public virtual Vector3 GetVelocity()
    {
        return GetComponentInChildren<Rigidbody>().velocity;
    }

    public bool HasAtmosphere()
    {
        return hasAtmosphere;
    }

    public float SoundPercent(Vector3 point)
    {
        if (influenceRange <= 0 && startFade <= 0) return 1;        // Return if there should not be any sound fading

        float height = (point - transform.position).magnitude;
        if (height < startFade) return 1;

        height -= startFade;
        return Mathf.Clamp01(1 - (height / (influenceRange - startFade)));
    }

    public virtual Vector3 GetUp(Vector3 point)
    {
        return point - transform.position;
    }

    public string Key { get { return key; } }
}
