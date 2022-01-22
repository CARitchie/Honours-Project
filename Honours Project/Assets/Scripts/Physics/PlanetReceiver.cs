using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetReceiver : GravityReceiver
{
    [SerializeField] Vector3 velocity;
    [SerializeField] Vector3 angularVelocity;

    public override void ApplyForce(List<PlanetGravity> sources, float time, Vector3 playerVelocity)
    {
        Vector3 force = CalculateForce(sources, time) / rb.mass;

        velocity += force * time;
        velocity += playerVelocity;

        rb.MovePosition(rb.position + (velocity * time));
        rb.MoveRotation(rb.rotation * Quaternion.Euler(angularVelocity * time));
    }
}
