using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterGravity : GravityReceiver
{
    [SerializeField] float rotationSpeed = 150;
    [SerializeField] bool player;
    PersonController controller;

    GravitySource nearSource;

    protected override void Awake()
    {
        base.Awake();
        controller = GetComponentInChildren<PersonController>();
    }

    public override void ApplyForce(List<PlanetGravity> sources, float time, Vector3 playerVelocity)
    {
        if (!gameObject.activeInHierarchy) return;

        Vector3 force = Vector3.zero;

        float G = GravityController.gravityConstant;

        float max = float.MaxValue;
        Vector3 dir = Vector3.zero;
        GravitySource closestSource = null;

        for (int i = 0; i < sources.Count; i++)                                                                 // For every gravity source acting on this object
        {
            if (sources[i].transform != transform)                                                              // If the source is not this object
            {
                Vector3 direction = sources[i].transform.position - transform.position;
                float distance = direction.sqrMagnitude;
                direction = direction.normalized;

                float strength = (G * rb.mass * sources[i].GetMass()) / distance;                               // Calculate the force using the law of gravitation
                force += direction * strength;

                if(distance - sources[i].GetSquareDistance() < max && distance < sources[i].Influence)          // Find the closest gravity source where the character is within its influence range
                {
                    max = distance;
                    dir = direction;
                    closestSource = sources[i];
                }
            }
        }

        force *= defaultMultiplier;

        if(localGravitySources.Count > 0){                                      // If there are local gravity sources
            force += GetLocalForce();                                           // Add the force caused by them

            LocalGravitySource localGravitySource = ClosestLocalSource();       // Find the closest local gravity source
            dir = localGravitySource.GetGravityDirection(Vector3.zero);
            closestSource = localGravitySource;
        }

        controller.SetNearestSource(closestSource);                             // Set the controllers nearest gravity source
        nearSource = closestSource;

        if (float.IsNaN(force.x)) return;

        rb.AddForce(force);                                                     // Apply the force
        if (!player)
        {
            rb.AddForce(playerVelocity, ForceMode.VelocityChange);              // If not the player, add the player's velocity as a change in velocity
        }

    }

    // Function to rotate so that the gameobject is perpendicular to its nearest source's surface
    void Rotate(Vector3 direction, float time)
    {
        if (direction == Vector3.zero) return;

        // https://answers.unity.com/questions/395033/quaternionfromtorotation-misunderstanding.html

        Quaternion rot = Quaternion.FromToRotation(transform.up, -direction);

        rot = Quaternion.RotateTowards(Quaternion.identity, rot, time * rotationSpeed);

        rb.MoveRotation(rot * transform.rotation);
    }

    private void LateUpdate()
    {
        if (nearSource == null) return;
        Vector3 direction = nearSource.GetGravityDirection(transform.position);     // Find the correct direction
        Rotate(direction, Time.deltaTime);                                          // Rotate to said direction
    }
}
