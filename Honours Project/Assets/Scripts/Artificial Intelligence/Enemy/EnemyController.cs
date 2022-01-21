using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : PersonController
{
    [Header("Enemy Settings")]
    CharacterGravity gravity;
    PathFinder pathFinder;

    [SerializeField] State[] states;
    State currentState;

    Stack<Vector3> nodes;
    Vector3 currentNode;

    protected override void Awake()
    {
        base.Awake();

        pathFinder = GetComponentInParent<PathFinder>();
        gravity = GetComponentInParent<CharacterGravity>();
    }

    protected override void Start(){
        base.Start();

        GravityController.FindClosest(gravity);

        for(int i = 0; i < states.Length; i++)
        {
            states[i].InitialiseState(this);
        }
    }

    private void Update()
    {
        for (int i = 0; i < states.Length; i++)
        {
            if (states[i] != currentState && states[i].EntryCondition())
            {
                ChangeState(states[i]);
                break;
            }
        }

        currentState.OnExecute();
    }

    void ChangeState(State state)
    {
        if (currentState != null) currentState.OnExitState();

        currentState = state;

        if (currentState != null) currentState.OnEnterState();
    }


    public override void Move()
    {
        movementSpeed = walkSpeed;

        Vector3 target = rb.position + (transform.forward * movementSpeed * Time.deltaTime);

        rb.MovePosition(target);
    }

    public void Look(Vector3 point){

        Vector3 direction = point - transform.position;
        Vector3 originalAngles = transform.localEulerAngles;

        direction = Vector3.RotateTowards(transform.forward, direction, lookSensitivity * Time.deltaTime, 0.0f);
        transform.rotation = Quaternion.LookRotation(direction);
        
        Vector3 newAngles = transform.localEulerAngles;
        transform.localEulerAngles = new Vector3(originalAngles.x,newAngles.y,originalAngles.z);
    }

    public bool FindPath(Vector3 target)
    {
        nodes = pathFinder.FindPath(target, nearestSource?.transform);
        if (nodes != null && nodes.Count > 0)
        {
            currentNode = nodes.Pop();
            return true;
        }
        return false;
    }

    public void NextNode()
    {
        if (nodes != null && nodes.Count > 0)
        {
            currentNode = nodes.Pop();
        }
        else
        {
            currentNode = Vector3.zero;
        }
    }

    public Vector3 GetCurrentNode()
    {
        return currentNode + nearestSource.transform.position;
    }
}
