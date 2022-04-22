using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Class invokes events depending upon the time of day on a planet
public class SunAngleEvents : MonoBehaviour
{
    [SerializeField] Transform planet;
    [SerializeField] Transform sun;
    [SerializeField] float angle;
    [SerializeField] UnityEvent startEvent;
    [SerializeField] UnityEvent endEvent;

    bool activated = false;

    private void Start()
    {
        if(GetCurrentDot() < angle)
        {
            activated = true;
            startEvent?.Invoke();
        }
        else
        {
            activated = false;
            endEvent?.Invoke();
        }
    }

    private void Update()
    {
        float dot = GetCurrentDot();
        if (activated)
        {
            if(dot >= angle)                // If the dot is large enough and the startEvent has been activated
            {
                activated = false;
                endEvent?.Invoke();
            }
        }
        else
        {
            if(dot < angle)                 // If the dot is small enough and the endEvent has been activated
            {
                activated = true;
                startEvent?.Invoke();
            }
        }
    }

    // Function to find the dot product between the direction from the planet to this gameobject, and the direction from the planet to the sun
    float GetCurrentDot()
    {
        Vector3 dir1 = (transform.position - planet.position).normalized;
        Vector3 dir2 = (sun.position - planet.position).normalized;
        return Vector3.Dot(dir1, dir2);
    }
}
