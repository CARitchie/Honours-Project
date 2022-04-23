using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponEquip : MonoBehaviour, Interact
{
    [SerializeField] int weaponIndex;

    private void Start()
    {
        if (SaveManager.WeaponUnlocked(weaponIndex))        // Destroy the gameobject if the player has already unlocked it
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
        Useful.DestroyGameObject(gameObject);
    }
}
