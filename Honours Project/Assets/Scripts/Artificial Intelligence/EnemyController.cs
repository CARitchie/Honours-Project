using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] float walkSpeed;
    [SerializeField] float rotationSpeed;
    [SerializeField] Transform target;
    [SerializeField] float timeDelay = 5;

    Rigidbody rb;
    PathFinder pathFinder;
    CharacterGravity gravity;

    Stack<Vector3> nodes;

    Vector3 currentNode;

    private void Awake()
    {
        rb = GetComponentInParent<Rigidbody>();
        pathFinder = GetComponentInParent<PathFinder>();
        gravity = GetComponentInParent<CharacterGravity>();
    }

    void Start(){
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

        Look(currentNode);
        Move();

        if((currentNode - transform.position).sqrMagnitude < 1){
            if(nodes.Count > 0){
                currentNode = nodes.Pop();
            }else{
                currentNode = Vector3.zero;
            }
        }
    }

    void Move(){
        
        Vector3 target = rb.position + (transform.forward * walkSpeed * Time.deltaTime);

        rb.MovePosition(target);
    }

    void Look(Vector3 point){
        

        Vector3 direction = point - transform.position;
        Vector3 originalAngles = transform.localEulerAngles;

        direction = Vector3.RotateTowards(transform.forward, direction, rotationSpeed * Time.deltaTime, 0.0f);
        transform.rotation = Quaternion.LookRotation(direction);
        
        Vector3 newAngles = transform.localEulerAngles;
        transform.localEulerAngles = new Vector3(originalAngles.x,newAngles.y,originalAngles.z);
    }
}
