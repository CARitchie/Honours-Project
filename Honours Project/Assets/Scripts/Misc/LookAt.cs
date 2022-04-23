using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] bool inverted = false;
    // Update is called once per frame
    void Update()
    {
        // Look at, or away from, the specified target transform
        transform.rotation = Quaternion.LookRotation(!inverted ? target.position - transform.position : transform.position - target.position);
    }
}
