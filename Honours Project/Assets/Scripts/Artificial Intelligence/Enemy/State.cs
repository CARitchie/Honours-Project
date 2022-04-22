using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State : MonoBehaviour
{
    protected EnemyController controller;

    public void InitialiseState(EnemyController controller)
    {
        // Set the controller to the enemy that this state is active for
        this.controller = controller;
    }

    public abstract void OnEnterState();
    public abstract void OnExitState();
    public abstract void OnExecute();
    public abstract bool EntryCondition();
    public abstract bool ExitCondition();
}
