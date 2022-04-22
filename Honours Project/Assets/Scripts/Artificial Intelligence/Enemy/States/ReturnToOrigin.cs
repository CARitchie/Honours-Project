using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToOrigin : State
{
    bool returned = false;

    public override bool EntryCondition()
    {
        // If the player is too far away, or the player is slightly out of range and the enemy is too far away from origin
        return controller.IsHostile() && ( controller.GetPlayerDistance() > 35 || (controller.GetPlayerDistance() > 15 && controller.SquareDistanceToOrigin > 2500) );
    }

    public override bool ExitCondition()
    {
        // If the player is close enough, and the enemy is close enough to its origin
        return controller.GetPlayerDistance() < 30 && controller.SquareDistanceToOrigin < 2000;
    }

    public override void OnEnterState()
    {
        controller.SetHostile(false);
        returned = false;
    }

    public override void OnExecute()
    {
        if(controller.SquareDistanceToOrigin > 8)           // If not close enough to origin
        {
            controller.Look(controller.Origin);             // Look at origin
            controller.Move();                              // Move forwards
        }else if (!returned)                                // If in range for the first time
        {
            returned = true;
            controller.SetAnimFloat("MoveSpeed", 0);        // Stop playing movement animation
        }
    }

    public override void OnExitState()
    {
        
    }
}
