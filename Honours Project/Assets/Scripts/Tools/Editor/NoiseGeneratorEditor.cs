using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NoiseGenerator))]
public class NoiseGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        NoiseGenerator generator = (NoiseGenerator)target;

        if(GUILayout.Button("Generate Perlin Noise"))
        {
            generator.GeneratePerlinNoise();
        }

        if (GUILayout.Button("Generate Worley Noise"))
        {
            generator.GenerateWorleyNoise();
        }

        if(GUILayout.Button("Combine Textures"))
        {
            generator.CombineTextures();
        }
    }
}
