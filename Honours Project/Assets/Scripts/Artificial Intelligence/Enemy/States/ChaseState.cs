using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : State
{
    [SerializeField] float closeness;

    public override bool EntryCondition()
    {
        // Enter this state if hostile or the player is close enough
        return controller.IsHostile() || controller.GetPlayerDistance() < closeness;
    }

    public override void OnEnterState()
    {
        Debug.Log("Enter Chase");
        controller.SetHostile(true);
    }

    public override void OnExecute()
    {
        // Look at the player and then move forwards
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
