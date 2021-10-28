using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitViewer : MonoBehaviour
{
    [SerializeField] bool displayOrbits;
    [SerializeField] int numberOfPoints;
    [SerializeField] float timeStep;
    [SerializeField] Transform followCamera;
    [SerializeField] int followTarget;
    [SerializeField] List<OrbitObject> orbitObjects = new List<OrbitObject>();

    private void OnValidate()
    {
        UpdateOrbits();
        StartFollow();
    }

    void UpdateOrbits()
    {
        if (Application.isPlaying) return;

        if (!displayOrbits)
        {
            SetActive(false);
            return;
        }

        foreach(OrbitObject orbitObject in orbitObjects)
        {
            orbitObject.Initialise();
        }

        for(int i = 0; i < numberOfPoints; i++)
        {
            foreach (OrbitObject orbitObject in orbitObjects)
            {
                orbitObject.ChangeVelocity(orbitObjects, timeStep);
            }

            foreach (OrbitObject orbitObject in orbitObjects)
            {
                orbitObject.ChangePosition(timeStep);
                orbitObject.Finalise();
            }
        }
    }

    void SetActive(bool active)
    {
        foreach(OrbitObject orbitObject in orbitObjects)
        {
            orbitObject.SetActive(active);
        }
    }

    void StartFollow()
    {
        if (!Application.isPlaying) return;
        if (followTarget >= 0 && followTarget < orbitObjects.Count)
        {
            StopAllCoroutines();
            StartCoroutine(FollowPlanet());
        }
    }

    IEnumerator FollowPlanet()
    {
        Camera.main.gameObject.SetActive(false);
        followCamera.gameObject.SetActive(true);

        while (true && followTarget >= 0 && followTarget < orbitObjects.Count)
        {
            followCamera.transform.position = orbitObjects[followTarget].GetPosition();
            yield return new WaitForFixedUpdate();
        }
    }
}

[System.Serializable]
class OrbitObject
{
    [SerializeField] PlanetGravity source;
    [SerializeField] LineRenderer lineRenderer;

    List<Vector3> positions = new List<Vector3>();

    float mass;
    Vector3 lastPos;
    Vector3 velocity;

    public void Initialise()
    {
        if (source == null || lineRenderer == null) return;

        SetActive(true);
        mass = source.CalculateMass();
        lastPos = source.transform.position;
        velocity = source.GetVelocity();
        positions = new List<Vector3>();
    }

    public Vector3 GetPosition()
    {
        return source.transform.position + Vector3.up * source.GetDistance() * 10;
    }

    Vector3 CalculateAcceleration(List<OrbitObject> orbitObjects)
    {
        Vector3 acceleration = Vector3.zero;

        float G = GravityController.gravityConstant;

        for (int i = 0; i < orbitObjects.Count; i++)
        {
            if (orbitObjects[i] != this)
            {
                Vector3 distance = orbitObjects[i].lastPos - lastPos;

                float strength = (G * orbitObjects[i].mass) / distance.sqrMagnitude;
                acceleration += distance.normalized * strength;
            }
        }

        return acceleration;
    }

    public void ChangeVelocity(List<OrbitObject> orbitObjects, float timeStep)
    {
        velocity += CalculateAcceleration(orbitObjects) * timeStep;
    }

    public void ChangePosition(float timeStep)
    {
        positions.Add(lastPos);
        lastPos += velocity * timeStep;
    }

    public void Finalise()
    {
        if (lineRenderer == null) return;
        lineRenderer.positionCount = positions.Count;
        for (int i = 0 ; i < positions.Count; i++)
        {
            lineRenderer.SetPosition(i, positions[i]);
        }
    }

    public void SetActive(bool active)
    {
        if (lineRenderer == null) return;
        lineRenderer.gameObject.SetActive(active);
    }
}