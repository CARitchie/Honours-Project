using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(Rigidbody))]
public class GravityReceiver : MonoBehaviour
{
    [SerializeField] protected float defaultMultiplier = 1;
    [SerializeField] protected float localGravityMultiplier = 1;

    protected Rigidbody rb;
    protected List<LocalGravitySource> localGravitySources = new List<LocalGravitySource>();

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected virtual void Start()
    {
        GravityController.AddReceiver(this);
    }

    // Function to apply gravity forces and account for the player's velocity
    public virtual void ApplyForce(List<PlanetGravity> sources, float time, Vector3 playerVelocity)
    {
        Vector3 force = CalculateForce(sources, time);

        if (float.IsNaN(force.x)) return;

        rb.AddForce(force);
        rb.AddForce(playerVelocity, ForceMode.VelocityChange);
    }

    // Function to calculate the force that should be applied
    protected Vector3 CalculateForce(List<PlanetGravity> sources, float time)
    {
        Vector3 force = Vector3.zero;

        float G = GravityController.gravityConstant;

        for (int i = 0; i < sources.Count; i++)
        {
            if (sources[i].transform != transform)
            {
                Vector3 distance = sources[i].transform.position - transform.position;

                float strength = (G * rb.mass * sources[i].GetMass()) / distance.sqrMagnitude;      // Use the law of gravitation to calculate the magnitude of the force
                force += distance.normalized * strength;
            }
        }

        force *= defaultMultiplier;

        force += GetLocalForce();       // Add the force originating from local gravity sources

        return force;
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

    // Function to determine the force originating from local gravity sources
    protected Vector3 GetLocalForce(){
        Vector3 force = Vector3.zero;
        foreach(LocalGravitySource gravitySource in localGravitySources){
            force += gravitySource.GetForce();
        }
        return force * rb.mass * localGravityMultiplier;
    }

    // Function to find the closest local gravity source
    protected LocalGravitySource ClosestLocalSource(){
        if(localGravitySources.Count < 1) return null;
        if(localGravitySources.Count < 2) return localGravitySources[0];

        // If there are more than one local gravity sources acting on this object
        // Find the local gravity source that is closest

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

    private void OnDestroy()
    {
        GravityController.RemoveReceiver(this);
    }

    // Find the closest gravity source
    public GravitySource FindClosest(List<PlanetGravity> sources)
    {
        GravitySource closest = null;

        if (localGravitySources.Count > 0)
        {
            LocalGravitySource localGravitySource = ClosestLocalSource();
            closest = localGravitySource;
            return closest;
        }

        float max = 1000000;
        for (int i = 0; i < sources.Count; i++)
        {
            if (sources[i].transform != transform)
            {
                Vector3 direction = sources[i].transform.position - transform.position;
                float magnitude = direction.sqrMagnitude;

                if (magnitude - sources[i].GetSquareDistance() < max && magnitude < sources[i].Influence)
                {
                    max = magnitude;
                    closest = sources[i];
                }
            }
        }

        return closest;
    }

    // Find the closest gravity source without considering influence ranges
    public GravitySource FindGlobalClosest(List<PlanetGravity> sources)
    {
        GravitySource closest = null;

        if (localGravitySources.Count > 0)
        {
            LocalGravitySource localGravitySource = ClosestLocalSource();
            closest = localGravitySource;
            return closest;
        }

        float max = float.PositiveInfinity;
        for (int i = 0; i < sources.Count; i++)
        {
            if (sources[i].transform != transform)
            {
                Vector3 direction = sources[i].transform.position - transform.position;
                float magnitude = direction.sqrMagnitude;

                if (magnitude - sources[i].GetSquareDistance() < max)
                {
                    max = magnitude;
                    closest = sources[i];
                }
            }
        }

        return closest;
    }

    public void SetRigidBody(Rigidbody rb)
    {
        this.rb = rb;
    }
}
