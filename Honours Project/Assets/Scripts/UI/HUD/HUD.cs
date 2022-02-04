using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField] HealthBar healthBar;
    [SerializeField] HealthBar energyBar;


    public void SetHealthPercent(float percent)
    {
        healthBar.SetPercent(percent);
    }

    public void SetEnergyPercent(float percent)
    {
        energyBar.SetPercent(percent);
    }
}
