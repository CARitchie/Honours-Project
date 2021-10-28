using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitySource : MonoBehaviour
{
    [SerializeField] float surfaceAcceleration;
    [SerializeField] float distanceToSurface;

    Rigidbody rb;
    float mass;

    protected virtual void Awake()
    {
        CalculateMass();
    }

    private void Start()
    {
        GravityController.AddSource(this);
    }

    void CalculateMass()
    {
        mass = (surfaceAcceleration * Mathf.Pow(distanceToSurface, 2)) / GravityController.gravityConstant;
    }

    public float GetMass()
    {
        return mass;
    }
}
