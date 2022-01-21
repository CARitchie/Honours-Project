using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterGravity : GravityReceiver
{
    [SerializeField] float rotationSpeed = 150;
    PersonController controller;

    protected override void Awake()
    {
        base.Awake();
        controller = GetComponentInChildren<PersonController>();
    }


    public override void CalculateForce(List<PlanetGravity> sources, float time)
    {
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
                float magnitude = direction.sqrMagnitude;
                direction = direction.normalized;

                float strength = (G * rb.mass * sources[i].GetMass()) / magnitude;
                force += direction * strength;

                if(magnitude - sources[i].GetSquareDistance() < max && magnitude < sources[i].Influence)
                {
                    max = magnitude;
                    dir = direction * strength;
                    closestSource = sources[i];
                }
            }
        }

        if(localGravitySources.Count > 0){
            force += GetLocalForce();

            LocalGravitySource localGravitySource = ClosestLocalSource();
            dir = localGravitySource.GetDirection();
            closestSource = localGravitySource;
        }

        controller.SetNearestSource(closestSource);

        rb.AddForce(force);

        // https://answers.unity.com/questions/395033/quaternionfromtorotation-misunderstanding.html

        if (dir == Vector3.zero) return;

        Quaternion rot = Quaternion.FromToRotation(transform.up, -dir);

        rot = Quaternion.RotateTowards(Quaternion.identity, rot, time * rotationSpeed);

        transform.rotation = rot * transform.rotation;
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
