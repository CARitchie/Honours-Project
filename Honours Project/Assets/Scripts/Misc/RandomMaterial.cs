using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMaterial : MonoBehaviour
{
    [SerializeField] Renderer[] renderers;
    [SerializeField] Material[] materials;

    private void Awake()
    {
        int index = Random.Range(0, materials.Length);      // Select a random material
        foreach(Renderer renderer in renderers)
        {
            renderer.material = materials[index];           // Apply the selected material to the renderer
        }       
        Destroy(this);
    }
}
