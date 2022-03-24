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
        
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, Time.deltaTime * speed);
    }

    void UpdateTarget()
    {
        timer -= Time.deltaTime;
        if (!lastValid && timer > 0) return;
        timer = 2;

        Vector3 directToTarget = (target.position - origin.position).normalized;
        Vector3 right = Vector3.Cross(origin.right, directToTarget).normalized;
        Vector3 toTarget = Vector3.Cross(right, origin.right);
        float angle = Vector3.Dot(toTarget, origin.up);

        angle = Mathf.Acos(angle) * Mathf.Rad2Deg;

        if (float.IsNaN(angle)) return;

        if (angle > maxAngle || angle < minAngle) {
            if(angle > 90) targetRot = Quaternion.LookRotation(origin.forward, origin.up);
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
