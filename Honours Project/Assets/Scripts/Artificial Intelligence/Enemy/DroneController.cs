using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneController : EnemyController
{
    [Header("Drone Settings")]
    [SerializeField] float hoverHeight;

    GravitySource centre;

    protected override void Start()
    {
        base.Start();

        centre = nearestSource;

        Vector3 up = centre.GetUp(transform.position);
        transform.up = up;                                  // Align the drone's up direction with the inverse of the direction to the planet

        up = up.normalized;
        transform.position += (hoverHeight + 1.2f) * up;    // Increase the drone's height above the planet's surface
    }

    // Function to look at a specific point
    public override void Look(Vector3 point)
    {
        Vector3 direction = point - transform.position;                 // Find the direction to the point
        Vector3 toCentre = -centre.GetUp(transform.position);           // Find the direction to the centre of the planet

        direction = Vector3.RotateTowards(transform.forward, direction, lookSensitivity * Time.deltaTime, 0.0f);            // Rotate towards the target direction
        Vector3 right = -Vector3.Cross(toCentre, direction);            // Work out the directions to ensure that the drone remains perpendicular to the planet's surface
        Vector3 up = -Vector3.Cross(right, direction);

        if(Physics.Raycast(transform.position, toCentre, out RaycastHit hit,hoverHeight, 1 << 8))                           // If the drone is too close to the ground
        {   
            /*
            if(Vector3.Dot(direction,toCentre) > 0)
            {
                Vector3 forward = Vector3.Cross(right, -toCentre);
                direction = Vector3.RotateTowards(direction, forward, lookSensitivity * Time.deltaTime * 1.5f, 0.0f);
                up = -Vector3.Cross(right, direction);
            }*/

            transform.position = hit.point + (-toCentre.normalized) * hoverHeight;                                          // Increase the drone's height

        }

        transform.rotation = Quaternion.LookRotation(direction, up);    // Set the drone's rotation
    }

    // Function to move forwards
    public override void Move()
    {
        movementSpeed = 2;

        // Increase the drone's position so that it moves forward
        Vector3 target = transform.position + (transform.forward * movementSpeed * Time.deltaTime);

        transform.position = target;
    }
}
