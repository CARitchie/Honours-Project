using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        SceneManager.LoadScene("Space");
    }

    public virtual void OnShot(float damage)
    {
        TakeDamage(damage);
    }

    public virtual void OnExplosion(float damage)
    {
        TakeDamage(damage);
    }

    public float HealthPercent()
    {
        return health / maxHealth;
    }

    public void SetImmune(bool val)
    {
        immune = val;
    }

    public virtual bool HealUp(float amount)
    {
        if (health >= maxHealth) return false;
        health += amount;
        if (health > maxHealth) health = maxHealth;
        return true;
    }

    public void OnMelee(float damage)
    {
        TakeDamage(damage);
    }
}
