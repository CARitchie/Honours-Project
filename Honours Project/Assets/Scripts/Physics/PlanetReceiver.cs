using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetReceiver : GravityReceiver
{
    [SerializeField] Vector3 velocity;
    [SerializeField] Vector3 angularVelocity;

    public override void CalculateForce(List<GravitySource> sources, float time)
    {
        Vector3 force = Vector3.zero;

        float G = GravityController.gravityConstant;

        float max = 0;
        Vector3 dir = Vector3.zero;

        for (int i = 0; i < sources.Count; i++)
        {
            if (sources[i].transform != transform)
            {
                Vector3 distance = sources[i].transform.position - transform.position;

                float strength = (G * sources[i].GetMass()) / distance.sqrMagnitude;
                force += distance.normalized * strength;

                if (strength > max)
                {
                    max = strength;
                    dir = distance.normalized * strength;
                }
            }
        }

        velocity += force * time;
        rb.MovePosition(rb.position + (velocity * time));
        rb.MoveRotation(rb.rotation * Quaternion.Euler(angularVelocity * time));
    }
}
