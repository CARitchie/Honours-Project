using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShipPodTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (GetComponentInChildren<CryoPod>() != null) return;

        if (other.attachedRigidbody.TryGetComponent(out CryoPod pod))
        {
            pod.AttachToTransform(transform);
            pod.StartAlign();
        }
    }
}
