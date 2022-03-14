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
    int powerCells = 0;

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
        if(!UseEnergy(energyDrainRate * Time.deltaTime))
        {
            TakeDamage(energyDrainRate * Time.deltaTime * 2);
        }
    }

    public override bool TakeDamage(float amount)
    {
        amount *= Random.Range(0.9f, 1.05f);
        bool val = base.TakeDamage(amount);

        hud.SetHealthPercent(HealthPercent());

        return val;
    }

    public bool UseEnergy(float amount)
    {
        if (energy <= 0)
        {
            if(powerCells <= 0) return false;
            else
            {
                energy += maxEnergy;
                powerCells--;
                hud.SetNumberOfPowerCells(powerCells);
            }
        }

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

    public void AddPowerCell()
    {
        powerCells++;
        hud.SetNumberOfPowerCells(powerCells);
    }

    public void Recharge(float amount)
    {
        energy = Mathf.Clamp(energy + amount, 0, maxEnergy);
    }
}
