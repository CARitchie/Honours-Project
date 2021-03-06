using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetails : PersonDetails
{
    [SerializeField] float maxEnergy = 200;
    [SerializeField] float energyDrainRate;
    [SerializeField] HUD hud;
    [SerializeField] bool finalFight;
    [Header("Temporary")] [SerializeField] GameObject shipCompass;
    float energy;
    float fullMaxHealth;
    float fullMaxEnergy;
    int powerCells = 0;

    float maxShield = 80;
    float shield = 80;
    const float shieldDelay = 4;
    float shieldTimer;
    bool shieldActive = false;

    bool solarPanel = false;
    bool nanites = false;
    float healTime = 0;

    int layerMask = ~(1 << 2 | 1 << 10 | 1 << 11 | 1 << 12);

    Transform sun;
    AudioManager audioManager;

    protected override void Awake()
    {
        base.Awake();
        audioManager = GetComponent<AudioManager>();
    }

    private void Start()
    {
        fullMaxHealth = maxHealth;
        fullMaxEnergy = maxEnergy;
        shieldTimer = shieldDelay;

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

        SceneManager.reload = false;
    }

    private void OnDestroy()
    {
        InputController.GodMode -= ToggleGodMode;
        SaveManager.OnUpgradeChanged -= LoadUpgrades;
    }

    private void Update()
    {
        if (!SolarPanelUsage(false))
        {
            if (!UseEnergy(energyDrainRate * Time.deltaTime))           // Slowly drain energy
            {
                TakeDamage(energyDrainRate * Time.deltaTime * 2);       // Slowly take damage if there is no more energy
            }
        }

        ShieldCooldown();

        NanitesCooldown();
    }

    // Function to handle things that should keep occurring even when the player is piloting the spaceship
    public void KeepLooping()
    {
        SolarPanelUsage(true);
        ShieldCooldown();
        NanitesCooldown();
    }

    // Function to recharge with a solar panel, and even faster when facing the sun
    bool SolarPanelUsage(bool inShip)
    {
        if (!solarPanel) return false;

        float rate = energyDrainRate * 2;

        // Find out if the player has line of sight with the sun
        if (!inShip && sun != null && Physics.Raycast(transform.position, sun.position - transform.position, out RaycastHit hit, (sun.position - transform.position).magnitude, layerMask))
        {
            if (hit.transform.CompareTag("Sun")) rate *= 5;
        }

        Recharge(rate * Time.deltaTime);

        return true;
    }

    void ShieldCooldown()
    {
        if (!shieldActive) return;

        if (shieldTimer > 0) shieldTimer -= Time.deltaTime;
        else IncreaseShield(6 * Time.deltaTime);
        
    }

    void NanitesCooldown()
    {
        if (!nanites) return;
        if (healTime > 0) healTime -= Time.deltaTime;
        else HealUp(4 * Time.deltaTime);
    }

    // Function to handle taking damage
    // Returns false if dead
    public override bool TakeDamage(float amount)
    {
        amount *= Random.Range(0.9f, 1.05f);        // Add a bit of randomness
        bool val = base.TakeDamage(amount);

        hud.SetHealthPercent(HealthPercent());
        healTime = 4;

        return val;
    }

    // Function to use energy
    // Returns false if no more energy can be used
    public bool UseEnergy(float amount)
    {
        if (energy <= 0)
        {
            if(powerCells <= 0) return false;
            else                                            // Activate a new power cell if at least one exists
            {
                audioManager.PlaySound("newCell");
                energy += maxEnergy;                        // Reset energy to the max
                powerCells--;                               // Reduce the number of power cells
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
        if (healed)
        {
            audioManager.PlaySound("heal");
        }
        return healed;
    }

    void ToggleGodMode()
    {
        immune = !immune;
        if(shipCompass != null) shipCompass.SetActive(immune);
    }

    public void AddPowerCell()
    {
        powerCells++;
        hud.SetNumberOfPowerCells(powerCells);
        audioManager.PlaySound("addPower");
    }

    public void Recharge(float amount)
    {
        if (energy == maxEnergy) return;
        energy = Mathf.Clamp(energy + amount, 0, maxEnergy);
        hud.SetEnergyPercent(EnergyPercent);
    }

    // Function to handle being shot
    public override void OnShot(float damage, Transform origin)
    {
        if (!shieldActive || !DecreaseShield(damage))
        {
            base.OnShot(damage, origin);
        }
        shieldTimer = shieldDelay;
        HUD.AddDamageIndicator(origin);         // Try to add a damage indicator pointing to the origin of the projectile
    }

    public override void OnMelee(float damage, Transform origin)
    {
        base.OnMelee(damage, origin);
        HUD.AddDamageIndicator(origin);         // Try to add a damage indicator pointing to the origin of the attack
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
        if (!finalFight)                                        // If not in the final fight
        {
            immune = true;                                      // Prevent the player from taking any more damage and potentially dying again
            PlayerController.SetPaused(true);                   // Prevent the player from making any more actions
            SaveManager.LoadGame();                             // Reload the save data, doesn't need to occur here but done so just to be safe
            SceneManager.reload = true;
            SceneManager.FadeToScene("Space");                  // Reload the scene
        }
        else
        {
            FindObjectOfType<EndFight>()?.OnPlayerDeath();      // If this is the final fight, activate the correct cutscene
        }

    }

    void LoadUpgrades()
    {
        // If the health sacrifice has been made, reduce the maximum health
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

        // If the energy sacrifice has been made, reduce the maximum energy
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

        // Enable healing nanites if unlocked
        if (SaveManager.SelfUpgraded("upgrade_nanites"))
        {
            nanites = true;
        }

        // Activate energy recharging if the solar panel is unlocked
        if (SaveManager.SelfUpgraded("upgrade_solar"))
        {
            sun = GlobalLightControl.SunTransform();
            solarPanel = true;
        }

        // Activat the shield if it has been unlocked
        if (SaveManager.SelfUpgraded("upgrade_shield"))
        {
            shieldActive = true;
            hud.SetShieldPercent(ShieldPercent);
        }
    }

    public override float HealthPercent()
    {
        return health / fullMaxHealth;
    }

    float EnergyPercent { get { return energy / fullMaxEnergy; } }

    void IncreaseShield(float amount)
    {
        if (shield == maxShield) return;
        shield = Mathf.Clamp(shield + amount, 0, maxShield);
        hud.SetShieldPercent(ShieldPercent);
    }

    bool DecreaseShield(float amount)
    {
        if (shield <= 0) return false;
        shield -= amount;
        hud.SetShieldPercent(ShieldPercent);
        return true;
    }

    float ShieldPercent { get { return shield / maxShield; } }

    public override void OnExplosion(float damage)
    {
        base.OnExplosion(damage * 0.2f);
    }
}
