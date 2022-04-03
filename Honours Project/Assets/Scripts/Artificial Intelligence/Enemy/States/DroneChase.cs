using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneChase : State
{
    [SerializeField] float closeness;
    float findTime = 0;

    public override bool EntryCondition()
    {
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

        if (controller.PlayerVisible())
        {
            controller.Look(controller.PlayerPos());
            controller.Move();
        }
        else
        {
            if(findTime <= 0)
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

    public override void OnExitState()
    {
    }

    public override bool ExitCondition()
    {
        return true;
    }
}
