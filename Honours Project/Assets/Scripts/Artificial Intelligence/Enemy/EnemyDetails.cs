using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class EnemyDetails : PersonDetails
{
    [SerializeField] UnityEvent Death;
    EnemyController controller;

    EnemyWave wave;
    EnemySpawnPoint spawnPoint;

    protected override void Awake()
    {
        base.Awake();
        controller = GetComponentInChildren<EnemyController>();
    }

    // Function to apply damage
    public override bool TakeDamage(float amount)
    {
        if (!base.TakeDamage(amount)) return false;     // Return if the enemy dies

        controller.SetAnimTrigger("Take Damage");       // Play the damage animation

        controller.SetHostile(true);                    // Become hostile

        return true;
    }

    // Function to handle death
    public override void OnDeath()
    {
        if (!alive) return;
        alive = false;

        if (wave != null) wave.CheckComplete();         // If the enemy is part of a wave, make the wave determine whether it has been completed

        controller.SetActive(false);                    // Disable the controller
        controller.SetAnimTrigger("Die");               // Play the death animation
        if (spawnPoint != null) spawnPoint.OnDeath();
        else StartCoroutine(Despawn());
        Death?.Invoke();
    }

    public bool IsAlive()
    {
        return health > 0;
    }

    public void SetWave(EnemyWave wave, EnemySpawnPoint spawnPoint)
    {
        this.wave = wave;
        this.spawnPoint = spawnPoint;
    }

    // Function to handle despawning
    IEnumerator Despawn()
    {
        yield return new WaitForSeconds(10);

        Transform player = PlayerController.Instance.transform;

        bool loop = true;

        while (loop)
        {
            float distance = (player.position - transform.position).sqrMagnitude;

            // If the player isn't too close and/or they are not looking, stop looping
            if (distance > 10000) loop = false;
            else if (distance > 16 && Vector3.Dot(player.forward, (transform.position - player.position).normalized) < -0.2f) loop = false;
            
            yield return new WaitForEndOfFrame();
        }

        Debug.Log(gameObject + " was despawned");
        DestroyEnemy();         // Despawn the enemy
    }

    public void DestroyEnemy()
    {
        Useful.DestroyGameObject(gameObject);
    }
}
