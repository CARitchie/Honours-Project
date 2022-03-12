using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentFixer : MonoBehaviour
{
    [SerializeField] Transform trueParent;

    public void FixParent()
    {
        transform.parent = trueParent;
    }
}
