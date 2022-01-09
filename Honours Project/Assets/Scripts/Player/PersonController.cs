using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonController : MonoBehaviour
{
    [Header("Generic Settings")]
    [SerializeField] protected float walkSpeed;
    [SerializeField] protected float sprintSpeed;
    [SerializeField] protected float lookSensitivity;

    protected float movementSpeed;

    protected bool grounded = false;
    protected Rigidbody rb;
    Vector3 lastVelocity;

    protected virtual void Awake(){
        rb = GetComponentInParent<Rigidbody>();
    }

    protected virtual void Start(){
        lastVelocity = rb.velocity;
    }

    void LateUpdate(){
        CheckGrounded();
    }

    void FixedUpdate(){
        VelocityCheck();
    }

    protected virtual void Move(){

    }

    public void ForceVelocity(Vector3 velocity)
    {
        rb.velocity = velocity;
        lastVelocity = velocity;
    }

    public void SetPosition(Vector3 position)
    {
        transform.parent.position = position;
    }

    public void SetRotation(Vector3 rotation)
    {
        transform.parent.eulerAngles = rotation;
    }

    void VelocityCheck(){
        Vector3 currentVelocity = rb.velocity;
        float deltaV = (lastVelocity - currentVelocity).magnitude;
        lastVelocity = currentVelocity;

        if(deltaV > 15) Debug.LogError(transform.name + " Velocity death: " + deltaV + "m/s",transform);
    }

    protected virtual void CheckGrounded(){
        bool newGrounded = IsGrounded();
        if(grounded != newGrounded){
            grounded = newGrounded;
        }
    }

    public bool IsGrounded(){
        return Physics.BoxCast(transform.position, new Vector3(0.3f, 0.05f, 0.3f), -transform.up, transform.rotation, 1);
    }
}
