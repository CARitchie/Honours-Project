using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerTrigger : MonoBehaviour
{
    [SerializeField] UnityEvent OnEnter;
    [SerializeField] UnityEvent OnExit;


    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody == null) return;

        if (other.attachedRigidbody.TryGetComponent(out PlayerDetails player))
        {
            OnEnter?.Invoke();          // Invoke the OnEnter event if the player enters the collider
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody == null) return;

        if (other.attachedRigidbody.TryGetComponent(out PlayerDetails player))
        {
            OnExit?.Invoke();           // Invoke the OnExit event if the player exits the collider
        }
    }
}
