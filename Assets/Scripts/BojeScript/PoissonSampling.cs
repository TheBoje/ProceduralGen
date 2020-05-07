
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Security.Cryptography;
using System.Diagnostics;
using UnityEngine;

public class PoissonSampling : MonoBehaviour
{
    [Header("Basic Settings")]

    [SerializeField]
    [Range(0f, 100f)]
    [Tooltip("Distance minimale entre chaque points")]
    private float rayonPoisson = 10f;

    [SerializeField]
    [Tooltip("Nombre d'essais par nouveau point")]
    private int iterations = 100;

    [SerializeField]
    [Tooltip("Nombre d'essais de point")]
    private int precision = 10000;

    [SerializeField]
    [Range(2, 2)]
    [Tooltip("WIP - NE PAS MODIFIER")]
    private int dimension = 2;

    [SerializeField]
    [Range(0f, 2000f)]
    [Tooltip("Taille sur X")]
    public int rangeX = 500;

    [SerializeField]
    [Range(0f, 2000f)]
    [Tooltip("Taille sur Z")]
    public int rangeZ = 500;
    [Header("Y Settings")]

    [SerializeField]
    [Tooltip("Active le positionnement sur Y")]
    private bool scaleYEnable = true;

    [SerializeField]
    [Range(0f, 100f)]
    [Tooltip("Taille sur Y")]
    public float scaleY = 2f;

    [SerializeField]
    [Range(0f, 10f)]
    [Tooltip("Taille du masque de bruit de Perlin")]
    public float perlinScale = 2f;

    [Header("Display Settings")]

    [SerializeField]
    [Tooltip("Active l'instanciation")]
    private bool instanciateEnable = true;

    [SerializeField]
    [Tooltip("Objet instancié a chaque point calculé")]
    private GameObject objetInstance;

    [Header("Activity Settings")]

    [SerializeField] private bool activityEnable = true;
    [SerializeField] private int activityConcentration;


    private Vector3[,] grid;
    private List<GameObject> instanciatedPoints = new List<GameObject>();
    private List<List<Vector3>> resultGrid;
    private List<Vector3> active = new List<Vector3>();
    private List<Vector3> activityPoints;
    private Vector3 newPos;
    private Vector3 randomPos;
    private Vector3 activePos;
    private Vector3 neighborPos;
    private Vector3Int randomPosFloored;
    private Vector3Int newPosFloored;
    private Stopwatch stopwatchTimer;
    private float cellSize;
    private float randomMagnitude;
    private float distance;
    private float activityRange;
    private int colsSize;
    private int rowsSize;
    private int namingCount;
    private int randomIndex;
    private int debugCount;
    private bool isFound;
    private bool isCorrectDistance;

    public void initPoisson()
    {
        deleteComputed();
        cellSize = (float)(rayonPoisson / Math.Sqrt(dimension));
        rowsSize = (int)Math.Ceiling(rangeX / cellSize);
        colsSize = (int)Math.Ceiling(rangeZ / cellSize);
        grid = new Vector3[colsSize, rowsSize];
        namingCount = 0;
        debugCount = 0;
        stopwatchTimer = new Stopwatch();
        stopwatchTimer.Start();
        randomPos = new Vector3(UnityEngine.Random.Range(0.0f, (float)rangeX), 0f, UnityEngine.Random.Range(0.0f, (float)rangeZ));
        randomPosFloored = floorVector3(randomPos);
        grid[randomPosFloored.x, randomPosFloored.z] = randomPos; //FIXME
        active.Add(randomPos);
    }
    public void computePoints()
    {
        initPoisson();
        for (int l = 0; l < precision; l++)
        {
            if (active.Count <= 0 && l != 0) { break; } // Safety check
            isFound = false;
            randomIndex = UnityEngine.Random.Range(0, active.Count);
            activePos = active[randomIndex];
            for (int n = 0; n < iterations; n++)
            {
                newPos = new Vector3(UnityEngine.Random.Range(-1.0f, 1.0f), 0f, UnityEngine.Random.Range(-1.0f, 1.0f)).normalized;
                randomMagnitude = UnityEngine.Random.Range(0.0f, (float)(2 * rayonPoisson));
                newPos = newPos * randomMagnitude;
                newPos += activePos;
                newPosFloored = floorVector3(newPos);
                if (0 <= newPos.x && newPos.x < rangeX && 0 <= newPos.z && newPos.z < rangeZ && 0 <= newPosFloored.x && newPosFloored.x < colsSize && 0 <= newPosFloored.z && newPosFloored.z < rowsSize && grid[newPosFloored.x, newPosFloored.z] == Vector3.zero)
                {
                    isCorrectDistance = true;
                    for (int i = -1; i < 2; i++)
                    {
                        for (int j = -1; j < 2; j++)
                        {
                            if (newPosFloored.x + i >= 0 && newPosFloored.x + i < colsSize && newPosFloored.z + j >= 0 && newPosFloored.z + j < rowsSize)
                            {
                                neighborPos = grid[newPosFloored.x + i, newPosFloored.z + j];
                                if (neighborPos != Vector3.zero)
                                {
                                    Vector2 pt1 = new Vector2(newPos.x, newPos.z);
                                    Vector2 pt2 = new Vector2(neighborPos.x, neighborPos.z);
                                    distance = Vector2.Distance(pt1, pt2);
                                    if (distance < 2 * rayonPoisson)
                                    {
                                        isCorrectDistance = false;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (isCorrectDistance)
                    {
                        grid[newPosFloored.x, newPosFloored.z] = newPos;
                        debugCount += 1;
                        active.Add(newPos);
                        isFound = true;
                    }
                }
            }
            if (!isFound)
            {
                active.Remove(active[randomIndex]);
            }
        }
        if (scaleYEnable)
        {
            computePointsHeight();
        }
        displayGrid();
        stopwatchTimer.Stop();
        UnityEngine.Debug.Log("Poisson finished - Placed " + debugCount.ToString() + " points in " + (stopwatchTimer.ElapsedMilliseconds).ToString() + " ms ");
    }
    public void displayGrid()
    {
        if (instanciateEnable)
        {
            for (int i = 0; i < colsSize; i++)
            {
                for (int j = 0; j < rowsSize; j++)
                {
                    if (grid[i, j] != Vector3.zero)
                    {
                        GameObject resultInstance = Instantiate(objetInstance, grid[i, j], Quaternion.identity);
                        resultInstance.name = namingCount.ToString();
                        namingCount++;
                        instanciatedPoints.Add(resultInstance);
                    }
                }
            }
        }
    }
    public void displayPoint()
    {
        if (instanciateEnable)
        {
            Vector3 pt = newPos;
            GameObject resultInstance = Instantiate(objetInstance, pt, Quaternion.identity);
            resultInstance.name = namingCount.ToString();
            namingCount++;
            instanciatedPoints.Add(resultInstance);
        }
    }
    public void deleteComputed()
    {
        if (instanciatedPoints.Count > 0)
        {
            foreach (GameObject item in instanciatedPoints)
            {
                Destroy(item);
            }
            instanciatedPoints.Clear();
        }
    }
    public void computePointsHeight()
    {
        for (int i = 0; i < colsSize; i++)
        {
            for (int j = 0; j < rowsSize; j++)
            {
                if (grid[i, j] != Vector3.zero)
                {
                    Vector3 temp = grid[i, j];
                    temp.y = GetComponent<PerlinNoiseGenerator>().perlinNoiseGeneratePoint(temp.x, temp.z, rangeX, rangeZ, perlinScale) * scaleY;
                    grid[i, j] = temp;
                }
            }
        }
    }
    public Vector3Int floorVector3(Vector3 vec)
    {
        Vector3Int result;
        result = new Vector3Int((int)Math.Floor(vec.x / cellSize), 0, (int)Math.Floor(vec.z / cellSize));
        return result;
    }
    public Vector3[,] poissonGrid
    {
        get { return grid; }
    }
    public int getRowSize
    {
        get { return rowsSize; }
    }
    public int getColSize
    {
        get { return colsSize; }
    }
}
