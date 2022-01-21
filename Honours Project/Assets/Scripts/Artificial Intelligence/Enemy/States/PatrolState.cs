using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : State
{
    [SerializeField] Transform[] patrolPoints;
    [SerializeField] float closeness;
    int currentIndex = -1;
    bool active = false;

    public override bool EntryCondition()
    {
        return patrolPoints != null && patrolPoints.Length > 0;
    }

    public override void OnEnterState()
    {
        Debug.Log("Enter");
        IncreaseIndex();
    }

    public override void OnExecute()
    {
        if (!active) return;

        if(Useful.Close(controller.transform.position, patrolPoints[currentIndex].position, closeness))
        {
            IncreaseIndex();
        }

        Vector3 target = controller.GetCurrentNode();

        controller.Look(target);
        controller.Move();

        if (Useful.Close(controller.transform.position, target, 1)) controller.NextNode();
    }

    public void IncreaseIndex()
    {
        currentIndex++;
        if (currentIndex >= patrolPoints.Length) currentIndex = 0;

        FindPath();
    }

    public void FindPath()
    {
        active = controller.FindPath(patrolPoints[currentIndex].position);
        if (!active) StartCoroutine(DoubleCheck());
    }

    IEnumerator DoubleCheck()
    {
        yield return new WaitForSeconds(3);
        FindPath();
    }

    public override void OnExitState()
    {
        StopAllCoroutines();
    }
}
