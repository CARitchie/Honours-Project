using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomLOD : MonoBehaviour
{
    [SerializeField] float distance;
    [SerializeField] GameObject lodObject;
    Transform player;

    private void Start()
    {
        player = Camera.main.transform;
    }

    private void Update()
    {
        if((player.position - lodObject.transform.position).sqrMagnitude > distance)
        {
            if (lodObject.activeInHierarchy) lodObject.SetActive(false);
        }
        else
        {
            if (!lodObject.activeInHierarchy) lodObject.SetActive(true);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (lodObject == null) return;
        Gizmos.DrawRay(lodObject.transform.position, lodObject.transform.forward * Mathf.Sqrt(distance));
    }
}
