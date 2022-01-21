using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetails : PersonDetails
{
    MeshRenderer meshRenderer;

    private void Start()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount);

        meshRenderer.material.color = Color.Lerp(Color.red, Color.green, HealthPercent());

    }

    public override void OnDeath()
    {
        Destroy(gameObject);
    }
}
