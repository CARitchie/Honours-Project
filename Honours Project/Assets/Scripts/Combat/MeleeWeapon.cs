using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    [SerializeField] float damage;
    Collider hitBox;

    private void Start()
    {
        hitBox = GetComponent<Collider>();
        hitBox.enabled = false;
    }

    public void Activate()
    {
        hitBox.enabled = true;
    }

    public void Deactivate()
    {
        hitBox.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody == null) return;

        if(other.attachedRigidbody.TryGetComponent(out Damageable damageable))
        {
            if (damageable == GetComponentInParent<Damageable>()) return;

            damageable.OnMelee(damage, transform);

            // For some reason this needs to be checked again here
            // It's possible for a null reference exception to occur without it
            if (other.attachedRigidbody == null) return;

            other.attachedRigidbody.AddForce(transform.forward * 5, ForceMode.VelocityChange);
            
        }
    }
}
