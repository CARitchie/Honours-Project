using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneController : EnemyController
{
    [Header("Drone Settings")]
    GravitySource centre;
    [SerializeField] float hoverHeight;

    protected override void Start()
    {
        base.Start();

        centre = nearestSource;

        Vector3 up = centre.GetUp(transform.position);
        transform.up = up;

        up = up.normalized;
        transform.position += (hoverHeight + 1.2f) * up;
    }

    public override void Look(Vector3 point)
    {
        Vector3 direction = point - transform.position;
        Vector3 toCentre = -centre.GetUp(transform.position);

        direction = Vector3.RotateTowards(transform.forward, direction, lookSensitivity * Time.deltaTime, 0.0f);
        Vector3 right = -Vector3.Cross(toCentre, direction);
        Vector3 up = -Vector3.Cross(right, direction);

        if(Physics.Raycast(transform.position, toCentre, out RaycastHit hit,hoverHeight, 1 << 8))
        {/*
            if(Vector3.Dot(direction,toCentre) > 0)
            {
                Vector3 forward = Vector3.Cross(right, -toCentre);
                direction = Vector3.RotateTowards(direction, forward, lookSensitivity * Time.deltaTime * 1.5f, 0.0f);
                up = -Vector3.Cross(right, direction);
            }*/

            transform.position = hit.point + (-toCentre.normalized) * hoverHeight;

        }

        transform.rotation = Quaternion.LookRotation(direction, up);
    }

    public override void Move()
    {
        movementSpeed = 2;

        Vector3 target = transform.position + (transform.forward * movementSpeed * Time.deltaTime);

        transform.position = target;
    }
}
