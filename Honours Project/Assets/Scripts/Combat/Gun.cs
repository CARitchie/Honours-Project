using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Weapon
{
    [SerializeField] float projectileSpeed;
    [SerializeField] int maxAmmo;
    [SerializeField] float recoilStrength;
    [SerializeField] Vector3 recoilDirection;
    [SerializeField] bool automatic = false;
    [SerializeField] float automaticDelay;
    [SerializeField] float spreadSize;
    [SerializeField] string projectileKey;
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] protected Transform firePoint;

    Recoil recoil;
    bool fired = false;

    float delayTimer = 0;

    ObjectPool projectilePool;
    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        recoil = GetComponentInParent<Recoil>();
    }

    private void Start()
    {
        projectilePool = ObjectPool.GetPool(projectileKey);
    }

    public override void OnEquip(PersonController controller)
    {
        base.OnEquip(controller);
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
            if(fired && animator != null)
            {
                animator.ResetTrigger("Fire");
            }

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

        projectile.transform.position = firePoint.position;

        Vector3 direction = controller.GetAimDirection(firePoint);
        direction = SpreadAim(direction);
        Vector3 velocity = controller.GetVelocity() + direction * projectileSpeed;

        GravitySource source = controller.GetNearestSource();
        Transform body = source != null ? source.transform : null; 

        projectile.Fire(velocity, body);
        if(animator != null) animator.SetTrigger("Fire");
        if(muzzleFlash != null) muzzleFlash.Play();

        if (recoil != null) recoil.RecoilFire(recoilDirection);
    }

    public void AimAt(Vector3 point)
    {
        firePoint.LookAt(point);
    }

    public Vector3 SpreadAim(Vector3 baseDirection)
    {
        Vector3 spread = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0) * spreadSize / 100;
        return (baseDirection + spread).normalized;
    }

}
