using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthItem : MonoBehaviour
{
    [SerializeField] float amount = 30;

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody == null) return;
        if(other.attachedRigidbody.TryGetComponent(out PlayerDetails player))
        {
            if(player.HealUp(amount)) Useful.DestroyGameObject(gameObject);
        }
    }
}
