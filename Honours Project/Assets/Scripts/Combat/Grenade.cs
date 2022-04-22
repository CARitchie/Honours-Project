using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : Projectile
{
    [Header("Grenade Settings")]
    [SerializeField] float radius;
    [SerializeField] float strength;

    // Function that is called when the projectile successfuly hits something
    public override void HitSuccess(RaycastHit hit, Vector3 direction)
    {
        PlaceHitMarker(hit, direction);

        OnHit?.Invoke();

        Collider[] hits = Physics.OverlapSphere(hit.point, radius);                     // Find all collisions within a radius
        List<Rigidbody> bodies = new List<Rigidbody>();
        foreach(Collider collider in hits)
        {
            Rigidbody rb = collider.GetComponentInParent<Rigidbody>();
            if (rb != null && !bodies.Contains(rb)) bodies.Add(rb);
        }

        foreach(Rigidbody rb in bodies)
        {
            if(!rb.isKinematic) rb.AddExplosionForce(strength, hit.point, radius);      // Add a force to all rigidbodies within the radius
            if (rb.TryGetComponent(out Damageable damageable))                          // If the rigidbody can take damage
            {
                damageable.OnExplosion(damage * damageMultiplier);                      // Add explosion damage to the object
            }
        }

        gameObject.SetActive(false);

    }

    protected override void Despawn()
    {
        RaycastHit hit = new RaycastHit();
        hit.point = transform.position;
        HitSuccess(hit, Vector3.one);
    }
}
