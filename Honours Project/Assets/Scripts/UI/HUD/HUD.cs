using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField] HealthBar healthBar;
    [SerializeField] HealthBar energyBar;
    [SerializeField] WeaponWheel weaponWheel;

    private void Awake()
    {
        weaponWheel.gameObject.SetActive(true);
    }

    public void SetHealthPercent(float percent)
    {
        healthBar.SetPercent(percent);
    }

    public void SetEnergyPercent(float percent)
    {
        energyBar.SetPercent(percent);
    }

    public void SetWeaponWheelActive(float val)
    {
        weaponWheel.Activate(val);
    }
}
