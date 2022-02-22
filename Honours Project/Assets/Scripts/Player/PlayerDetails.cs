using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetails : PersonDetails
{
    [SerializeField] float maxEnergy = 200;
    [SerializeField] float energyDrainRate;
    [SerializeField] HUD hud;
    [Header("Temporary")] [SerializeField] GameObject shipCompass;
    float energy;

    private void Start()
    {
        hud.SetHealthPercent(HealthPercent());

        energy = maxEnergy;
        hud.SetEnergyPercent(energy / maxEnergy);

        InputController.GodMode += ToggleGodMode;
    }

    private void OnDestroy()
    {
        InputController.GodMode -= ToggleGodMode;
    }

    private void Update()
    {
        UseEnergy(energyDrainRate * Time.deltaTime);
    }

    public override bool TakeDamage(float amount)
    {
        bool val = base.TakeDamage(amount);

        hud.SetHealthPercent(HealthPercent());

        return val;
    }

    public bool UseEnergy(float amount)
    {
        if (energy <= 0) return false;

        energy -= amount;
        hud.SetEnergyPercent(energy / maxEnergy);
        return true;
    }

    public override bool HealUp(float amount)
    {
        bool healed = base.HealUp(amount);
        hud.SetHealthPercent(HealthPercent());
        return healed;
    }

    void ToggleGodMode()
    {
        immune = !immune;
        shipCompass.SetActive(immune);
    }
}
