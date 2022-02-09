using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : PersonController
{
    [Header("Enemy Settings")]
    [SerializeField] State[] states;
    [SerializeField] float hostileRange;
    [SerializeField] Transform projectileHolder;
    [SerializeField] float hostileResetTime;
    CharacterGravity gravity;
    PathFinder pathFinder;

    float hostileTimer;

    State currentState;

    Stack<Vector3> nodes;
    Vector3 currentNode;

    Transform player;
    float playerDistance;

    bool hostile = false;
    EnemyDetails details;

    protected override void Awake()
    {
        base.Awake();

        pathFinder = GetComponentInParent<PathFinder>();
        gravity = GetComponentInParent<CharacterGravity>();
        details = GetComponentInParent<EnemyDetails>();
    }

    protected override void Start(){
        base.Start();

        GravityController.FindClosest(gravity);

        for(int i = 0; i < states.Length; i++)
        {
            states[i].InitialiseState(this);
        }

        player = PlayerController.Instance.transform;
    }

    protected override void FixedUpdate()
    {
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

        if(hostileTimer > 0)
        {
            hostileTimer -= Time.fixedDeltaTime;
            if (hostileTimer <= 0) hostile = false;
        }
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
        // THIS CAN BREAK
        return currentNode + nearestSource.transform.position;
    }

    public bool IsHostile()
    {
        return hostile;
    }

    public Vector3 PlayerPos()
    {
        return player.position;
    }

    public float GetPlayerDistance()
    {
        return playerDistance;
    }

    public bool PlayerVisible()
    {
        Vector3 point1 = transform.position + transform.up * 0.5f;
        Vector3 point2 = player.position + player.up * 0.5f;

        if (Physics.Raycast(point1, point2 - point1, out RaycastHit hit)){
            if (hit.transform.CompareTag("Player"))
            {
                Debug.DrawLine(point1, point2, Color.cyan);
                return true;
            }
        }

        return false;
    }

    public override Transform ProjectileSpawnPoint()
    {
        return projectileHolder;
    }

    public void AimAtPlayer()
    {
        projectileHolder.LookAt(player);
    }

    public void SetHostile(bool val)
    {
        hostile = val;

        if (hostile) hostileTimer = hostileResetTime;
    }

    public EnemyDetails GetDetails()
    {
        return details;
    }
}
