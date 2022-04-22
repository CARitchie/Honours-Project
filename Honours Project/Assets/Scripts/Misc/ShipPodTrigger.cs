using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShipPodTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (GetComponentInChildren<CryoPod>() != null || other.attachedRigidbody == null) return;
        if (other.attachedRigidbody.TryGetComponent(out CryoPod pod))       // If a cryopod activates the trigger
        {
            pod.AttachToTransform(transform);                               // Attach the cryopod to this gameobject
            pod.StartAlign();                                               // Align the cryopod with this gameobject
        }
    }
}
