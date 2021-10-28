using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlanetGravity : GravitySource
{
    [SerializeField] Vector3 initialVelocity;

    Rigidbody rb;

    protected override void Awake()
    {
        base.Awake();

        rb = GetComponent<Rigidbody>();
        rb.mass = GetMass();
        rb.AddForce(initialVelocity, ForceMode.VelocityChange);
    }
}
