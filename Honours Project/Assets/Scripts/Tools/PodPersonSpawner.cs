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
}
