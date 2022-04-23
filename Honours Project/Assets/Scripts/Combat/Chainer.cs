using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chainer : PoolObject
{
    [SerializeField] ParticleSystem hitMarker;
    EnemyDetails target;
    float damage;

    // Function to set the chase target
    public void StartChase(EnemyDetails target, float damage)
    {
        this.target = target;
        this.damage = damage;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        if(target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, Time.deltaTime * 8);    // Move towards the target

            if((transform.position- target.transform.position).sqrMagnitude < 2)                                            // If close enough
            {
                target.TakeDamage(damage);                                                                                  // Damage the target
                gameObject.SetActive(false);

                if (hitMarker == null)
                {
                    Debug.LogWarning("No hitmarker present");
                    return;
                }
                hitMarker.transform.position = target.transform.position;                                                   // Move the hitmarker to the target
                hitMarker.transform.parent = target.transform;
                hitMarker.gameObject.SetActive(true);
                hitMarker.Play();
            }
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

}
