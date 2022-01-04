using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterGravity : GravityReceiver
{
    [SerializeField] float rotationSpeed = 150;

    public override void CalculateForce(List<GravitySource> sources, float time)
    {
        Vector3 force = Vector3.zero;

        float G = GravityController.gravityConstant;

        float max = 100000000;
        Vector3 dir = Vector3.zero;

        for (int i = 0; i < sources.Count; i++)
        {
            if (sources[i].transform != transform)
            {
                Vector3 distance = sources[i].transform.position - transform.position;

                float strength = (G * rb.mass * sources[i].GetMass()) / distance.sqrMagnitude;
                force += distance.normalized * strength;

                if(distance.sqrMagnitude - sources[i].GetSquareDistance() < max)
                {
                    max = distance.sqrMagnitude;
                    dir = distance.normalized * strength;
                }
            }
        }

        if(localGravitySources.Count > 0){
            force += GetLocalForce();
            dir = ClosestLocalSource().GetDirection();
        }

        rb.AddForce(force);

        // https://answers.unity.com/questions/395033/quaternionfromtorotation-misunderstanding.html

        Quaternion rot = Quaternion.FromToRotation(transform.up, -dir);

        rot = Quaternion.RotateTowards(Quaternion.identity, rot, time * rotationSpeed);

        transform.rotation = rot * transform.rotation;
    }
}
