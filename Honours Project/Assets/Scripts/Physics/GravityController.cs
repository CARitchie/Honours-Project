using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityController : MonoBehaviour
{
    [SerializeField] GravityReceiver player;

    public static float gravityConstant = 1f;
    static GravityController Instance;

    List<GravityReceiver> receivers = new List<GravityReceiver>();
    List<PlanetGravity> sources = new List<PlanetGravity>();

    Vector3 playerVelocity = Vector3.zero;

    private void Awake()
    {
        Instance = this;
    }

    public static void AddReceiver(GravityReceiver receiver)
    {
        if (Instance == null) return;

        Instance.receivers.Add(receiver);
    }

    public static void AddSource(PlanetGravity source)
    {
        if (Instance == null) return;

        Instance.sources.Add(source);
    }

    void AddForces()
    {
        float time = Time.fixedDeltaTime;

        Vector3 force = Vector3.zero;

        foreach(GravityReceiver receiver in receivers)
        {
            if(receiver.enabled) receiver.ApplyForce(sources, time, -playerVelocity);
        }
    }

    private void FixedUpdate()
    {
        AddForces();

        playerVelocity = Vector3.zero;
    }

    public static GravitySource FindClosest(GravityReceiver receiver, bool global = false)
    {
        if (Instance == null) return null;

        if (!global)
        {
            return receiver.FindClosest(Instance.sources);
        }
        else
        {
            return receiver.FindGlobalClosest(Instance.sources);
        }
        
    }


    public static void RemoveReceiver(GravityReceiver receiver)
    {
        if (Instance == null) return;

        Instance.receivers.Remove(receiver);
    }

    public static void AddToPlayerVelocity(Vector3 velocity)
    {
        if (Instance == null) return;

        Instance.playerVelocity += velocity;
    }

    public static void SetPlayer(GravityReceiver receiver)
    {
        if (Instance == null) return;

        Instance.player = receiver;
    }

    public static bool FindSource(string key, out GravitySource source)
    {
        source = null;
        if (Instance == null) return false;

        GravitySource[] allSources = FindObjectsOfType<GravitySource>();
        for (int i = 0; i < allSources.Length; i++)
        {
            if (allSources[i].Key == key)
            {
                source = allSources[i];
                return true;
            }
        }

        return false;
    }
}
