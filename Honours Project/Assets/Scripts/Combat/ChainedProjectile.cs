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
        pool = ObjectPool.GetPool(chainerKey);
    }

    public override void HitSuccess(RaycastHit hit, Vector3 direction)
    {
        PlaceHitMarker(hit, direction);

        OnHit?.Invoke();

        Collider[] hits = Physics.OverlapSphere(hit.point, radius);
        List<EnemyDetails> enemies = new List<EnemyDetails>();
        foreach(Collider collider in hits)
        {
            EnemyDetails enemy = collider.GetComponentInParent<EnemyDetails>();
            if (enemy != null && !enemies.Contains(enemy)) enemies.Add(enemy);
        }

        if(pool != null)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                Chainer chain = pool.GetObject().GetComponent<Chainer>();
                if(chain != null)
                {
                    chain.transform.position = transform.position;
                    chain.StartChase(enemies[i], damage * damageMultiplier);
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
