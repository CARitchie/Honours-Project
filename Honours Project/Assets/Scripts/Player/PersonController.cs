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
    protected Animator animator;

    protected virtual void Awake(){
        rb = GetComponentInParent<Rigidbody>();
        if (rb == null) rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }

    protected virtual void FixedUpdate(){
        CheckGrounded();
    }

    public virtual void Move(){

    }

    public void ForceVelocity(Vector3 velocity)
    {
        rb.velocity = velocity;
    }

    public void SetPosition(Vector3 position)
    {
        transform.parent.position = position;
    }

    public void SetRotation(Vector3 rotation)
    {
        transform.parent.eulerAngles = rotation;
    }

    protected virtual void CheckGrounded(){
        bool newGrounded = IsGrounded();
        if(grounded != newGrounded){
            grounded = newGrounded;
        }
    }

    public virtual bool IsGrounded(){
        return Physics.BoxCast(transform.position, new Vector3(0.3f, 0.05f, 0.3f), -transform.up, transform.rotation, 1);
    }

    public virtual void SetNearestSource(GravitySource source)
    {
        nearestSource = source;
    }

    public GravitySource GetNearestSource()
    {
        return nearestSource;
    }

    public Vector3 GetVelocity()
    {
        return rb.velocity;
    }

    public virtual void Recoil(float strength)
    {
        AddForce(strength * -transform.forward);
    }

    public virtual void AddForce(Vector3 force)
    {
        rb.AddForce(force, ForceMode.VelocityChange);
    }

    public void SetAnimBool(string key, bool val)
    {
        animator?.SetBool(key, val);
    }

    public void SetAnimTrigger(string key)
    {
        animator?.SetTrigger(key);
    }

    public void SetAnimFloat(string key, float val)
    {
        animator?.SetFloat(key, val);
    }

    public float GetAnimFloat(string key)
    {
        if (animator == null) return 0;
        return animator.GetFloat(key);
    }

    public virtual Vector3 GetAimDirection(Transform fireHole)
    {
        return fireHole.forward;
    }

    public float GetHeight()
    {
        if (nearestSource == null) return -1;

        return Vector3.Distance(nearestSource.transform.position, transform.position);
    }
}
