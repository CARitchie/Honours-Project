using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] PoolObject prefab;
    [SerializeField] int numberOfObjects;

    Queue<PoolObject> objects;

    private void Start()
    {
        objects = new Queue<PoolObject>();
        for(int i = 0; i < numberOfObjects; i++)
        {
            PoolObject poolObject = Instantiate(prefab, transform);
            objects.Enqueue(poolObject);
        }
    }

    public PoolObject GetObject()
    {
        PoolObject poolObject = objects.Dequeue();
        poolObject.OnExitQueue();
        objects.Enqueue(poolObject);
        return poolObject;
    }

}

