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
    bool suspended = false;

    Vector3 offset;

    int layerMask = (1 << 0 | 1 << 8 | 1 << 9 | 1 << 10);

    protected override void Awake()
    {
        base.Awake();

        pathFinder = GetComponentInParent<PathFinder>();
        gravity = GetComponentInParent<CharacterGravity>();
        details = GetComponentInParent<EnemyDetails>();

        offset = details.transform.position - details.transform.parent.position;
    }

    protected virtual void Start(){
        if(gravity!=null) GravityController.FindClosest(gravity);       // Find the gravity source that is closest to the enemy

        for(int i = 0; i < states.Length; i++)
        {
            states[i].InitialiseState(this);                            // Initialise all of the states
        }

        player = PlayerController.Instance.transform;                   // Retrieve the player's transform
    }

    protected override void FixedUpdate()
    {
        if (!active) return;

        //base.FixedUpdate();

        playerDistance = Vector3.Distance(transform.position, player.position);     // Work out the distance to the player

        if (playerDistance > 100)                                                   // If the player is too far away
        {
            if (!suspended)
            {
                suspended = true;
                SetAnimFloat("MoveSpeed", 0);                                       // Stop playing movement animations
            }

            return;                                                                 // Don't bother to run or test any of the states
        }
        else if (suspended) suspended = false;

        if (currentState == null || currentState.ExitCondition())                   // If there is no current state, or the current state can be exited
        {
            for (int i = 0; i < states.Length; i++)                                 // Loop through all of the states
            {
                if (states[i].EntryCondition())                                     // If this state can be entered
                {
                    if (states[i] != currentState) ChangeState(states[i]);          // If this state is not the current state, change state
                    break;
                }
            }
        }

        currentState?.OnExecute();      // Run the current state

        CheckDespawnDistance();
    }

    // Function to change state
    void ChangeState(State state)
    {
        if (currentState != null) currentState.OnExitState();       // Exit the current state

        currentState = state;                                       // Set the current state to the new state

        if (currentState != null) currentState.OnEnterState();      // Enter the new current state
    }

    // Function to move forwards
    public override void Move()
    {
        movementSpeed = tempSpeed;

        // Work out the movement animation blend value based on the movement speed
        float moveSpeed;
        if (movementSpeed < walkSpeed) moveSpeed = movementSpeed / walkSpeed;
        else moveSpeed = ((movementSpeed - walkSpeed) / (sprintSpeed - walkSpeed)) + 1;
        //SetAnimFloat("MoveSpeed", moveSpeed);

        // Gradually change the animation blend value to the new value
        AnimMoveTo(moveSpeed);

        // Work out where the enemy should be moving to
        Vector3 target = rb.position + (transform.forward * movementSpeed * Time.deltaTime);

        // Move towards the target using the built in physics
        rb.MovePosition(target);
    }

    // Function to look at a target
    public virtual void Look(Vector3 point){

        Vector3 direction = point - transform.position;                                                                 // Calculate the direction towards the point
        Vector3 originalAngles = transform.localEulerAngles;                                                            // Store the original rotation

        direction = Vector3.RotateTowards(transform.forward, direction, lookSensitivity * Time.deltaTime, 0.0f);        // Rotate the current direction towards the target direction
        transform.rotation = Quaternion.LookRotation(direction);                                                        // Set the enemy's rotation to the new direction 
        
        Vector3 newAngles = transform.localEulerAngles;
        transform.localEulerAngles = new Vector3(originalAngles.x,newAngles.y,originalAngles.z);                        // Limit the rotation to only occur in the y axis
    }

    // Function to find a path towards a target
    public bool FindPath(Vector3 target, bool inViewGoodEnough)
    {
        nodes = pathFinder.FindPath(target, nearestSource?.transform, inViewGoodEnough);        // Set the stack of nodes equal to a newly found path
        if (nodes != null && nodes.Count > 0)                                                   // If nodes were found
        {
            currentNode = nodes.Pop();                                                          // Set the current node equal to the top in the stack
            return true;                                                                        // Return true if a path was found
        }
        return false;                                                                           // Return false if no path was found
    }

    // Function to move on to the next node
    public void NextNode()
    {
        if (nodes != null && nodes.Count > 0)       // If there are more nodes
        {
            currentNode = nodes.Pop();              // Set the current node to the top in the stack
        }
        else
        {
            currentNode = Vector3.zero;
        }
    }

    // Function to get the position of the current node
    public Vector3 GetCurrentNode()
    {
        // Return the current node added to the position of the planet
        return currentNode + nearestSource.transform.position;
    }

    // Function to determine whether a path exists
    public bool PathExists()
    {
        return nodes != null && nodes.Count > 1;
    }

    public bool IsHostile()
    {
        return hostile;
    }

    // Function to get the position of the player
    public Vector3 PlayerPos()
    {
        // Could add code to correct for when player is in the ship
        return player.position;
    }

    // Function to get the distance to the player
    public float GetPlayerDistance()
    {
        return playerDistance;
    }

    // Function to determine whether the player is visible
    public bool PlayerVisible()
    {
        Vector3 point1 = transform.position;
        Vector3 point2 = player.position;

        // Find all collisions between the enemy and the player
        RaycastHit[] hits = Physics.RaycastAll(point1, point2 - point1, Vector3.Distance(point1, point2), layerMask);

        if (hits == null || hits.Length < 1)                // If a collision was detected
        {
            Debug.DrawLine(point1, point2, Color.red);      // Draw a debug line in the editor

            return false;
        }

        if(hits.Length == 1 && hits[0].transform.CompareTag("Player"))      // If only the player was collided with
        {
            Debug.DrawLine(point1, point2, Color.cyan);
            return true;

        }

        for(int i = 0; i < hits.Length; i++)                                    // Loop through all collisions
        {
            if (hits[i].collider.gameObject.layer != 10) return false;          // If the collision wasn't another enemy
        }

        return true;
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

    // Function to match the enemy's velocity to that of its nearest source
    public void MatchSourceVelocity()
    {
        rb.velocity = nearestSource.GetVelocity();
    }

    // Function to gradually decrease the animation move speed
    public void AnimatorSlowDown()
    {
        float value = GetAnimFloat("MoveSpeed");
        value = Mathf.MoveTowards(value, 0, Time.deltaTime);
        SetAnimFloat("MoveSpeed", value);
    }

    // Function to gradually change the animation move speed
    public void AnimMoveTo(float target)
    {
        float current = GetAnimFloat("MoveSpeed");
        SetAnimFloat("MoveSpeed", Mathf.MoveTowards(current, target, Time.deltaTime * 3));
    }

    // Function to despawn the enemy if they are too far away from their origin
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

    // Function to find the player's height from the surface
    public float PlayerHeight()
    {
        if (player == null) return -1;
        if (Physics.Raycast(player.position, -player.up, out RaycastHit hit, 5, 1 << 8))
        {
            return hit.distance - 1;
        }
        return -1;
    }

    // Function to determine if the player is close enough to the ground
    public bool PlayerLowEnough()
    {
        float height = PlayerHeight();
        return height != -1 && height < 4;
    }
}
