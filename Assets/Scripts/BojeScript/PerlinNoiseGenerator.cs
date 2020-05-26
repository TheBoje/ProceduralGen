using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoiseGenerator : MonoBehaviour
{
    /// <summary>Retourne un bruit de perlin de taille (width, height) 
    /// représenté par un tableau de float [0,1]</summary>
    public List<List<float>> perlinNoiseGenerateMap(int width, int height)
    {
        // Initialisation de la liste de points (un points est un float[0,1])
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
    /// <summary>Retourne la valeur du bruit de Perlin en (x, y) dans un tableau de (width, height)*scale (optionnal)</summary>
    public float perlinNoiseGeneratePoint(float x, float y, float width, float height, float scale = 1)
    {
        return Mathf.PerlinNoise((float)((x / width) * scale), (float)((y / height) * scale));
    }
}
