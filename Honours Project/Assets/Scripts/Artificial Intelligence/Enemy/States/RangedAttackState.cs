using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttackState : State
{
    [SerializeField] Gun gun;
    [SerializeField] float firingRange;
    [SerializeField] float closeRange;

    public override bool EntryCondition()
    {
        // Enter the state if the player is within range
        return controller.GetPlayerDistance() < firingRange;
    }

    public override void OnEnterState()
    {
        Debug.Log("Enter Ranged Attack");
        gun.OnEquip(controller);
        controller.SetHostile(true);
    }

    public override void OnExecute()
    {
        Vector3 playerPos = controller.PlayerPos();

        controller.Look(playerPos);                             // Look at the player

        gun.AimAt(playerPos);                                   // Aim the ranged weapon at the player

        if(controller.GetPlayerDistance() >= closeRange)        // If the player is too far away
        {
            controller.Move();                                  // Move forwards
        }

        gun.PrimaryAction(1);                                   // Fire the ranged weapon
    }

    public override void OnExitState()
    {
        gun.OnUnEquip();
    }

    public override bool ExitCondition()
    {
        return true;
    }
}
