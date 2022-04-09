using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] WeaponContainer[] weapons;
    [SerializeField] float rotateSpeed;
    [SerializeField] float restoreSpeed;
    [SerializeField] float angle;

    float weaponZ = 0;
    int lastIndex = 0;

    public delegate void WeaponUnlock(int index);
    public static event WeaponUnlock OnWeaponUnlock;

    private void Start()
    {
        LoadWeapons();
        SaveManager.OnUpgradeChanged += LoadUpgrades;
        LoadUpgrades();
    }

    private void OnDestroy()
    {
        SaveManager.OnUpgradeChanged -= LoadUpgrades;
    }

    void LoadUpgrades()
    {
        if (SaveManager.SelfUpgraded("upgrade_damage"))
        {
            for(int i = 0; i < weapons.Length; i++)
            {
                weapons[i].GetWeapon().SetDamageMultiplier(1.8f);
            }
        }

        if (SaveManager.SelfUpgraded("upgrade_ammo"))
        {
            for (int i = 0; i < weapons.Length; i++)
            {
                Gun gun = weapons[i].GetWeapon().GetComponent<Gun>();
                if (gun != null) gun.SetMaxAmmoMultiplier(1.5f);
            }

            PlayerController.Instance.UpdateAmmoText();
        }
    }

    public bool IsLocked(int index)
    {
        if (index >= weapons.Length) return true;
        return weapons[index].IsLocked();
    }

    public Weapon GetWeapon(int index)
    {
        if (index >= weapons.Length) return null;
        lastIndex = index;
        return weapons[index].GetWeapon();
    }

    public void Rotate(float yChange)
    {
        yChange *= rotateSpeed;
        weaponZ -= yChange;
        weaponZ = Mathf.Clamp(weaponZ, -angle, angle);
        if (weaponZ != 0 && yChange == 0)
        {
            weaponZ = Mathf.MoveTowards(weaponZ, 0, Time.deltaTime * restoreSpeed);
            if (weaponZ < 0.2f && weaponZ > -0.2f) weaponZ = 0;
        }

        transform.localEulerAngles = Vector3.forward * weaponZ;
    }

    public int Scroll(float value)
    {
        bool increase = value < 0;
        int counter = 0;

        do
        {
            if (increase) IncreaseIndex();
            else DecreaseIndex();

            counter++;
        } while (counter < weapons.Length && (weapons[lastIndex].IsLocked() || weapons[lastIndex].GetWeapon() == null));

        return lastIndex;
    }

    void IncreaseIndex()
    {
        lastIndex++;
        if (lastIndex >= weapons.Length) lastIndex = 0;
    }

    void DecreaseIndex()
    {
        lastIndex--;
        if (lastIndex < 0) lastIndex = weapons.Length - 1;
    }

    public void UnlockWeapon(int index)
    {
        if(index < weapons.Length)
        {
            weapons[index].Unlock();
            OnWeaponUnlock?.Invoke(index);
        }
    }

    public List<bool> GetWeaponStates()
    {
        List<bool> states = new List<bool>();
        for(int i = 0; i < weapons.Length; i++)
        {
            states.Add(!weapons[i].IsLocked());
        }

        return states;
    }

    public void LoadWeapons()
    {
        bool unlocked = false;

        for (int i = 0; i < weapons.Length; i++)
        {
            SaveFile.WeaponData data = SaveManager.GetWeaponState(i);
            if (data == null) break;

            if (data.unlocked)
            {
                UnlockWeapon(i);
                unlocked = true;
                if(data.currentAmmo != -1000)
                {
                    weapons[i].GetWeapon().SetAmmo(data.currentAmmo);
                }
            }
        }

        if (unlocked) HUD.ActivateAmmoIndicator();
    }

    public bool AddAmmo(float percentOfMax)
    {
        bool added = false;
        foreach(WeaponContainer weapon in weapons)
        {
            if (weapon.GetWeapon().AddAmmo(percentOfMax)) added = true;
        }

        return added;
    }

    public void SaveWeapons()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            SaveManager.SetWeaponData(i, weapons[i]);
        }
    }
}

[System.Serializable]
public class WeaponContainer
{
    [SerializeField] Weapon weapon;
    [SerializeField] bool locked = true;

    public bool IsLocked()
    {
        return locked;
    }

    public Weapon GetWeapon()
    {
        return weapon;
    }

    public void Unlock()
    {
        locked = false;
    }
}
