using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PodPersonSpawner : MonoBehaviour
{
    [SerializeField] PodPerson[] options;

    GameObject person;
    static float sumOfWeights = -1;

    public void SpawnPerson()
    {
        PodPerson chosen = GetRandomPerson();
        if(chosen != null) person = chosen.SpawnPerson(transform);
    }

    public void DestroyPerson()
    {
        if (person != null) Object.DestroyImmediate(person);
        person = null;
    }

    public void CalculateWeights()
    {
        sumOfWeights = 0;
        foreach (PodPerson person in options)
        {
            sumOfWeights += person.GetWeight();
        }
    }

    // Function to return a random person model
    public PodPerson GetRandomPerson()
    {
        float bigWeight = Random.Range(0, sumOfWeights);
        for (int i = 0; i < options.Length; i++)
        {
            float weight = options[i].GetWeight();
            if (bigWeight < weight) return options[i];
            bigWeight -= weight;
        }

        return null;
    }

    // Function to destroy unnecessary gameobjects
    public void TidyUp()
    {
        List<GameObject> objects = FindChildren(transform);
        for(int i = 0; i < objects.Count; i++)
        {
            if (!objects[i].activeSelf) Object.DestroyImmediate(objects[i]);
            objects[i] = null;
        }
    }

    // Function to return a list of all child gameobjects
    public List<GameObject> FindChildren(Transform parent)
    {
        List<GameObject> children = new List<GameObject>();
        for (int i = 0; i < parent.childCount; i++)
        {
            children.Add(parent.GetChild(i).gameObject);
        }

        for (int i = 0; i < parent.childCount; i++)
        {
            if (children[i].activeSelf)
            {
                List<GameObject> newChildren = FindChildren(children[i].transform);
                foreach(GameObject child in newChildren)
                {
                    children.Add(child);
                }
            }
        }

        return children;
    }
}
