using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PropArea : MonoBehaviour
{
    [SerializeField] float radius;
    [SerializeField] PropScheme scheme;
    [SerializeField] int numberOfProps;
    [SerializeField] bool checkCollision;
    PlanetGravity source;
    float height;

    public void GenerateProps()
    {
        source = GetComponentInParent<PlanetGravity>();

        int progress = 0;
        scheme.CalculateWeights();
        height = source.GetDistance() + 10;
        while (progress < numberOfProps)
        {
            PlaceableProp prop = scheme.GetRandomProp();
            if (prop == null) continue;

            Vector3 pos = FindPosition();

            if (pos != Vector3.zero)
            {
                Vector3 rot = GetRotation(pos);

                prop.Spawn(transform, pos, rot);
            }

            progress++;
        }
    }

    Vector3 GetRotation(Vector3 pos)
    {
        return (pos - source.transform.position);
    }

    Vector3 FindPosition()
    {
        int numberOfAttempts = 50;
        while (numberOfAttempts > 0)
        {
            numberOfAttempts--;

            Vector3 pos = Random.onUnitSphere * radius + transform.position;

            Vector3 dir = (pos - source.transform.position).normalized * height;

            pos = source.transform.position + dir;

            if (!Physics.Raycast(pos, -dir, out RaycastHit hit, height)) continue;


            if (hit.collider.gameObject.layer != 8 && checkCollision) continue;

            return hit.point;
        }

        Debug.Log("COULDN'T PLACE PROP");
        return Vector3.zero;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 0, 1, 0.1f);
        Gizmos.DrawSphere(transform.position, radius);
    }

    public void DestroyProps()
    {
        List<GameObject> children = new List<GameObject>();
        for(int i =0; i < transform.childCount; i++)
        {
            children.Add(transform.GetChild(i).gameObject);
        }

        for(int i = 0; i < children.Count; i++)
        {
            if(children[i] != gameObject)
            {
                Object.DestroyImmediate(children[i]);
                children[i] = null;
            }

        }

        children.Clear();
    }

}
