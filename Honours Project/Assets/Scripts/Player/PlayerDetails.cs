using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetails : PersonDetails
{
    [SerializeField] float maxEnergy = 200;
    [SerializeField] float energyDrainRate;
    [SerializeField] HUD hud;
    float energy;

    private void Start()
    {
        hud.SetHealthPercent(health / maxHealth);

        energy = maxEnergy;
        hud.SetEnergyPercent(energy / maxEnergy);
    }

    private void Update()
    {
        UseEnergy(energyDrainRate * Time.deltaTime);
    }

    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount);

        hud.SetHealthPercent(health / maxHealth);
    }

    public bool UseEnergy(float amount)
    {
        if (energy <= 0) return false;

        energy -= amount;
        hud.SetEnergyPercent(energy / maxEnergy);
        return true;
    }
}
