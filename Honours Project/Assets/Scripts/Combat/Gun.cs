using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Weapon
{
    [SerializeField] float projectileSpeed;
    [SerializeField] int maxAmmo;
    [SerializeField] float recoilStrength;
    [SerializeField] bool automatic = false;
    [SerializeField] float automaticDelay;
    [SerializeField] string projectileKey;

    bool fired = false;

    float delayTimer = 0;

    protected Transform projectileOrigin;

    ObjectPool projectilePool;

    private void Start()
    {
        projectilePool = ObjectPool.GetPool(projectileKey);
    }

    public override void OnEquip(PersonController controller)
    {
        base.OnEquip(controller);
        projectileOrigin = controller.ProjectileSpawnPoint();
    }

    public override void PrimaryAction(float val)
    {
        if(val > 0)
        {
            if (fired && !automatic) return;

            if(delayTimer > 0)
            {
                delayTimer -= Time.deltaTime;
                return;
            }

            Fire();

            if (automatic) delayTimer = automaticDelay;
        }
        else
        {
            fired = false;
            delayTimer = 0;
        }

    }

    public override void SecondaryAction(float val)
    {
        if (val > 0)
        {
        }
        else
        {
        }
    }

    public void Fire()
    {
        fired = true;
        controller.Recoil(recoilStrength);

        if (projectilePool == null) return;
        
        Projectile projectile = projectilePool.GetObject().GetComponent<Projectile>();

        projectile.transform.position = projectileOrigin.position + projectileOrigin.forward * 1.5f;

        Vector3 velocity = controller.GetVelocity() + projectileOrigin.forward * projectileSpeed;

        GravitySource source = controller.GetNearestSource();
        Transform body = source != null ? source.transform : null; 

        projectile.Fire(velocity, body);
    }

}
