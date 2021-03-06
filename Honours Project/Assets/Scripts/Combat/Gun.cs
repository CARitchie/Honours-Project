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

    int _MaxAmmo = -450;   // Actual max ammo

    int currentAmmo = -450;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        recoil = GetComponentInParent<Recoil>();
        audioManager = GetComponent<AudioManager>();
        if(_MaxAmmo == -450) _MaxAmmo = maxAmmo;
        if(currentAmmo == -450 || currentAmmo > _MaxAmmo) currentAmmo = _MaxAmmo;
    }

    private void Start()
    {
        projectilePool = ObjectPool.GetPool(projectileKey);             // Store the object pool that is used for the type of projectile used by this gun   
    }

    public override void OnEquip(PersonController controller)
    {
        base.OnEquip(controller);
    }

    // Use the gun's primary action, shoot
    public override void PrimaryAction(float val)
    {
        if(val > 0)
        {
            if (fired && !automatic) return;                        // Return if the gun has been fired but is not automatic

            if(delayTimer > 0)
            {
                delayTimer -= Time.deltaTime;
                return;
            }

            if(currentAmmo > 0 || maxAmmo == -450)                  // If there is still ammunition
            {
                if (maxAmmo != -450) currentAmmo--;                 // Decrease the amount of ammo if there is not an infinite amount

                Fire();

                if (automatic) delayTimer = automaticDelay;         // Reset the delay
            }

        }
        else
        {
            if(fired && animator != null)
            {
                animator.ResetTrigger("Fire");      // If the fire animation trigger is still active, reset it
            }

            fired = false;
            delayTimer = 0;
        }

    }

    // Function for a secondary action
    // Could have been used for aiming down the weapons sights, or some other ability
    // Ultimately not used
    public override void SecondaryAction(float val)
    {
        if (val > 0)
        {
        }
        else
        {
        }
    }

    // Function to fire a projectile from the gun
    protected virtual void Fire()
    {
        fired = true;
        controller.Recoil(recoilStrength);                                                  // Add recoil to the enemy/player

        if (projectilePool == null) return;
        
        Projectile projectile = projectilePool.GetObject().GetComponent<Projectile>();      // Retrieve a projectile from the pool

        projectile.transform.position = firePoint.position;                                 // Move the projectile to the gun's muzzle

        Vector3 direction = controller.GetAimDirection(firePoint);                          // Find the direction that the gun should be aiming at
        direction = SpreadAim(direction);                                                   // Add some spread to the direction
        Vector3 velocity = controller.GetVelocity() + direction * projectileSpeed;          // Create the velocity for the projectile

        GravitySource source = controller.GetNearestSource();
        Transform body = source != null ? source.transform : null; 

        projectile.Fire(velocity, body, transform.parent, damageMultiplier);                // Fire the projectile

        FireEffects();
    }

    public void AimAt(Vector3 point)
    {
        firePoint.LookAt(point);
    }

    // Function to add some random spread to an aim direction
    public Vector3 SpreadAim(Vector3 baseDirection)
    {
        Vector3 spread = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0) * spreadSize / 100;
        return (baseDirection + spread).normalized;
    }

    public void SetMaxAmmoMultiplier(float percent)
    {
        _MaxAmmo = (int)(maxAmmo * percent);
    }

    // Function to play the special effects when firing a projectile
    protected void FireEffects()
    {
        if (animator != null) animator.SetTrigger("Fire");          // Play the gun's fire animation
        if (muzzleFlash != null) muzzleFlash.Play();                // Play the muzzle flash particle system

        if (recoil != null) recoil.RecoilFire(recoilDirection);     // Add a recoil screen shake
        if (audioManager != null) audioManager.PlaySound("Fire");   // Play a gun shot sound effect
    }

    public override string GetAmmoText()
    {
        return currentAmmo.ToString() + "/" + _MaxAmmo.ToString();
    }

    public override bool IsInfinite()
    {
        return maxAmmo == -450;
    }

    public override bool AddAmmo(float percentOfMax)
    {
        if (_MaxAmmo == -450) _MaxAmmo = maxAmmo;
        if (currentAmmo == _MaxAmmo || IsInfinite()) return false;
        currentAmmo = (int)Mathf.Clamp(currentAmmo + percentOfMax * _MaxAmmo, 0, _MaxAmmo);
        return true;
    }

    public override int GetAmmo()
    {
        return currentAmmo;
    }

    public override void SetAmmo(int ammo)
    {
        currentAmmo = ammo;
    }
}
