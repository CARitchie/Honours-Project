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
        // Enter state if the player is close enough
        return controller.GetPlayerDistance() < closeness;
    }

    public override bool ExitCondition()
    {
        // Exit the state when a random length of time has passed
        timer -= Time.deltaTime;
        return timer < 0;
    }

    public override void OnEnterState()
    {
        controller.SetHostile(true);
        timer = Random.Range(minRestTime, maxRestTime);         // Set the timer to a random value
        controller.SetAnimBool("Attack", true);                 // Activate the attack animation
        StartCoroutine(SlowDown());                             // Gradually reduce the enemy's movement speed
    }

    public override void OnExecute()
    {
        // If the player is too far away, stop the attack animation
        if (controller.GetPlayerDistance() >= closeness) controller.SetAnimBool("Attack", false);
    }

    public override void OnExitState()
    {
        StopAllCoroutines();
        controller.SetAnimBool("Attack", false);
    }

    // Gradually reduce the enemy's movement speed
    IEnumerator SlowDown()
    {
        while (true)
        {
            controller.AnimatorSlowDown();
            yield return new WaitForEndOfFrame();
        }
    }
}
