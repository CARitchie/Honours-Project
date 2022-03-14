using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponEquip : MonoBehaviour, Interact
{
    [SerializeField] int weaponIndex;

    public void OnEnter()
    {
        HUD.SetInteractText("Pickup");
    }

    public void OnExit()
    {
        HUD.ClearInteractText();
    }

    public void OnSelect()
    {
        PlayerController.Instance.UnlockWeapon(weaponIndex);
        PlayerController.Instance.EquipWeapon(weaponIndex);
        Destroy(gameObject);
    }
}
