using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowImprinter : MonoBehaviour
{
    [SerializeField] float distanceDown;
    [SerializeField] float imprintSize = 150;

    int layerMask = 1 << 8;

    public bool Contact(out Vector3 pos)
    {
        pos = Vector3.zero;
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, distanceDown, layerMask))
        {
            if (!hit.collider.CompareTag("Snow")) return false;
            pos = hit.point;
            return true;
        }
        return false;
    }

    public float GetSize()
    {
        return imprintSize;
    }
}
