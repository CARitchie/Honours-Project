using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Gun
{
    [SerializeField] int numberOfProjectiles = 1;

    protected override void Fire()
    {
        fired = true;
        controller.Recoil(recoilStrength);

        if (projectilePool == null) return;

        Vector3 aimDirection = controller.GetAimDirection(firePoint);
        Vector3 playerVelocity = controller.GetVelocity();

        // Fire as many projectiels as has been specified
        for(int i = 0; i < numberOfProjectiles; i++)
        {
            Projectile projectile = projectilePool.GetObject().GetComponent<Projectile>();      // Retrieve a projectile from the pool

            projectile.transform.position = firePoint.position;


            Vector3 direction = SpreadAim(aimDirection);
            Vector3 velocity = playerVelocity + direction * projectileSpeed;

            GravitySource source = controller.GetNearestSource();
            Transform body = source != null ? source.transform : null;

            projectile.Fire(velocity, body, transform.parent, damageMultiplier);
        }

        FireEffects();
    }
}
