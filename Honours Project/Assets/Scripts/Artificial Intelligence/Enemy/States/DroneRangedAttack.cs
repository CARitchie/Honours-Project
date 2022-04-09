using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneRangedAttack : State
{
    [SerializeField] Gun gun;
    [SerializeField] float firingRange;
    [SerializeField] float closeRange;
    float findTime = 0;

    public override bool EntryCondition()
    {
        return controller.GetPlayerDistance() < firingRange;
    }

    public override void OnEnterState()
    {
        findTime = 0.5f;
        Debug.Log("Enter Ranged Attack");
        gun.OnEquip(controller);
        controller.SetHostile(true);
    }

    public override void OnExecute()
    {
        findTime -= Time.deltaTime;
        if (controller.PlayerVisible() || !PlayerController.IsPlayerActive())
        {
            AttackPlayer();
        }
        else
        {
            if (findTime <= 0)
            {
                findTime = 3;
                controller.FindPath(controller.PlayerPos(), true);
            }

            Vector3 target = controller.GetCurrentNode();

            controller.Look(target);
            controller.Move();

            if (Useful.Close(controller.transform.position, target, 1)) controller.NextNode();
        }


    }

    void AttackPlayer()
    {
        Vector3 playerPos = controller.PlayerPos();

        controller.Look(playerPos);

        gun.AimAt(playerPos);

        if (controller.GetPlayerDistance() >= closeRange)
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
