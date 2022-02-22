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

        for (int i = 0; i < sources.Count; i++)
        {
            if (sources[i].transform != transform)
            {
                Vector3 direction = sources[i].transform.position - transform.position;
                float distance = direction.sqrMagnitude;
                direction = direction.normalized;

                float strength = (G * rb.mass * sources[i].GetMass()) / distance;
                force += direction * strength;

                if(distance - sources[i].GetSquareDistance() < max && distance < sources[i].Influence)
                {
                    max = distance;
                    dir = direction;
                    closestSource = sources[i];
                }
            }
        }

        force *= defaultMultiplier;

        if(localGravitySources.Count > 0){
            force += GetLocalForce();

            LocalGravitySource localGravitySource = ClosestLocalSource();
            dir = localGravitySource.GetGravityDirection(Vector3.zero);
            closestSource = localGravitySource;
        }

        controller.SetNearestSource(closestSource);
        nearSource = closestSource;

        if (float.IsNaN(force.x)) return;

        rb.AddForce(force);
        if (!player)
        {
            rb.AddForce(playerVelocity, ForceMode.VelocityChange);
        }

    }

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
        Vector3 direction = nearSource.GetGravityDirection(transform.position);
        Rotate(direction, Time.deltaTime);
    }

    public void FindClosest(List<PlanetGravity> sources)
    {
        GravitySource closest = null;

        if (localGravitySources.Count > 0)
        {
            LocalGravitySource localGravitySource = ClosestLocalSource();
            closest = localGravitySource;
            return;
        }

        float max = 0;
        for (int i = 0; i < sources.Count; i++)
        {
            if (sources[i].transform != transform)
            {
                Vector3 direction = sources[i].transform.position - transform.position;
                float magnitude = direction.sqrMagnitude;

                if (magnitude - sources[i].GetSquareDistance() < max && magnitude < sources[i].Influence)
                {
                    max = magnitude;
                    closest = sources[i];
                }
            }
        }
    }
}
