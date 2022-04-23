using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTo : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Transform origin;
    [SerializeField] float minAngle;
    [SerializeField] float maxAngle;
    [SerializeField] float speed;

    float targetAngle;

    Quaternion targetRot;

    float timer;
    bool lastValid = false;

    private void Update()
    {
        UpdateTarget();
        
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, Time.deltaTime * speed);       // Rotate towards the target
    }

    void UpdateTarget()
    {
        timer -= Time.deltaTime;
        if (!lastValid && timer > 0) return;        // Return if the last angle wasn't valid and the timer hasn't reached 0 yet. Done to reduce the number of calculations carried out
        timer = 2;

        // Work out the direction to the target by only rotating in one axis
        Vector3 directToTarget = (target.position - origin.position).normalized;
        Vector3 right = Vector3.Cross(origin.right, directToTarget).normalized;
        Vector3 toTarget = Vector3.Cross(right, origin.right);
        float angle = Vector3.Dot(toTarget, origin.up);     // Find the angle to the target direction from the default rotation

        angle = Mathf.Acos(angle) * Mathf.Rad2Deg;

        if (float.IsNaN(angle)) return;

        if (angle > maxAngle || angle < minAngle) {
            if(angle > 90) targetRot = Quaternion.LookRotation(origin.forward, origin.up);      // If the angle is too large, set the target to the default rotation

            // If the angle is decreasing and less than 90, set the target to the minimum angle
            if (Vector3.Dot(toTarget, origin.forward) > 0 && angle < 90) targetRot = Quaternion.RotateTowards(Quaternion.LookRotation(origin.forward, origin.up), CalculateTargetRot(toTarget), -minAngle);
            lastValid = false;
            return;
        }
        lastValid = true;
        
        targetRot = CalculateTargetRot(toTarget);
    }

    Quaternion CalculateTargetRot(Vector3 toTarget)
    {
        Vector3 forward = Vector3.Cross(origin.right, toTarget);

        return Quaternion.LookRotation(forward, toTarget);
    }

}
