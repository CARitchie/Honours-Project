using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitySource : MonoBehaviour
{
    [SerializeField] float surfaceAcceleration;
    [SerializeField] float distanceToSurface;
    [SerializeField] float influenceRange;

    public float Influence { get { return influenceRange * influenceRange; } }

    float mass;

    protected virtual void Awake()
    {
        mass = CalculateMass();
    }

    private void Start()
    {
        GravityController.AddSource(this);
    }

    public float CalculateMass()
    {
        return (surfaceAcceleration * Mathf.Pow(distanceToSurface, 2)) / GravityController.gravityConstant;
    }

    public float GetMass()
    {
        return mass;
    }

    public float GetDistance()
    {
        return distanceToSurface;
    }

    public float GetSquareDistance()
    {
        return Mathf.Pow(distanceToSurface, 2);
    }
}
