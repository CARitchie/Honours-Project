using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorEvents : MonoBehaviour
{
    [SerializeField] MeleeWeapon weapon;

    public void ActivateMelee()
    {
        weapon.Activate();
    }

    public void DeactivateMelee()
    {
        weapon.Deactivate();
    }
}
