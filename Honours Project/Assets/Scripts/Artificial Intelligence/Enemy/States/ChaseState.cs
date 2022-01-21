using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : State
{
    [SerializeField] float closeness;

    public override bool EntryCondition()
    {
        return controller.IsHostile() || Useful.Close(controller.transform.position, controller.PlayerPos(), closeness);
    }

    public override void OnEnterState()
    {
        Debug.Log("Enter Chase");
    }

    public override void OnExecute()
    {
        controller.Look(controller.PlayerPos());
        controller.Move();
    }

    public override void OnExitState()
    {
    }
}
