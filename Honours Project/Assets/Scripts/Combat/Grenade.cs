using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : Projectile
{
    [Header("Grenade Settings")]
    [SerializeField] float radius;
    [SerializeField] float strength;

    public override void HitSuccess(RaycastHit hit, Vector3 direction)
    {
        PlaceHitMarker(hit, direction);

        OnHit?.Invoke();

        Collider[] hits = Physics.OverlapSphere(hit.point, radius);
        List<Rigidbody> bodies = new List<Rigidbody>();
        foreach(Collider collider in hits)
        {
            Rigidbody rb = collider.GetComponentInParent<Rigidbody>();
            if (rb != null && !bodies.Contains(rb)) bodies.Add(rb);
        }

        foreach(Rigidbody rb in bodies)
        {
            if(!rb.isKinematic) rb.AddExplosionForce(strength, hit.point, radius);
            if(rb.TryGetComponent(out Damageable damageable))
            {
                damageable.OnExplosion(damage * damageMultiplier);
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
