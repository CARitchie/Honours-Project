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

        do
        {
            if (increase) IncreaseIndex();
            else DecreaseIndex();
        } while (weapons[lastIndex].IsLocked() || weapons[lastIndex].GetWeapon() == null);

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
}