using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : State
{
    [SerializeField] Transform[] patrolPoints;
    [SerializeField] float closeness;
    int currentIndex = 0;
    bool active = false;

    public override bool EntryCondition()
    {
        // Enter state if there are positions that can be patrolled to
        return patrolPoints != null && patrolPoints.Length > 0;
    }

    public override void OnEnterState()
    {
        Debug.Log("Enter Patrol");
        FindPath();
    }

    public override void OnExecute()
    {
        if (!active) return;

        // If close enough to the current target, move on to the next target
        if(Useful.Close(controller.transform.position, patrolPoints[currentIndex].position, closeness))
        {
            IncreaseIndex();
        }

        Vector3 target = controller.GetCurrentNode();                                               // Get the position of the current node

        controller.Look(target);                                                                    // Look at the current node
        controller.Move();                                                                          // Move forwards

        if (Useful.Close(controller.transform.position, target, 1)) controller.NextNode();          // If close enough to the node, move on to the next node
    }

    // Function to change the target to the next patrol point
    public void IncreaseIndex()
    {
        currentIndex++;
        if (currentIndex >= patrolPoints.Length) currentIndex = 0;

        FindPath();
    }

    // Function to find a path to the current patrol point
    public void FindPath()
    {
        active = controller.FindPath(patrolPoints[currentIndex].position, false);
        if (!active) StartCoroutine(DoubleCheck());
    }

    // Function to ensure that a path was found
    IEnumerator DoubleCheck()
    {
        // Wait for three seconds and then find a path again
        yield return new WaitForSeconds(3);
        FindPath();
    }

    public override void OnExitState()
    {
        StopAllCoroutines();
    }

    public override bool ExitCondition()
    {
        return true;
    }
}
