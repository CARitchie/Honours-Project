using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnedSafely : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody == null) return;
        if (other.attachedRigidbody.CompareTag("Player"))
        {
            FindObjectOfType<EndFight>()?.OnPlayerReturn();
        }
    }
}
