using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : PersonController
{
    [Header("Enemy Settings")]
    [SerializeField] Transform target;
    [SerializeField] float timeDelay = 5;
    [SerializeField] Transform planet;

    PathFinder pathFinder;
    CharacterGravity gravity;

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

        CalculatePath();
        StartCoroutine(Finder());
    }

    IEnumerator Finder(){
        float timer = timeDelay;
        while(true){

            while(timer > 0){
                timer -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            CalculatePath();
            timer = timeDelay;
        }
    }

    void CalculatePath(){
        nodes = pathFinder.FindPath(target.position);
        if(nodes != null && nodes.Count > 0) currentNode = nodes.Pop();
    }

    void Update(){
        if(currentNode == Vector3.zero) return;

        Look(currentNode + planet.position);
        Move();

        if((currentNode + planet.position - transform.position).sqrMagnitude < 1){
            if(nodes != null && nodes.Count > 0){
                currentNode = nodes.Pop();
            }else{
                currentNode = Vector3.zero;
            }
        }
    }

    protected override void Move()
    {
        movementSpeed = walkSpeed;

        Vector3 target = rb.position + (transform.forward * movementSpeed * Time.deltaTime);

        rb.MovePosition(target);
    }

    void Look(Vector3 point){
        

        Vector3 direction = point - transform.position;
        Vector3 originalAngles = transform.localEulerAngles;

        direction = Vector3.RotateTowards(transform.forward, direction, lookSensitivity * Time.deltaTime, 0.0f);
        transform.rotation = Quaternion.LookRotation(direction);
        
        Vector3 newAngles = transform.localEulerAngles;
        transform.localEulerAngles = new Vector3(originalAngles.x,newAngles.y,originalAngles.z);
    }
}
