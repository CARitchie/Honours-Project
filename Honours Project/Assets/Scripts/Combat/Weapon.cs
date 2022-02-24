using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
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
}
