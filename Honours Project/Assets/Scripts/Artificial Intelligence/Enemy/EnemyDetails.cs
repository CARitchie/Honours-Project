using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetails : PersonDetails
{
    MeshRenderer meshRenderer;
    EnemyController controller;

    EnemyWave wave;

    protected override void Awake()
    {
        base.Awake();
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        controller = GetComponentInChildren<EnemyController>();
    }

    public override bool TakeDamage(float amount)
    {
        if (!base.TakeDamage(amount)) return false;

        controller.SetAnimTrigger("Take Damage");

        meshRenderer.material.color = Color.Lerp(Color.red, Color.green, HealthPercent());

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
