using UnityEngine;
using UnityEditor;
using UnityEngine.Experimental.TerrainAPI;
using System.Drawing;

public class TerrainGenerator : MonoBehaviour
{
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
        terrainData.size = new Vector3(nextPowerOfTwo(terrainWidth) + 1, terrainHeight, nextPowerOfTwo(terrainLength) + 1);
        terrainData.SetHeights(0, 0, generateHeights());
        return terrainData;
    }

    private float[,] generateHeights()
    {
        float[,] points = new float[nextPowerOfTwo(terrainWidth) + 1, nextPowerOfTwo(terrainLength) + 1];

        for (int i = 0; i < nextPowerOfTwo(terrainWidth) + 1; i++)
        {
            for (int j = 0; j < nextPowerOfTwo(terrainLength) + 1; j++)
            {
                float xCoord = (float)i / terrainWidth * noiseScale;
                float yCoord = (float)j / terrainLength * noiseScale;
                points[i, j] = Mathf.PerlinNoise(xCoord, yCoord);
            }
        }

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
