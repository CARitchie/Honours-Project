using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Weapon
{
    [SerializeField] protected float projectileSpeed;
    [SerializeField] [Tooltip("Set to -450 for infinite ammo")] int maxAmmo;   // Default max ammo
    [SerializeField] protected float recoilStrength;
    [SerializeField] Vector3 recoilDirection;
    [SerializeField] bool automatic = false;
    [SerializeField] float automaticDelay;
    [SerializeField] float spreadSize;
    [SerializeField] string projectileKey;
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] protected Transform firePoint;

    Recoil recoil;
    protected bool fired = false;

    float delayTimer = 0;

    protected ObjectPool projectilePool;
    Animator animator;
    AudioManager audioManager;

    int _MaxAmmo;   // Actual max ammo

    int currentAmmo;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        recoil = GetComponentInParent<Recoil>();
        audioManager = GetComponent<AudioManager>();
        _MaxAmmo = maxAmmo;
        currentAmmo = _MaxAmmo;
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

            if(currentAmmo > 0 || maxAmmo == -450)
            {
                if (maxAmmo != -450) currentAmmo--;

                Fire();

                if (automatic) delayTimer = automaticDelay;
            }

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

    protected virtual void Fire()
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

        projectile.Fire(velocity, body, transform.parent, damageMultiplier);

        FireEffects();
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

    public void SetMaxAmmoMultiplier(float percent)
    {
        _MaxAmmo = (int)(maxAmmo * percent);
    }

    protected void FireEffects()
    {
        if (animator != null) animator.SetTrigger("Fire");
        if (muzzleFlash != null) muzzleFlash.Play();

        if (recoil != null) recoil.RecoilFire(recoilDirection);
        if (audioManager != null) audioManager.PlaySound("Fire");
    }

    public override string GetAmmoText()
    {
        return currentAmmo.ToString() + "/" + _MaxAmmo.ToString();
    }

    public override bool IsInfinite()
    {
        return maxAmmo == -450;
    }
}
