using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalGravitySource : MonoBehaviour
{
    [SerializeField] float strength;

    List<GravityReceiver> receivers = new List<GravityReceiver>();

    private void OnTriggerEnter(Collider other)
    {
        GravityReceiver receiver = other.GetComponentInParent<GravityReceiver>();
        if(receiver != null){
            if(!receivers.Contains(receiver)){
                receivers.Add(receiver);
                receiver.AddLocalGravitySource(this);
            } 
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GravityReceiver receiver = other.GetComponentInParent<GravityReceiver>();
        if(receiver != null){
            if(receivers.Contains(receiver)){
                receivers.Remove(receiver);
                receiver.RemoveLocalGravitySource(this);
            } 
        }
    }

    public Vector3 GetForce(){
        return strength * GetDirection();
    }

    public Vector3 GetDirection(){
        return -transform.up;
    }
}