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
        GravityController.AddSource(this);          // Add this source to the gravity controllers list of sources
    }

    // Function to retrieve the velocity of the planet
    public override Vector3 GetVelocity()
    {
        return GetComponent<PlanetReceiver>().GetVelocity();
    }

    private void OnValidate()
    {
        if (Application.isPlaying) return;              // Return if the game is playing

        OrbitViewer orbitViewer = FindObjectOfType<OrbitViewer>();
        if( orbitViewer != null) orbitViewer.UpdateOrbits();
    }   

    public float CalculateMass()
    {
        return (surfaceAcceleration * Mathf.Pow(distanceToSurface, 2)) / GravityController.gravityConstant;     // Rearranged law of gravitation is used to calculate mass
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
        // Calculate the the direction to the north pole from the player's position so that it is perpendicular to the player's up direction
        Vector3 dir1 = (player.position - transform.position).normalized;
        Vector3 cross1 = Vector3.Cross(dir1, transform.up).normalized;
        return Vector3.Cross(cross1, dir1);
    }
}
