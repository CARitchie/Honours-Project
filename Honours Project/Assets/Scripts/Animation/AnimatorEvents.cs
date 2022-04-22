using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is added to animator components that will require the use of melee weapons
public class AnimatorEvents : MonoBehaviour
{
    [SerializeField] MeleeWeapon weapon;

    // Called by an animation when it has reached the point where
    // the melee weapon should be activated
    public void ActivateMelee()
    {
        weapon.Activate();
    }

    // Called by an animation when it has reached the point where
    // the melee weapon should be deactivated
    public void DeactivateMelee()
    {
        weapon.Deactivate();
    }
}
