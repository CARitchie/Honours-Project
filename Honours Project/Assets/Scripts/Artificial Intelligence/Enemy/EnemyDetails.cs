using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetails : PersonDetails
{
    MeshRenderer meshRenderer;
    EnemyController controller;

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
        Destroy(gameObject);
    }
}
