using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetails : PersonDetails
{
    EnemyController controller;

    EnemyWave wave;

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
        if (wave != null) wave.CheckComplete();
        Destroy(gameObject);
    }

    public bool IsAlive()
    {
        return health > 0;
    }

    public void SetWave(EnemyWave wave)
    {
        this.wave = wave;
    }
}
