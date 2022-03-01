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
        controller.SetAnimBool("Attack", true);
        StartCoroutine(SlowDown());
    }

    public override void OnExecute()
    {
        if (controller.GetPlayerDistance() >= closeness) controller.SetAnimBool("Attack", false);
    }

    public override void OnExitState()
    {
        StopAllCoroutines();
        controller.SetAnimBool("Attack", false);
    }

    IEnumerator SlowDown()
    {
        while (true)
        {
            controller.AnimatorSlowDown();
            yield return new WaitForEndOfFrame();
        }
    }
}
