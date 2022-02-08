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

    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount);

        meshRenderer.material.color = Color.Lerp(Color.red, Color.green, HealthPercent());

        controller.SetHostile(true);
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
