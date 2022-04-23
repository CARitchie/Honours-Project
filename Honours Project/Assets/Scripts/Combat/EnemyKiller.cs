using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class creates an area that will kill any enemies that enter
public class EnemyKiller : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody == null) return;

        if(other.attachedRigidbody.TryGetComponent(out EnemyDetails details))
        {
            details.OnDeath();
            Debug.Log("An enemy fell into the planet");
        }
    }
}
