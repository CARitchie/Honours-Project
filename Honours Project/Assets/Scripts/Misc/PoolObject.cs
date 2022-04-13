using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObject : MonoBehaviour
{
    protected virtual void Start()
    {
        gameObject.SetActive(false);
    }
    public virtual void OnExitQueue()
    {
        gameObject.SetActive(true);
    }
}
