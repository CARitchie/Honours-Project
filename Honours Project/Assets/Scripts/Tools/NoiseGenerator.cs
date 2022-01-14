using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NoiseGenerator : MonoBehaviour
{
    [SerializeField] int dimension;

    [Header("Perlin")]
    [SerializeField] float smooth = 8;
    [SerializeField] string perlinLocation;

    [Header("Worley")]
    [SerializeField] int numberOfPoints;
    [SerializeField] float inverseBrightness = 80;
    [SerializeField] string worleyLocation;

    [Header("Combiner")]
    [SerializeField] Texture3D redTexture;
    [SerializeField] Texture3D greenTexture;
    [SerializeField] Texture3D blueTexture;
    [SerializeField] string outputLocation;

    Vector3[] points;
    double[,,] noise;

    void GenerateRandomNoise()
    {
        noise = new double[dimension, dimension, dimension];

        for(int z = 0; z < dimension; z++)
        {
            for(int y = 0; y < dimension; y++)
            {
                for(int x = 0; x < dimension; x++)
                {
                    noise[z, y, x] = (Random.Range(0, 32768) % 32768) / 32768.0;
                }
            }
        }
    }

    double smoothNoise(double x, double y, double z)
    {
        double fractX = x - (int)x;
        double fractY = y - (int)y;
        double fractZ = z - (int)z;

        int x1 = ((int)x + dimension) % dimension;
        int y1 = ((int)y + dimension) % dimension;
        int z1 = ((int)z + dimension) % dimension;

        int x2 = (x1 + dimension - 1) % dimension;
        int y2 = (y1 + dimension - 1) % dimension;
        int z2 = (z1 + dimension - 1) % dimension;

        double value = 0.0;
        value += fractX * fractY * fractZ * noise[z1, y1, x1];
        value += fractX * (1- fractY) * fractZ * noise[z1, y2, x1];
        value += (1 - fractX) * fractY * fractZ * noise[z1, y1, x2];
        value += (1 - fractX) * (1 - fractY) * fractZ * noise[z1, y2, x2];

        value += fractX * fractY * (1 - fractZ) * noise[z2, y1, x1];
        value += fractX * (1 - fractY) * (1 - fractZ) * noise[z2, y2, x1];
        value += (1 - fractX) * fractY * (1 - fractZ) * noise[z2, y1, x2];
        value += (1 - fractX) * (1 - fractY) * (1 - fractZ) * noise[z2, y2, x2];

        return value;
    }

    double turbulence(double x, double y, double z, double size)
    {
        double value = 0.0;
        double initialSize = size;

        while(size >= 1)
        {
            value += smoothNoise(x / size, y / size, z / size) * size;
            size /= 2;
        }

        return 128.0 * value / initialSize;
    }

    public void GeneratePerlinNoise()
    {
        Debug.Log("Generating Perlin Noise");

        GenerateRandomNoise();

        Texture3D texture = new Texture3D(dimension, dimension, dimension, TextureFormat.RGBA32, false);

        double size = smooth;

        for(int i = 0; i < dimension; i++)
        {
            for(int j = 0; j < dimension; j++)
            {
                for(int k = 0; k < dimension; k++)
                {
                    double turb = turbulence(i, j, k, size);
                    float val = (float)turb / 256.0f;

                    Color colour = new Color(val, val, val, 1);
                    texture.SetPixel(i, j, k, colour);
                }

            }
        }

        texture.Apply();

        AssetDatabase.CreateAsset(texture, perlinLocation);

        Debug.Log("Generating Perlin Noise: COMPLETE");
    }

    // https://www.youtube.com/watch?v=4066MndcyCk
    public void GenerateWorleyNoise()
    {
        Debug.Log("Generating Worley Noise");

        GeneratePoints();

        Texture3D texture = new Texture3D(dimension, dimension, dimension, TextureFormat.RGBA32, false);

        for (int i = 0; i < dimension; i++)
        {
            for (int j = 0; j < dimension; j++)
            {
                for(int k = 0; k < dimension; k++)
                {
                    float val = DistanceToClosest(new Vector3(i, j, k));
                    val = Remap(val, 0, inverseBrightness, 1, 0);

                    Color colour = new Color(val, val, val, 1);
                    texture.SetPixel(i, j, k, colour);
                }

            }
        }

        texture.Apply();

        AssetDatabase.CreateAsset(texture, worleyLocation);

        Debug.Log("Generating Worley Noise: COMPLETE");
    }

    float DistanceToClosest(Vector3 point)
    {
        float distance = Vector3.Distance(point, points[0]);
        for(int i = 1; i < points.Length; i++)
        {
            float newDistance = Vector3.Distance(point, points[i]);
            if (newDistance < distance) distance = newDistance;
        }

        return distance;
    }

    void GeneratePoints()
    {
        points = new Vector3[numberOfPoints * 27];
        Vector3[] initialPoints = new Vector3[numberOfPoints];

        for(int i = 0; i < numberOfPoints; i++)
        {
            initialPoints[i] = new Vector3(Random.Range(0, dimension), Random.Range(0, dimension), Random.Range(0, dimension));
            points[i] = initialPoints[i];
            points[i + numberOfPoints] = initialPoints[i] + new Vector3(-dimension, -dimension, -dimension);
            points[i + numberOfPoints * 2] = initialPoints[i] + new Vector3(0, -dimension, -dimension);
            points[i + numberOfPoints * 3] = initialPoints[i] + new Vector3(dimension, -dimension, -dimension);
            points[i + numberOfPoints * 4] = initialPoints[i] + new Vector3(-dimension, -dimension, 0);
            points[i + numberOfPoints * 5] = initialPoints[i] + new Vector3(0, -dimension, 0);
            points[i + numberOfPoints * 6] = initialPoints[i] + new Vector3(dimension, -dimension, 0);
            points[i + numberOfPoints * 7] = initialPoints[i] + new Vector3(-dimension, -dimension, dimension);
            points[i + numberOfPoints * 8] = initialPoints[i] + new Vector3(0, -dimension, dimension);
            points[i + numberOfPoints * 9] = initialPoints[i] + new Vector3(dimension, -dimension, dimension);
            points[i + numberOfPoints * 26] = initialPoints[i] + new Vector3(-dimension, 0, -dimension);
            points[i + numberOfPoints * 10] = initialPoints[i] + new Vector3(0, 0, -dimension);
            points[i + numberOfPoints * 11] = initialPoints[i] + new Vector3(dimension, 0, -dimension);
            points[i + numberOfPoints * 12] = initialPoints[i] + new Vector3(-dimension, 0, 0);
            points[i + numberOfPoints * 13] = initialPoints[i] + new Vector3(dimension, 0, 0);
            points[i + numberOfPoints * 14] = initialPoints[i] + new Vector3(-dimension, 0, dimension);
            points[i + numberOfPoints * 15] = initialPoints[i] + new Vector3(0, 0, dimension);
            points[i + numberOfPoints * 16] = initialPoints[i] + new Vector3(dimension, 0, dimension);
            points[i + numberOfPoints * 25] = initialPoints[i] + new Vector3(-dimension, dimension, -dimension);
            points[i + numberOfPoints * 17] = initialPoints[i] + new Vector3(0, dimension, -dimension);
            points[i + numberOfPoints * 18] = initialPoints[i] + new Vector3(dimension, dimension, -dimension);
            points[i + numberOfPoints * 19] = initialPoints[i] + new Vector3(-dimension, dimension, 0);
            points[i + numberOfPoints * 20] = initialPoints[i] + new Vector3(0, dimension, 0);
            points[i + numberOfPoints * 21] = initialPoints[i] + new Vector3(dimension, dimension, 0);
            points[i + numberOfPoints * 22] = initialPoints[i] + new Vector3(-dimension, dimension, dimension);
            points[i + numberOfPoints * 23] = initialPoints[i] + new Vector3(0, dimension, dimension);
            points[i + numberOfPoints * 24] = initialPoints[i] + new Vector3(dimension, dimension, dimension);
        }
    }

    float Remap(float val, float lo, float ho, float ln, float hn)
    {
        return ln + ((val - lo) * (hn - ln)) / (ho - lo);
    }

    public void CombineTextures()
    {
        Texture3D texture = new Texture3D(dimension, dimension, dimension, TextureFormat.RGBA32, false);

        for (int i = 0; i < dimension; i++)
        {
            for (int j = 0; j < dimension; j++)
            {
                for (int k = 0; k < dimension; k++)
                {
                    float r = redTexture.GetPixel(i, j, k).r;
                    float g = greenTexture.GetPixel(i, j, k).r;
                    float b = blueTexture.GetPixel(i, j, k).r;

                    Color colour = new Color(r, g, b, 1);
                    texture.SetPixel(i, j, k, colour);
                }

            }
        }

        texture.Apply();

        AssetDatabase.CreateAsset(texture, outputLocation);
    }
}
