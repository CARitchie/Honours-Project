using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityController : MonoBehaviour
{
    public static float gravityConstant = 1f;
    static GravityController Instance;

    List<GravityReceiver> receivers = new List<GravityReceiver>();
    List<GravitySource> sources = new List<GravitySource>();

    private void Awake()
    {
        Instance = this;
    }

    public static void AddReceiver(GravityReceiver receiver)
    {
        if (Instance == null) return;

        Instance.receivers.Add(receiver);
    }

    public static void AddSource(GravitySource source)
    {
        if (Instance == null) return;

        Instance.sources.Add(source);
    }

    void AddForces()
    {
        float time = Time.fixedDeltaTime;

        foreach(GravityReceiver receiver in receivers)
        {
            receiver.CalculateForce(sources, time);
        }
    }

    private void FixedUpdate()
    {
        AddForces();
    }
}
