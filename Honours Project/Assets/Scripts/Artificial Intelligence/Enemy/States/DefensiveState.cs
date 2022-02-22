using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefensiveState : State
{
    [SerializeField] float defenseRange;

    public override bool EntryCondition()
    {
        return controller.GetPlayerDistance() < defenseRange;
    }

    public override bool ExitCondition()
    {
        return controller.GetPlayerDistance() > defenseRange;
    }

    public override void OnEnterState()
    {
        controller.SetAnimBool("Defend", true);
        controller.GetDetails().SetImmune(true);
    }

    public override void OnExecute()
    {
    }

    public override void OnExitState()
    {
        controller.SetAnimBool("Defend", false);
        controller.GetDetails().SetImmune(false);
    }
}
