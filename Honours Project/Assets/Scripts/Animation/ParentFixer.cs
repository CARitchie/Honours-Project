using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentFixer : MonoBehaviour
{
    [SerializeField] Transform trueParent;

    // Set the parent of the transform equal to the desired transform
    public void FixParent()
    {
        transform.parent = trueParent;
    }
}
