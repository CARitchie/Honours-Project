using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlanetGravity : GravitySource
{
    [SerializeField] float surfaceAcceleration;
    [SerializeField] float distanceToSurface;

    Rigidbody rb;
    float mass;

    void Awake()
    {
        mass = CalculateMass();

        rb = GetComponent<Rigidbody>();
        rb.mass = GetMass();
        //rb.AddForce(initialVelocity, ForceMode.VelocityChange);
    }

    private void Start()
    {
        GravityController.AddSource(this);
    }

    public override Vector3 GetVelocity()
    {
        return GetComponent<PlanetReceiver>().GetVelocity();
    }

    private void OnValidate()
    {
        if (Application.isPlaying) return;

        OrbitViewer orbitViewer = FindObjectOfType<OrbitViewer>();
        if( orbitViewer != null) orbitViewer.UpdateOrbits();
    }   

    public float CalculateMass()
    {
        return (surfaceAcceleration * Mathf.Pow(distanceToSurface, 2)) / GravityController.gravityConstant;
    }

    public float GetMass()
    {
        return mass;
    }

    public float GetDistance()
    {
        return distanceToSurface;
    }

    public float GetSquareDistance()
    {
        return distanceToSurface * distanceToSurface;
    }

    public override Vector3 GetNorthDirection(Transform player)
    {
        Vector3 dir1 = (player.position - transform.position).normalized;
        Vector3 cross1 = Vector3.Cross(dir1, transform.up).normalized;
        return Vector3.Cross(cross1, dir1);
    }
}
