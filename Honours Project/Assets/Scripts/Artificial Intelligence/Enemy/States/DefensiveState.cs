using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefensiveState : State
{
    [SerializeField] float defenseRange;

    public override bool EntryCondition()
    {
        // Enter state if the player is close enough
        return controller.GetPlayerDistance() < defenseRange;
    }

    public override bool ExitCondition()
    {
        // Exit the state if the player is too far away
        return controller.GetPlayerDistance() > defenseRange;
    }

    public override void OnEnterState()
    {
        // Play the defensive animation and become immune to damage
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
