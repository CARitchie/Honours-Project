using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : State
{
    [SerializeField] float closeness;
    [SerializeField] float minRestTime = 0.3f;
    [SerializeField] float maxRestTime = 2;
    float timer;

    public override bool EntryCondition()
    {
        return controller.GetPlayerDistance() < closeness;
    }

    public override bool ExitCondition()
    {
        timer -= Time.deltaTime;
        return timer < 0;
    }

    public override void OnEnterState()
    {
        timer = Random.Range(minRestTime, maxRestTime);
        controller.SetAnimFloat("MoveSpeed", 0);
        controller.SetAnimBool("Attack", true);
    }

    public override void OnExecute()
    {
        if (controller.GetPlayerDistance() >= closeness) controller.SetAnimBool("Attack", false);
    }

    public override void OnExitState()
    {
        controller.SetAnimBool("Attack", false);
    }
}
