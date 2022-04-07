using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponEquip : MonoBehaviour, Interact
{
    [SerializeField] int weaponIndex;

    private void Start()
    {
        if (SaveManager.GetWeaponState(weaponIndex))
        {
            Destroy(gameObject);
        }
    }

    public void OnEnter()
    {
        HUD.SetInteractText("Equip");
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
