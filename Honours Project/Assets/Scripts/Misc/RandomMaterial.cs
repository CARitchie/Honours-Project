using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMaterial : MonoBehaviour
{
    [SerializeField] Material[] materials;

    private void Awake()
    {
        int index = Random.Range(0, materials.Length);
        GetComponent<SkinnedMeshRenderer>().material = materials[index];
        Destroy(this);
    }
}
