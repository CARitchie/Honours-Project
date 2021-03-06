using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalGravitySource : GravitySource
{
    [SerializeField] float strength;

    List<GravityReceiver> receivers = new List<GravityReceiver>();

    private void OnTriggerEnter(Collider other)
    {
        GravityReceiver receiver = other.GetComponentInParent<GravityReceiver>();
        if(receiver != null && receiver.GetComponent<PlanetReceiver>() == null){
            if(!receivers.Contains(receiver)){
                receivers.Add(receiver);                    // Add the gravity receiver to the list of receivers
                receiver.AddLocalGravitySource(this);       // Tell the receiver that it is being affected by this local gravity source
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
        return strength * GetGravityDirection(Vector3.zero);
    }

    public override Vector3 GetGravityDirection(Vector3 point)
    {
        // Direction of gravity in a local gravity source will always be downwards
        return -transform.up;
    }

    public override Vector3 GetNorthDirection(Transform player)
    {
        return transform.forward;
    }

    public void RemoveReceiver(GravityReceiver receiver)
    {
        receivers.Remove(receiver);
    }

    public override Vector3 GetVelocity()
    {
        PlanetGravity planet = GetComponentInParent<PlanetGravity>();
        if(planet != null)
        {
            return planet.GetVelocity();
        }

        return GetComponentInParent<Rigidbody>().velocity;
    }

    public override Vector3 GetUp(Vector3 point)
    {
        return transform.up;
    }
}
