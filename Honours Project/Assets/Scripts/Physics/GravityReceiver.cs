using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GravityReceiver : MonoBehaviour
{
    protected Rigidbody rb;
    protected List<LocalGravitySource> localGravitySources = new List<LocalGravitySource>();

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        GravityController.AddReceiver(this);
    }

    public virtual void CalculateForce(List<PlanetGravity> sources, float time)
    {
        Vector3 force = Vector3.zero;

        float G = GravityController.gravityConstant;

        for (int i = 0; i < sources.Count; i++)
        {
            if (sources[i].transform != transform)
            {
                Vector3 distance = sources[i].transform.position - transform.position;

                float strength = (G * rb.mass * sources[i].GetMass()) / distance.sqrMagnitude;
                force += distance.normalized * strength;
            }
        }

        force += GetLocalForce();

        rb.AddForce(force);
    }

    public void AddLocalGravitySource(LocalGravitySource gravitySource){
        if (!localGravitySources.Contains(gravitySource))
        {
            localGravitySources.Add(gravitySource);
        }
    }

    public void RemoveLocalGravitySource(LocalGravitySource gravitySource){
        localGravitySources.Remove(gravitySource);

    }

    protected Vector3 GetLocalForce(){
        Vector3 force = Vector3.zero;
        foreach(LocalGravitySource gravitySource in localGravitySources){
            force += gravitySource.GetForce();
        }
        return force;
    }

    protected LocalGravitySource ClosestLocalSource(){
        if(localGravitySources.Count < 1) return null;
        if(localGravitySources.Count < 2) return localGravitySources[0];

        LocalGravitySource source = localGravitySources[0];
        float dist = (source.transform.position - transform.position).sqrMagnitude;
        for(int i = 1 ; i < localGravitySources.Count ; i++){
            float distance = (localGravitySources[i].transform.position - transform.position).sqrMagnitude;
            if(distance < dist){
                dist = distance;
                source = localGravitySources[i];
            }
        }
        return source;
    }

    private void OnDisable()
    {
        foreach(LocalGravitySource source in localGravitySources)
        {
            source.RemoveReceiver(this);
        }
        localGravitySources.Clear();
    }
}
