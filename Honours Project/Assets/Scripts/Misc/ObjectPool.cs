using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] string key;
    [SerializeField] PoolObject prefab;
    [SerializeField] int numberOfObjects;

    static Dictionary<string, ObjectPool> dictionary = new Dictionary<string, ObjectPool>();

    Queue<PoolObject> objects;

    private void Awake()
    {
        if (dictionary.ContainsKey(key))
        {
            dictionary[key] = this;
        }
        else
        {
            dictionary.Add(key, this);
        }
    }

    private void Start()
    {
        objects = new Queue<PoolObject>();                                  // Create a new queue
        for(int i = 0; i < numberOfObjects; i++)
        {
            PoolObject poolObject = Instantiate(prefab, transform);         // Create the desired number of pool objects
            objects.Enqueue(poolObject);                                    // Add them to the queue
        }
    }

    // Function to retrieve a pool object
    public PoolObject GetObject()
    {
        PoolObject poolObject = objects.Dequeue();
        poolObject.OnExitQueue();
        objects.Enqueue(poolObject);
        return poolObject;
    }

    // Function to gain access to a specific pool
    public static ObjectPool GetPool(string key)
    {
        if (dictionary == null || !dictionary.ContainsKey(key)) return null;
        return dictionary[key];
    }

}

