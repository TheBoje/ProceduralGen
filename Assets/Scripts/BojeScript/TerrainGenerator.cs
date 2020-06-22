using UnityEngine;
using UnityEditor;
using UnityEngine.Experimental.TerrainAPI;
using System.Drawing;
using System.Collections.Generic;

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] private int octaveCount = 3;
    [SerializeField] private float frequency = 1f;
    [SerializeField] private float amplitude = 1f;

    private int terrainWidth;
    private int terrainLength;
    private float terrainHeight;
    private float noiseScale;
    private Terrain terrainComponent;
    private PoissonSampling poissonSamplingScript;

    public bool isDoneTerrain = false;

    private void Start()
    {
        terrainComponent = GetComponent<Terrain>();
        poissonSamplingScript = (PoissonSampling)GameObject.Find("GenManager").GetComponent<PoissonSampling>();
    }

    public void generateTerrain()
    {
        isDoneTerrain = false;
        terrainWidth = poissonSamplingScript.rangeX;
        terrainLength = poissonSamplingScript.rangeZ;
        terrainHeight = poissonSamplingScript.scaleY;
        noiseScale = poissonSamplingScript.perlinScale;
        terrainComponent.terrainData = generateTerrainData(terrainComponent.terrainData);
        isDoneTerrain = true;
    }

    private TerrainData generateTerrainData(TerrainData oldTerrainData)
    {
        TerrainData terrainData = oldTerrainData;
        terrainData.heightmapResolution = Mathf.Max(terrainWidth, terrainLength) + 1;
        terrainData.size = new Vector3(nextPowerOfTwo(terrainWidth) + 1, (int)(terrainHeight * 1.5), nextPowerOfTwo(terrainLength) + 1);
        terrainData.SetHeights(0, 0, generateHeights());
        return terrainData;
    }

    private float[,] generateHeights()
    {
        float[,] points = new float[nextPowerOfTwo(terrainWidth) + 1, nextPowerOfTwo(terrainLength) + 1];
        float init_freq= frequency;
        float init_ampl = amplitude;
        for (int i = 0; i < nextPowerOfTwo(terrainWidth) + 1; i++)
        {
            for (int j = 0; j < nextPowerOfTwo(terrainLength) + 1; j++)
            {
                points[i, j] = 0f;
                frequency = init_freq;
                amplitude = init_ampl;
                for (int k = 0; k < octaveCount; k++)
                {
                    float xCoord = (float)i / terrainWidth * noiseScale;
                    float yCoord = (float)j / terrainLength * noiseScale;
                    points[i, j] += Mathf.PerlinNoise(xCoord * frequency, yCoord * frequency) * amplitude;
                    frequency *= 2f;
                    amplitude /= 1.5f;
                }
            }
        }
        frequency = init_freq;
        amplitude = init_ampl;
        return points;
    }

    private int nextPowerOfTwo(int input)
    {
        int result = 1;
        while (result < input)
        {
            result *= 2;
        }
        return result;
    }

    public float getTerrainLocalHeight(Vector3 position)
    {
        return terrainComponent.SampleHeight(position);
    }
}