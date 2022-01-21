using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonDetails : MonoBehaviour, Damageable
{
    [SerializeField] protected float maxHealth = 100;
    protected float health;

    private void Awake()
    {
        health = maxHealth;
    }

    public virtual void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0) OnDeath();
    }

    public virtual void OnDeath()
    {

    }

    public virtual void OnShot(float damage)
    {
        TakeDamage(damage);
    }

    public float HealthPercent()
    {
        return health / maxHealth;
    }
}
