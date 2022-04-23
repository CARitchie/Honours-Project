using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NearObject : MonoBehaviour
{
    bool tempDisabled = false;

    private void OnTriggerEnter(Collider other)
    {
        if (tempDisabled || other.attachedRigidbody == null) return;

        if (other.attachedRigidbody.TryGetComponent(out PlayerDetails player))
        {
            Compass.AddNearObject(transform);           // If the player has come close enough, add this gameobject to the compass' list of objects to cause a pulse
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody == null) return;

        if (other.attachedRigidbody.TryGetComponent(out PlayerDetails player))
        {
            Compass.RemoveNearObject(transform);        // If the player is no longer close enough, remove this gameobject from the compass' list
        }
    }

    private void OnDestroy()
    {
        Compass.RemoveNearObject(transform);
    }

    public void Disable()
    {
        tempDisabled = true;
        Compass.RemoveNearObject(transform);
    }

    public void Enable()
    {
        tempDisabled = false;
    }
}
