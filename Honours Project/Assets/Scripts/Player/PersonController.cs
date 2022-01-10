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
    protected GravitySource nearestSource;
    Vector3 lastVelocity;
    

    protected virtual void Awake(){
        rb = GetComponentInParent<Rigidbody>();
    }

    protected virtual void Start(){
        lastVelocity = rb.velocity;
    }

    protected virtual void LateUpdate(){
        CheckGrounded();
    }

    protected virtual void FixedUpdate(){
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

    public void SetNearestSource(GravitySource source)
    {
        nearestSource = source;
    }

    public GravitySource GetNearestSource()
    {
        return nearestSource;
    }
}
