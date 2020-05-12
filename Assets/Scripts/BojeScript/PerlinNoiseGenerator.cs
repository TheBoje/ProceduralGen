using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoiseGenerator : MonoBehaviour
{
    public List<List<float>> perlinNoiseGenerateMap(int width, int height)
    {
        List<List<float>> perlinResult = new List<List<float>>();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                perlinResult[i][j] = Mathf.PerlinNoise((float)(i / width), (float)(j / height));
            }
        }
        return perlinResult;
    }

    /*     public float perlinNoiseGeneratePoint(float x, float y, float width, float height, float scale)
        {
            return Mathf.PerlinNoise((float)((x / width) * scale), (float)((y / height) * scale));
        } */
}
