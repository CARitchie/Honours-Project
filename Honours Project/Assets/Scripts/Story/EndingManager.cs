using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class EndingManager : MonoBehaviour
{
    [SerializeField] PlayableDirector director;
    [SerializeField] float avoidMineTime;
    [SerializeField] float clipMineTime;
    [SerializeField] float hitMineTime;
    [SerializeField] float mineExplosionTime;
    [SerializeField] float suckTime;
    [SerializeField] float enemyArriveTime;
    float health = 100;
    int teleportType = 0;

    private void Start()
    {
        ForceStates();
        CalculateSurvival();
    }

    void ForceStates()
    {
        SaveManager.SetUpgradeState("upgrade_teleport", SaveFile.UpgradeState.Sacrificed);
        SaveManager.SetUpgradeState("upgrade_solar", SaveFile.UpgradeState.Sacrificed);
        SaveManager.SetUpgradeState("upgrade_ammo", SaveFile.UpgradeState.Sacrificed);
        SaveManager.SetUpgradeState("upgrade_damage", SaveFile.UpgradeState.Sacrificed);
        SaveManager.SetUpgradeState("upgrade_gun", SaveFile.UpgradeState.Sacrificed);
        SaveManager.SetUpgradeState("upgrade_shield", SaveFile.UpgradeState.Sacrificed);
        SaveManager.SetUpgradeState("upgrade_nanites", SaveFile.UpgradeState.Sacrificed);
        SaveManager.SetUpgradeState("upgrade_thruster", SaveFile.UpgradeState.Sacrificed);
        SaveManager.SetUpgradeState("sacrifice_lava", SaveFile.UpgradeState.PlayerUnlocked);
        SaveManager.SetUpgradeState("sacrifice_energy", SaveFile.UpgradeState.PlayerUnlocked);
        SaveManager.SetUpgradeState("sacrifice_speed", SaveFile.UpgradeState.PlayerUnlocked);
        SaveManager.SetUpgradeState("sacrifice_indicators", SaveFile.UpgradeState.PlayerUnlocked);
        SaveManager.SetUpgradeState("sacrifice_pulse", SaveFile.UpgradeState.Sacrificed);
        SaveManager.SetUpgradeState("sacrifice_jump", SaveFile.UpgradeState.Sacrificed);
        SaveManager.SetUpgradeState("sacrifice_compass", SaveFile.UpgradeState.PlayerUnlocked);
        SaveManager.SetUpgradeState("sacrifice_health", SaveFile.UpgradeState.PlayerUnlocked);
    }

    public void CalculateSurvival()
    {
        Teleport();
        Offense();
        Avoidance();
        Debug.Log("COLONY SHIP HEALTH: " + health);
    }

    void Teleport()
    {
        if (SaveManager.SacrificeMade("upgrade_teleport"))
        {
            int count = 0;
            if (SaveManager.SacrificeMade("sacrifice_lava")) count++;
            if (SaveManager.SacrificeMade("sacrifice_energy")) count++;
            if (SaveManager.SacrificeMade("upgrade_solar")) count++;

            switch (count)
            {
                case 3:
                    // Escape if at least one other sacrifice made
                    teleportType = 2;
                    break;
                case 2:
                    health -= 10;
                    teleportType = 1;
                    break;
                case 1:
                    health -= 20;
                    break;
            }
        }
        else
        {
            health -= 30;
        }
    }

    void Offense()
    {
        float multi = 1;
        float reduction = 0;
        if (!SaveManager.SacrificeMade("sacrifice_speed"))
        {
            reduction += 8;
            multi = 1.5f;
        }

        if (SaveManager.SacrificeMade("sacrifice_indicators")) multi *= 0.75f;

        if (!SaveManager.SacrificeMade("upgrade_ammo")) reduction += 15 * multi;
        if (!SaveManager.SacrificeMade("upgrade_damage")) reduction += 15 * multi;
        if (!SaveManager.SacrificeMade("upgrade_gun")) reduction += 15 * multi;
        else health += 30;

        if (!SaveManager.SacrificeMade("upgrade_shield")) health *= 0.9f;
        if (!SaveManager.SacrificeMade("upgrade_nanites")) health *= 0.9f;
        if (!SaveManager.SacrificeMade("upgrade_thruster")) health *= 0.9f;

        health -= reduction;
    }

    void Avoidance()
    {
        if (SaveManager.SacrificeMade("sacrifice_pulse"))
        {
            if (!SaveManager.SacrificeMade("sacrifice_jump"))
            {
                health -= 15;
            }
        }
        else
        {
            health -= 30;
        }

        if (!SaveManager.SacrificeMade("sacrifice_compass")) health -= 8;

        if (SaveManager.SacrificeMade("sacrifice_health")) health += 20;
    }

    public void StartMineCollision()
    {
        if (SaveManager.SacrificeMade("sacrifice_pulse"))
        {
            if (SaveManager.SacrificeMade("sacrifice_jump")) SetTime(avoidMineTime);
            else SetTime(clipMineTime);
        }
        else
        {
            SetTime(hitMineTime);
        }
    }

    public void MineCollision()
    {
        Debug.Log("COLLISION");
        SetTime(mineExplosionTime);
    }

    void SetTime(float time)
    {
        director.Stop();
        director.time = time;
        director.Play();
    }

    public void SuckOutPods()
    {
        if (!SaveManager.SacrificeMade("sacrifice_health"))
        {
            SetTime(suckTime);
        }
        else
        {
            EarlyWarning();
        }
    }

    public void EarlyWarning()
    {
        if (SaveManager.SacrificeMade("sacrifice_indicators"))
        {

        }
        else
        {
            SetTime(enemyArriveTime);
        }
    }

    public void FirstEscapeAttempt()
    {
        if(teleportType == 2 && health > 40)
        {
            //Escape
        }
    }

    public void SecondEscapeAttempt()
    {
        if((teleportType == 2 && health > 10) ||(teleportType == 2 && health > 50))
        {
            //Escape
        }
        else
        {
            EarlyWarning();
        }
    }

}
