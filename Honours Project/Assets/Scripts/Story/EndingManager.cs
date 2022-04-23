using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class EndingManager : MonoBehaviour
{
    [SerializeField] PlayableDirector director;
    [SerializeField] PlayableDirector enemyShip;
    [SerializeField] PlayableDirector turret;
    [SerializeField] ColonyTeleport teleporter;
    [SerializeField] float avoidMineTime;
    [SerializeField] float clipMineTime;
    [SerializeField] float hitMineTime;
    [SerializeField] float mineExplosionTime;
    [SerializeField] float suckTime;
    [SerializeField] float enemyArriveTime;
    [SerializeField] float bigGunTime;
    [SerializeField] ParticleSystem turretParticles;
    [SerializeField] float turretTime;
    [SerializeField] float enemyDestroyedTime;
    [SerializeField] float colonyShipDestroyedTime;
    [SerializeField] float enemyAttackTime;
    [SerializeField] float teleportTime;
    public static float health = 100;
    int teleportType = 0;
    bool turretsPlayed = false;

    private void Start()
    {
        //ForceStates();
        health = 100;
        CalculateSurvival();
    }

    // Function that was used during testing to determine the outcome of having certain sacrifices/upgrades unlocked
    void ForceStates()
    {
        SaveManager.LoadGame();
        SaveManager.SetUpgradeState("upgrade_teleport", SaveFile.UpgradeState.PlayerUnlocked);
        SaveManager.SetUpgradeState("upgrade_solar", SaveFile.UpgradeState.Sacrificed);
        SaveManager.SetUpgradeState("upgrade_ammo", SaveFile.UpgradeState.PlayerUnlocked);
        SaveManager.SetUpgradeState("upgrade_damage", SaveFile.UpgradeState.Sacrificed);
        SaveManager.SetUpgradeState("upgrade_gun", SaveFile.UpgradeState.Sacrificed);
        SaveManager.SetUpgradeState("upgrade_shield", SaveFile.UpgradeState.PlayerUnlocked);
        SaveManager.SetUpgradeState("upgrade_nanites", SaveFile.UpgradeState.PlayerUnlocked);
        SaveManager.SetUpgradeState("upgrade_thruster", SaveFile.UpgradeState.Sacrificed);
        SaveManager.SetUpgradeState("sacrifice_lava", SaveFile.UpgradeState.Sacrificed);
        SaveManager.SetUpgradeState("sacrifice_energy", SaveFile.UpgradeState.Sacrificed);
        SaveManager.SetUpgradeState("sacrifice_speed", SaveFile.UpgradeState.PlayerUnlocked);
        SaveManager.SetUpgradeState("sacrifice_indicators", SaveFile.UpgradeState.Sacrificed);
        SaveManager.SetUpgradeState("sacrifice_pulse", SaveFile.UpgradeState.Sacrificed);
        SaveManager.SetUpgradeState("sacrifice_jump", SaveFile.UpgradeState.Sacrificed);
        SaveManager.SetUpgradeState("sacrifice_compass", SaveFile.UpgradeState.Sacrificed);
        SaveManager.SetUpgradeState("sacrifice_health", SaveFile.UpgradeState.PlayerUnlocked);

        //SaveManager.SaveToFile();
    }

    // Function to determine the chance of survival
    public void CalculateSurvival()
    {
        Teleport();
        Offense();
        Avoidance();
        Debug.Log("COLONY SHIP HEALTH: " + health);
    }

    // Calculate the survival chance as a result of the teleport upgrades
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
                    // Escape if at least one other sacrifice made (not necessarily true anymore)
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

    // Calculate the survival chance as a result of the offensive upgrades
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

    // Calculate the survival chance as a result of the defensive upgrades
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

    // Determine whether the colony ship collides with the explosive mine
    public void StartMineCollision()
    {
        if (SaveManager.SacrificeMade("sacrifice_pulse"))
        {
            if (SaveManager.SacrificeMade("sacrifice_jump"))
            {
                SetTime(avoidMineTime);
                DialogueManager.PlayDialogue("audio_mine");
            }
            else SetTime(clipMineTime);
        }
        else
        {
            SetTime(hitMineTime);
        }
    }

    public void MineCollision()
    {
        SetTime(mineExplosionTime);
    }

    // Function to go to a specific time in the cutscene
    void SetTime(float time)
    {
        director.Stop();
        director.time = time;
        director.Play();
    }

    // Function to go to a specific time in the specified cutscene
    public void SetTime(float time, PlayableDirector playableDirector)
    {
        playableDirector.Stop();
        playableDirector.time = time;
        playableDirector.Play();
    }

    // Determine whether more colony pods are sucked out
    public void SuckOutPods()
    {
        if (!SaveManager.SacrificeMade("sacrifice_health"))
        {
            SetTime(suckTime);
        }
        else
        {
            SecondEscapeAttempt();
        }
    }

    // Determine whether the early warning system is active
    public void EarlyWarning()
    {
        SetTime(enemyArriveTime);
        SetTime(0, enemyShip);

        if (SaveManager.SacrificeMade("sacrifice_indicators"))
        {
            PlayTurrets();
            DialogueManager.PlayDialogue("audio_hyperdrive");
        }
    }

    // Function to move the turrets
    public void PlayTurrets()
    {
        if (turretsPlayed) return;
        turretsPlayed = true;

        // Used to play dialogue when the enemy ship arrived without warning
        // Removed as dialogue clip wasn't very good
        if (!SaveManager.SacrificeMade("sacrifice_indicators"))
        {
            //DialogueManager.PlayDialogue("audio_unexpected");
        }

        // If the turrets have been unlocked
        if (SaveManager.SacrificeMade("sacrifice_speed"))
        {
            // Make the turrets aim at the enemy ship
            SetTime(0, turret);
        }
    }

    // First attempt at teleporting away
    public void FirstEscapeAttempt()
    {
        if(teleportType == 2 && health > 40)
        {
            TeleportAway();
        }
    }

    // Second attempt at teleporting away
    public void SecondEscapeAttempt()
    {
        if((teleportType == 2 && health > 10) ||(teleportType == 1 && health > 50))
        {
            TeleportAway();
        }
        else
        {
            EarlyWarning();
        }
    }

    // Third and final attempt at teleporting away
    public void ThirdEscapeAttempt()
    {
        if(teleportType == 1 && health > 40)
        {
            TeleportAway();
        }
        else
        {
            BigGun();
        }
    }

    // Function to aim and fire the big gun
    void BigGun()
    {
        if (SaveManager.SacrificeMade("upgrade_gun"))
        {
            SetTime(bigGunTime);
            FireTurrets();
        }
        else
        {
            AttackEnemy();
        }
    }

    // Activate the turret particle system
    bool FireTurrets()
    {
        if (SaveManager.SacrificeMade("sacrifice_speed"))
        {
            turretParticles.Play();
            return true;
        }
        return false;
    }

    // Determine whether the colony ship can attack the enemy
    void AttackEnemy()
    {
        if (FireTurrets())
        {
            SetTime(turretTime);
        }
        else
        {
            AfterAttack();
        }
    }

    // Once the colony ship has attacked the enemy, determine whether the enemy survives or explodes
    public void AfterAttack()
    {
        if(health > 70)
        {
            if(SaveManager.SacrificeMade("upgrade_gun") || SaveManager.SacrificeMade("sacrifice_speed"))
            {
                SetTime(enemyDestroyedTime);
            }
            else
            {
                SetTime(62.3f);
            }
            
        }
        else
        {
            SetTime(enemyAttackTime);
        }
    }

    public void TestSurvived()
    {
        if(health < 10)
        {
            Debug.Log("Decimated");
            SetTime(colonyShipDestroyedTime);
        }
        else
        {
            Debug.Log("Survived");
            // switch to fighting
            director.Stop();
            SceneManager.FadeToScene("FinalFight");
        }
    }

    public void GameEnd()
    {
        director.Stop();
        SceneManager.FadeToScene("MainMenu");
    }

    void TeleportAway()
    {
        SetTime(teleportTime);
        teleporter.Teleport();
    }


}
