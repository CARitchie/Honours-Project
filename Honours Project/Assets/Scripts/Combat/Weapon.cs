using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    protected PlayerController player;

    public virtual void OnEquip(PlayerController controller)
    {
        player = controller;
        Debug.Log("Equip");
    }

    public virtual void OnUnEquip()
    {
        player = null;
    }

    public virtual void PrimaryAction(float val)
    {

    }

    public virtual void SecondaryAction(float val)
    {

    }
}
