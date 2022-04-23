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
    [SerializeField] int relativeBody;
    [SerializeField] Transform holder;
    [SerializeField] List<OrbitObject> orbitObjects = new List<OrbitObject>();

    Vector3 origin;
    bool relative = false;

    private void OnValidate()
    {
        UpdateOrbits();
        StartFollow();
    }

    private void Start()
    {
        if (relativeBody >= 0 && relativeBody < orbitObjects.Count)                 // If a relative body is to be used
        {
            transform.parent = orbitObjects[relativeBody].SourceTransform();        // Move the visualisers into the relative body
        }
    }

    public void UpdateOrbits()
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

        if (relativeBody >= 0 && relativeBody < orbitObjects.Count)     // If a relative body is to be used
        {
            origin = orbitObjects[relativeBody].GetPosition();          // Set the origin to the relative body's position
            relative = true;
        }
        else
        {
            relative = false;
        }

        for (int i = 0; i < numberOfPoints; i++)
        {
            foreach (OrbitObject orbitObject in orbitObjects)
            {
                orbitObject.ChangeVelocity(orbitObjects, timeStep);     // Alter the velocity of all the visualisers
            }

            foreach (OrbitObject orbitObject in orbitObjects)
            {
                if (!relative)
                {
                    orbitObject.ChangePosition(timeStep);               // Change the position of all of the visualisers
                }
                else
                {
                    orbitObject.ChangePosition(timeStep, orbitObjects[relativeBody].GetLastPos() - origin);     // Change the position of all of the visualisers relative to the relative body
                }
            }
        }

        foreach(OrbitObject orbitObject in orbitObjects)
        {
            orbitObject.Finalise();
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
            if (relativeBody >= 0 && relativeBody < orbitObjects.Count)
            {
                origin = orbitObjects[relativeBody].GetPosition();
                relative = true;
            }

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
            followCamera.transform.position = orbitObjects[followTarget].GetCameraPosition();
            if (relative)
            {
                holder.position = orbitObjects[relativeBody].GetPosition() - origin;
            }
            yield return new WaitForFixedUpdate();
        }
    }
}

[System.Serializable]
class OrbitObject
{
    [SerializeField] PlanetGravity source;
    [SerializeField] LineRenderer lineRenderer;

    PlanetReceiver receiver;

    List<Vector3> positions = new List<Vector3>();

    float mass;
    Vector3 lastPos;
    Vector3 velocity;

    public Transform SourceTransform()
    {
        return source.transform;
    }

    public void Initialise()
    {
        if (source == null || lineRenderer == null) return;

        SetActive(true);
        receiver = source.GetComponent<PlanetReceiver>();
        mass = source.CalculateMass();
        lastPos = source.transform.position;
        velocity = receiver.GetVelocity();
        positions = new List<Vector3>();
    }

    public Vector3 GetPosition()
    {
        return source.transform.position;
    }

    public Vector3 GetCameraPosition()
    {
        return source.transform.position + Vector3.up * source.GetDistance() * 10;
    }

    public Vector3 GetLastPos()
    {
        return lastPos;
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

                float strength = (G * orbitObjects[i].mass) / distance.sqrMagnitude;        // Use the law of gravitation to work out acceleration
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

    public void ChangePosition(float timeStep, Vector3 difference)
    {
        positions.Add(lastPos - difference);
        lastPos += velocity * timeStep;
    }

    public void Finalise()
    {
        if (lineRenderer == null) return;
        lineRenderer.positionCount = positions.Count;
        for (int i = 0 ; i < positions.Count; i++)
        {
            lineRenderer.SetPosition(i, positions[i]);      // Use all positions as points in the line renderer
        }
    }

    public void SetActive(bool active)
    {
        if (lineRenderer == null) return;
        lineRenderer.gameObject.SetActive(active);
    }
}