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
        return Useful.Close(controller.transform.position, controller.PlayerPos(), firingRange);
    }

    public override void OnEnterState()
    {
        Debug.Log("Enter Ranged Attack");
        gun.OnEquip(controller);
    }

    public override void OnExecute()
    {
        controller.Look(controller.PlayerPos());

        controller.AimAtPlayer();

        if(!Useful.Close(controller.transform.position, controller.PlayerPos(), closeRange))
        {
            controller.Move();
        }

        gun.PrimaryAction(1);
    }

    public override void OnExitState()
    {
        gun.OnUnEquip();
    }
}
