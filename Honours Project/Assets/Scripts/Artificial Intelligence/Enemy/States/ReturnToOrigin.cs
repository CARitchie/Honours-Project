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
        return controller.GetPlayerDistance() < 30 && controller.SquareDistanceToOrigin < 2000;
    }

    public override void OnEnterState()
    {
        controller.SetHostile(false);
        returned = false;
    }

    public override void OnExecute()
    {
        if(controller.SquareDistanceToOrigin > 8)
        {
            controller.Look(controller.Origin);
            controller.Move();
        }else if (!returned)
        {
            returned = true;
            controller.SetAnimFloat("MoveSpeed", 0);
        }
    }

    public override void OnExitState()
    {
        
    }
}
