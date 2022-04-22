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
        // Enter state if the player is within range
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

        // If the player can be seen or is piloting their ship
        if (controller.PlayerVisible() || !PlayerController.IsPlayerActive())
        {
            AttackPlayer();
        }
        else
        {
            if (findTime <= 0 && controller.PlayerLowEnough())                  // If enough time between finding paths has passed and the player is close enough to the ground
            {
                findTime = 3;                                                   // Reset the timer
                controller.FindPath(controller.PlayerPos(), true);              // Find a path towards the player
            }

            Vector3 target = controller.GetCurrentNode();                       // Get the position of the current node

            // Look at the node and move forwards
            controller.Look(target);
            controller.Move();

            // If close enough to the node, move on to the next node
            if (Useful.Close(controller.transform.position, target, 1)) controller.NextNode();
        }


    }

    void AttackPlayer()
    {
        Vector3 playerPos = controller.PlayerPos();

        controller.Look(playerPos);                             // Look at the player

        gun.AimAt(playerPos);                                   // Aim the ranged weapon at the player

        if (controller.GetPlayerDistance() >= closeRange)       // If not too close to the player
        {
            controller.Move();                                  // Move forward
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
