using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowFall : MonoBehaviour
{
    [SerializeField] float radius;
    [SerializeField] float flakeAmount;
    [SerializeField] float flakeOpacity;

    public Vector3 GetDetails()
    {
        return new Vector3(radius, flakeAmount, flakeOpacity);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0,1,0.1f);
        Gizmos.DrawSphere(transform.position, radius);
    }
}
