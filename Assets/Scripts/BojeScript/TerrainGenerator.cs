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
        terrainData.size = new Vector3(terrainWidth, terrainHeight, terrainLength);
        terrainData.SetHeights(0, 0, generateHeights());
        return terrainData;
    }

    private float[,] generateHeights()
    {
        float[,] points = new float[terrainWidth, terrainLength];

        for (int i = 0; i < terrainWidth; i++)
        {
            for (int j = 0; j < terrainLength; j++)
            {
                float xCoord = (float)i / terrainWidth * noiseScale;
                float yCoord = (float)j / terrainLength * noiseScale;

                points[i, j] = Mathf.PerlinNoise(xCoord, yCoord);
            }
        }

        return points;
    }
    public float getTerrainLocalHeight(Vector3 position)
    {
        return terrainComponent.SampleHeight(position);
    }
}
