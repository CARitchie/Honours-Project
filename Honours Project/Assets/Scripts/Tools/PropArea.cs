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
        while (progress < numberOfProps)                        // Loop for as many props as wanted
        {
            PlaceableProp prop = scheme.GetRandomProp();        // Get a random prop
            if (prop == null) continue;                         // Go to next loop if no prop was found

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

    // Function to find a spawn location
    Vector3 FindPosition()
    {
        int numberOfAttempts = 50;
        while (numberOfAttempts > 0)
        {
            numberOfAttempts--;

            Vector3 pos = Random.onUnitSphere * radius + transform.position;            // Choose a random point on the planet's surface

            Vector3 dir = (pos - source.transform.position).normalized * height;

            pos = source.transform.position + dir;

            if (!Physics.Raycast(pos, -dir, out RaycastHit hit, height)) continue;      // Go to next loop if there was no collision with planet


            if (hit.collider.gameObject.layer != 8 && checkCollision) continue;         // Go to next loop if collision wasn't planet and collisions aren't allowed

            return hit.point;
        }

        Debug.Log("COULDN'T PLACE PROP");
        return Vector3.zero;
    }

    private void OnDrawGizmosSelected()
    {
        // Draw the area where props can be spawned
        Gizmos.color = new Color(0, 0, 1, 0.1f);
        Gizmos.DrawSphere(transform.position, radius);
    }

    // Function to destroy all props that have been generated
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
