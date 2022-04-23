using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonDetails : MonoBehaviour, Damageable
{
    [SerializeField] protected float maxHealth = 100;
    protected float health = 100;
    protected bool immune = false;
    protected bool alive = true;

    protected virtual void Awake()
    {
        health = maxHealth;
    }

    // Function to handle taking damage
    // Returns false if the damage was fatal
    public virtual bool TakeDamage(float amount)
    {
        if (immune) return false;
        health -= amount;
        if (health <= 0)
        {
            OnDeath();
            return false;
        }
        return true;
    }

    public virtual void OnDeath()
    {
    }

    public virtual void OnShot(float damage, Transform origin)
    {
        TakeDamage(damage);
    }

    public virtual void OnExplosion(float damage)
    {
        TakeDamage(damage);
    }

    public virtual float HealthPercent()
    {
        return health / maxHealth;
    }

    public void SetImmune(bool val)
    {
        immune = val;
    }

    // Function to increase the person's health
    // Returns true if successful
    public virtual bool HealUp(float amount)
    {
        if (health >= maxHealth) return false;
        health += amount;
        if (health > maxHealth) health = maxHealth;
        return true;
    }

    public virtual void OnMelee(float damage, Transform origin)
    {
        TakeDamage(damage);
    }

    public float GetHealth()
    {
        return health;
    }
}
