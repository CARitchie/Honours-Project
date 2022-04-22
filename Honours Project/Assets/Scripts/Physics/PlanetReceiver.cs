using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetReceiver : GravityReceiver
{
    [SerializeField] Vector3 velocity;
    [SerializeField] Vector3 angularVelocity;
    [SerializeField] Rigidbody[] localBodies;

    public override void ApplyForce(List<PlanetGravity> sources, float time, Vector3 playerVelocity)
    {
        Vector3 acceleration = CalculateForce(sources, time) / rb.mass;

        velocity += acceleration * time;                                            // Increase the planet's velocity
        velocity += playerVelocity;                                                 // Account for the player's velocity

        rb.MovePosition(rb.position + (velocity * time));                           // Change position
        rb.MoveRotation(rb.rotation * Quaternion.Euler(angularVelocity * time));    // Rotate planet, this does work but all planets have an angularVelocity of 0 as a result of strange side effects
    }

    protected override void Start()
    {
        base.Start();

        foreach(Rigidbody localBody in localBodies)
        {
            localBody.AddForce(velocity, ForceMode.VelocityChange);
        }
    }

    public Vector3 GetVelocity()
    {
        return velocity;
    }

#if (UNITY_EDITOR)
    private void OnValidate()
    {
        FindObjectOfType<OrbitViewer>()?.UpdateOrbits();
    }
#endif
}
