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
    float fullMaxHealth;
    float fullMaxEnergy;
    int powerCells = 0;

    private void Start()
    {
        fullMaxHealth = maxHealth;
        fullMaxEnergy = maxEnergy;

        if (SaveManager.SaveExists())
        {
            health = SaveManager.save.GetHealth();
            energy = SaveManager.save.GetEnergy();
            powerCells = SaveManager.save.NumberOfCells();
        }
        else
        {
            energy = maxEnergy;
        } 

        hud.SetHealthPercent(HealthPercent());
        hud.SetEnergyPercent(EnergyPercent);
        hud.SetNumberOfPowerCells(powerCells);

        InputController.GodMode += ToggleGodMode;

        SaveManager.OnUpgradeChanged += LoadUpgrades;
        LoadUpgrades();
    }

    private void OnDestroy()
    {
        InputController.GodMode -= ToggleGodMode;
        SaveManager.OnUpgradeChanged -= LoadUpgrades;
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
        hud.SetEnergyPercent(EnergyPercent);
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

    public override void OnShot(float damage, Transform origin)
    {
        base.OnShot(damage, origin);
        HUD.AddDamageIndicator(origin);
    }

    public override void OnMelee(float damage, Transform origin)
    {
        base.OnMelee(damage, origin);
        HUD.AddDamageIndicator(origin);
    }

    public float GetEnergy()
    {
        return energy;
    }

    public int NumberOfCells()
    {
        return powerCells;
    }

    public override void OnDeath()
    {
        immune = true;
        SaveManager.LoadGame();
        SceneManager.FadeToScene("Space");
    }

    void LoadUpgrades()
    {
        if (SaveManager.SacrificeMade("sacrifice_health"))
        {
            maxHealth = 0.7f * fullMaxHealth;
            if (health > maxHealth)
            {
                health = maxHealth;
                hud.SetHealthPercent(HealthPercent());
            }
            HUD.SetReducedMaxHealth();
        }

        if (SaveManager.SacrificeMade("sacrifice_energy"))
        {
            maxEnergy = 0.7f * fullMaxEnergy;
            if (energy > maxEnergy)
            {
                energy = maxEnergy;
                hud.SetEnergyPercent(EnergyPercent);
            }
            HUD.SetReducedMaxEnergy();
        }
    }

    public override float HealthPercent()
    {
        return health / fullMaxHealth;
    }

    float EnergyPercent { get { return energy / fullMaxEnergy; } }
}
