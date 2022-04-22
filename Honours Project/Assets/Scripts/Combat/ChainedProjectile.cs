using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainedProjectile : Projectile
{
    [Header("Chained Settings")]
    [SerializeField] float radius;
    [SerializeField] string chainerKey;

    ObjectPool pool;

    protected override void Start()
    {
        base.Start();
        pool = ObjectPool.GetPool(chainerKey);          // Store the pool that contains the sub projectiles
    }

    // Function that is called when the projectile successfuly hits something
    public override void HitSuccess(RaycastHit hit, Vector3 direction)
    {
        PlaceHitMarker(hit, direction);

        OnHit?.Invoke();

        Collider[] hits = Physics.OverlapSphere(hit.point, radius);                     // Find all collisions within the radius
        List<EnemyDetails> enemies = new List<EnemyDetails>();
        foreach(Collider collider in hits)
        {
            EnemyDetails enemy = collider.GetComponentInParent<EnemyDetails>();
            if (enemy != null && !enemies.Contains(enemy)) enemies.Add(enemy);          // If the collision was an enemy, add them to the list
        }

        if(pool != null)
        {
            for (int i = 0; i < enemies.Count; i++)                                     // For every enemy that was collided with
            {
                Chainer chain = pool.GetObject().GetComponent<Chainer>();               // Take a new sub projectile from the pool
                if(chain != null)
                {
                    chain.transform.position = transform.position;
                    chain.StartChase(enemies[i], damage * damageMultiplier);            // Tell the sub projectile to chase the enemy
                }
            }
        }



        gameObject.SetActive(false);

    }

    protected override void Despawn()
    {
        RaycastHit hit = new RaycastHit();
        hit.point = transform.position;
        HitSuccess(hit, Vector3.one);
    }
}
