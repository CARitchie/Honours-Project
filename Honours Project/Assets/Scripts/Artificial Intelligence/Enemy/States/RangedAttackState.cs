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
        return controller.GetPlayerDistance() < firingRange;
    }

    public override void OnEnterState()
    {
        Debug.Log("Enter Ranged Attack");
        gun.OnEquip(controller);
    }

    public override void OnExecute()
    {
        Vector3 playerPos = controller.PlayerPos();

        controller.Look(playerPos);

        gun.AimAt(playerPos);

        if(controller.GetPlayerDistance() >= closeRange)
        {
            controller.Move();
        }

        gun.PrimaryAction(1);
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
