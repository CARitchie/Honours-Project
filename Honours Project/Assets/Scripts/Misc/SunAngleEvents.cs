using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
            if(dot >= angle)
            {
                activated = false;
                endEvent?.Invoke();
            }
        }
        else
        {
            if(dot < angle)
            {
                activated = true;
                startEvent?.Invoke();
            }
        }
    }

    float GetCurrentDot()
    {
        Vector3 dir1 = (transform.position - planet.position).normalized;
        Vector3 dir2 = (sun.position - planet.position).normalized;
        return Vector3.Dot(dir1, dir2);
    }
}
