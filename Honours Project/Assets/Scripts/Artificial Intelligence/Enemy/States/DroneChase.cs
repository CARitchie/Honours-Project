using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneChase : State
{
    [SerializeField] float closeness;
    float findTime = 0;

    public override bool EntryCondition()
    {
        // Enter this state if hostile or the player is close enough
        return controller.IsHostile() || controller.GetPlayerDistance() < closeness;
    }

    public override void OnEnterState()
    {
        findTime = 0.5f;
        Debug.Log("Enter Chase");
        controller.SetHostile(true);
    }

    public override void OnExecute()
    {
        findTime -= Time.deltaTime;

        // If the player can be seen or is piloting their ship
        if (controller.PlayerVisible() || !PlayerController.IsPlayerActive())
        {
            MoveToPlayer();
        }
        else
        {
            if(findTime <= 0 && controller.PlayerLowEnough())           // If enough time between finding paths has passed and the player is close enough to the ground
            {
                findTime = 3;                                           // Reset the timer
                controller.FindPath(controller.PlayerPos(), true);      // Find a new path towards the player
            }

            Vector3 target = controller.GetCurrentNode();               // Retrieve the current node in the path

            // Look at the node and move forwards
            controller.Look(target);
            controller.Move();

            // If close enough to the node, move on to the next node
            if (Useful.Close(controller.transform.position, target, 1)) controller.NextNode();
        }

    }

    void MoveToPlayer()
    {
        // Look at the player and move forwards
        controller.Look(controller.PlayerPos());
        controller.Move();
    }

    public override void OnExitState()
    {
    }

    public override bool ExitCondition()
    {
        return true;
    }
}
