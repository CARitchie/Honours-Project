using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GravityReceiver : MonoBehaviour
{
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        GravityController.AddReceiver(this);
    }

    public void CalculateForce(List<GravitySource> sources)
    {
        Vector3 force = Vector3.zero;

        float G = GravityController.gravityConstant;

        for (int i = 0; i < sources.Count; i++)
        {
            if (sources[i].transform != transform)
            {
                Vector3 distance = sources[i].transform.position - transform.position;

                float strength = (G * rb.mass * sources[i].GetMass()) / distance.sqrMagnitude;
                force += distance.normalized * strength;
            }
        }

        rb.AddForce(force);
    }
}
