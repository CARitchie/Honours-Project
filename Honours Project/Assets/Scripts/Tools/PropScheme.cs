using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "My Assets/Prop Placer/Prop Scheme")]
public class PropScheme : ScriptableObject
{
    [SerializeField] PlaceableProp[] props;
    float sumOfWeights;

    public void CalculateWeights()
    {
        sumOfWeights = 0;
        foreach(PlaceableProp prop in props)
        {
            sumOfWeights += prop.GetWeight();
        }
    }

    // Function to find a random prop using a weighting
    public PlaceableProp GetRandomProp()
    {
        float bigWeight = Random.Range(0, sumOfWeights);
        for(int i = 0; i < props.Length; i++)
        {
            float weight = props[i].GetWeight();
            if (bigWeight < weight) return props[i];
            bigWeight -= weight;
        }

        return null;
    }

}
