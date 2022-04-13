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

    public override bool TakeDamage(float amount)
    {
        if (!base.TakeDamage(amount)) return false;

        controller.SetAnimTrigger("Take Damage");

        controller.SetHostile(true);

        return true;
    }

    public override void OnDeath()
    {
        if (!alive) return;
        alive = false;

        if (wave != null) wave.CheckComplete();

        controller.SetActive(false);
        controller.SetAnimTrigger("Die");
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

    IEnumerator Despawn()
    {
        yield return new WaitForSeconds(10);

        Transform player = PlayerController.Instance.transform;

        bool loop = true;

        while (loop)
        {
            float distance = (player.position - transform.position).sqrMagnitude;

            if (distance > 10000) loop = false;
            else if (distance > 16 && Vector3.Dot(player.forward, (transform.position - player.position).normalized) < -0.2f) loop = false;
            
            yield return new WaitForEndOfFrame();
        }

        Debug.Log(gameObject + " was despawned");
        DestroyEnemy();
    }

    public void DestroyEnemy()
    {
        Useful.DestroyGameObject(gameObject);
    }
}
