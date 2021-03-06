using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtZone : MonoBehaviour
{
    [SerializeField] float damage;
    List<Damageable> damaged = new List<Damageable>();

    private void Update()
    {
        for(int i = 0; i < damaged.Count; i++)
        {
            // Remove the damageable object if it no longer exists
            if(damaged[i] == null)
            {
                damaged.RemoveAt(i);
                i--;
                continue;
            }

            damaged[i].OnMelee(damage * Time.deltaTime, transform);         // Damage any damageable objects within the zone
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody == null) return;
        if (other.attachedRigidbody.TryGetComponent(out Damageable damageable))
        {
            if (damageable == GetComponentInParent<Damageable>()) return;

            if (!damaged.Contains(damageable)) damaged.Add(damageable);     // Add the new damageable object to the list

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody == null) return;
        if (other.attachedRigidbody.TryGetComponent(out Damageable damageable))
        {
            if (damageable == GetComponentInParent<Damageable>()) return;

            if (damaged.Contains(damageable)) damaged.Remove(damageable);   // Remove the damageable object from the list

        }
    }
}
