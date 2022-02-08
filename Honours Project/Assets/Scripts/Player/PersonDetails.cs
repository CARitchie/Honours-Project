using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersonDetails : MonoBehaviour, Damageable
{
    [SerializeField] protected float maxHealth = 100;
    protected float health = 100;

    protected virtual void Awake()
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
        SceneManager.LoadScene("Space");
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
