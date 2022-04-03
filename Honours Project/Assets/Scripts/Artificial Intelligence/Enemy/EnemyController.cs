using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : PersonController
{
    [Header("Enemy Settings")]
    [SerializeField] State[] states;
    [SerializeField] float tempSpeed;
    CharacterGravity gravity;
    PathFinder pathFinder;

    State currentState;

    Stack<Vector3> nodes;
    Vector3 currentNode;

    Transform player;
    float playerDistance;

    bool hostile = false;
    protected EnemyDetails details;
    bool active = true;

    Vector3 offset;

    protected override void Awake()
    {
        base.Awake();

        pathFinder = GetComponentInParent<PathFinder>();
        gravity = GetComponentInParent<CharacterGravity>();
        details = GetComponentInParent<EnemyDetails>();

        offset = details.transform.position - details.transform.parent.position;
    }

    protected override void Start(){
        base.Start();

        if(gravity!=null) GravityController.FindClosest(gravity);

        for(int i = 0; i < states.Length; i++)
        {
            states[i].InitialiseState(this);
        }

        player = PlayerController.Instance.transform;
    }

    protected override void FixedUpdate()
    {
        if (!active) return;

        base.FixedUpdate();

        playerDistance = Vector3.Distance(transform.position, player.position);

        if (currentState == null || currentState.ExitCondition())
        {
            for (int i = 0; i < states.Length; i++)
            {
                if (states[i].EntryCondition())
                {
                    if (states[i] != currentState) ChangeState(states[i]);
                    break;
                }
            }
        }

        currentState?.OnExecute();

        CheckDespawnDistance();
    }

    void ChangeState(State state)
    {
        if (currentState != null) currentState.OnExitState();

        currentState = state;

        if (currentState != null) currentState.OnEnterState();
    }


    public override void Move()
    {
        movementSpeed = tempSpeed;

        float moveSpeed;
        if (movementSpeed < walkSpeed) moveSpeed = movementSpeed / walkSpeed;
        else moveSpeed = ((movementSpeed - walkSpeed) / (sprintSpeed - walkSpeed)) + 1;
        //SetAnimFloat("MoveSpeed", moveSpeed);
        AnimMoveTo(moveSpeed);

        Vector3 target = rb.position + (transform.forward * movementSpeed * Time.deltaTime);

        rb.MovePosition(target);
    }

    public virtual void Look(Vector3 point){

        Vector3 direction = point - transform.position;
        Vector3 originalAngles = transform.localEulerAngles;

        // This can probably be improved
        direction = Vector3.RotateTowards(transform.forward, direction, lookSensitivity * Time.deltaTime, 0.0f);
        transform.rotation = Quaternion.LookRotation(direction);
        
        Vector3 newAngles = transform.localEulerAngles;
        transform.localEulerAngles = new Vector3(originalAngles.x,newAngles.y,originalAngles.z);
    }

    public bool FindPath(Vector3 target, bool inViewGoodEnough)
    {
        nodes = pathFinder.FindPath(target, nearestSource?.transform, inViewGoodEnough);
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
        // THIS CAN BREAK
        return currentNode + nearestSource.transform.position;
    }

    public bool IsHostile()
    {
        return hostile;
    }

    public Vector3 PlayerPos()
    {
        // Could add code to correct for when player is in the ship
        return player.position;
    }

    public float GetPlayerDistance()
    {
        return playerDistance;
    }

    public bool PlayerVisible()
    {
        Vector3 point1 = transform.position;
        Vector3 point2 = player.position;

        if (Physics.Raycast(point1, point2 - point1, out RaycastHit hit)){
            if (hit.transform.CompareTag("Player"))
            {
                Debug.DrawLine(point1, point2, Color.cyan);
                return true;
            }
        }

        return false;
    }

    public void SetHostile(bool val)
    {
        hostile = val;
    }

    public EnemyDetails GetDetails()
    {
        return details;
    }

    public void SetActive(bool val)
    {
        active = val;
    }

    public void MatchSourceVelocity()
    {
        rb.velocity = nearestSource.GetVelocity();
    }

    public void AnimatorSlowDown()
    {
        float value = GetAnimFloat("MoveSpeed");
        value = Mathf.MoveTowards(value, 0, Time.deltaTime);
        SetAnimFloat("MoveSpeed", value);
    }

    public void AnimMoveTo(float target)
    {
        float current = GetAnimFloat("MoveSpeed");
        SetAnimFloat("MoveSpeed", Mathf.MoveTowards(current, target, Time.deltaTime * 3));
    }

    void CheckDespawnDistance()
    {
        if (!Useful.Close(Origin, details.transform.position, 500)) details.OnDeath();
    }

    public Vector3 Origin
    {
        get
        {
            return details.transform.parent.position + offset;
        }
    }

    public float SquareDistanceToOrigin
    {
        get
        {
            return (details.transform.position - Origin).sqrMagnitude;
        }
    }
}
