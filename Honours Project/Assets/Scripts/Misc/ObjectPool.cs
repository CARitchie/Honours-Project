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

    public static ObjectPool GetPool(string key)
    {
        if (dictionary == null || !dictionary.ContainsKey(key)) return null;
        return dictionary[key];
    }

}

