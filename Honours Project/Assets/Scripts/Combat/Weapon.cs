using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Weapon parent class
// Functions were mostly left blank as it was intended to be able to create proper melee weapons
public class Weapon : MonoBehaviour
{
    [SerializeField] protected float damageMultiplier = 1;
    protected PersonController controller;

    public virtual void OnEquip(PersonController controller)
    {
        this.controller = controller;
        Debug.Log("Equip");
        gameObject.SetActive(true);
    }

    public virtual void OnUnEquip()
    {
        controller = null;
        gameObject.SetActive(false);
    }

    public virtual void PrimaryAction(float val)
    {

    }

    public virtual void SecondaryAction(float val)
    {

    }

    public void SetDamageMultiplier(float multiplier)
    {
        damageMultiplier = multiplier;
    }

    public virtual string GetAmmoText()
    {
        return "null";
    }

    public virtual bool IsInfinite()
    {
        return false;
    }

    public virtual bool AddAmmo(float percentOfMax)
    {
        return false;
    }

    public virtual int GetAmmo()
    {
        return -1000;
    }

    public virtual void SetAmmo(int ammo)
    {

    }
}
