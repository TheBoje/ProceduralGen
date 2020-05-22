using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Security.Cryptography;
using System.Threading;
using System.Diagnostics;
using UnityEngine;

// Git branch TEST#1 
// Git branch Fonctionnalites
// Git branch Terrain 
public class PoissonSampling : MonoBehaviour
{
    [Header("Basic Settings")]
    // Git branch Fonctionnalites is working ?
    [SerializeField]
    [Tooltip("Seed de génération")]
    private int randomSeed = 1;

    [SerializeField]
    [Range(0f, 100f)]
    [Tooltip("Distance minimale entre chaque points")]
    private float rayonPoisson = 10f;
    // Git master is working aswell ? 
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

    Thread threadComputePoints;

    private System.Random randGiver = new System.Random(); // TODO Creer randomThread.cs car UnityEngine.Random.Range n'est pas autorisé dans un child thread - on utilise donc la bibliotheque C# directement
    private Intersection[,] grid;
    private List<GameObject> instanciatedPoints;
    private int rowsSize;
    private int colsSize;

    public void computePoints() // TODO poissonManager() qui gere l'appel des fonctions en fonction des booléens
    {


        //==================================================//
        //      Initialisation of all local variables       //
        //==================================================//


        Intersection newPos = new Intersection(Vector3.zero);
        Intersection randomPos = new Intersection(Vector3.zero);
        Intersection activePos = new Intersection(Vector3.zero);
        Intersection neighborPos = new Intersection(Vector3.zero);
        List<Intersection> activityPoints = new List<Intersection>();

        instanciatedPoints = new List<GameObject>();


        float cellSize = (float)(rayonPoisson / Math.Sqrt(dimension));
        rowsSize = (int)Math.Ceiling((float)rangeX / (float)cellSize);
        colsSize = (int)Math.Ceiling((float)rangeZ / (float)cellSize);

        grid = new Intersection[colsSize, rowsSize];

        for (int i = 0; i < colsSize; i++)
        {
            for (int j = 0; j < rowsSize; j++)
            {
                grid[i, j] = new Intersection(Vector3.zero);
            }
        }
        int poissonCount = 0;

        Stopwatch stopwatchTimerMain = new Stopwatch();
        randomPos = new Intersection(new Vector3(randomRangeFloatThreadSafe(0.0f, (float)rangeX), 0f, randomRangeFloatThreadSafe(0.0f, (float)rangeZ)));
        Vector3Int randomPosFloored = floorVector3(randomPos.position, cellSize);
        grid[randomPosFloored.x, randomPosFloored.z] = randomPos; //FIXME
        List<Intersection> active = new List<Intersection>();
        active.Add(randomPos);
        deleteComputed();

        //==================================================//
        //          Actual algorithm                        //
        //==================================================//

        stopwatchTimerMain.Start();
        for (int l = 0; l < precision; l++)
        {
            if (active.Count <= 0 && l != 0) { break; } // Safety check
            bool isFound = false;
            int randomIndex = randomRangeIntThreadSafe(0, active.Count);
            activePos = active[randomIndex];
            for (int n = 0; n < iterations; n++)
            {
                newPos = new Intersection(Vector3.zero);
                newPos.position = new Vector3(randomRangeFloatThreadSafe(-1.0f, 1.0f), 0f, randomRangeFloatThreadSafe(-1.0f, 1.0f)).normalized;
                float randomMagnitude = randomRangeFloatThreadSafe(0.0f, (float)(2 * rayonPoisson));
                newPos.position = newPos.position * randomMagnitude;
                newPos.position += activePos.position;
                Vector3Int newPosFloored = floorVector3(newPos.position, cellSize);
                if (0 <= newPos.position.x && newPos.position.x < rangeX && 0 <= newPos.position.z && newPos.position.z < rangeZ && 0 <= newPosFloored.x && newPosFloored.x < colsSize && 0 <= newPosFloored.z && newPosFloored.z < rowsSize && grid[newPosFloored.x, newPosFloored.z].position == Vector3.zero)
                {
                    bool isCorrectDistance = true;
                    for (int i = -1; i < 2; i++)
                    {
                        for (int j = -1; j < 2; j++)
                        {
                            if (newPosFloored.x + i >= 0 && newPosFloored.x + i < colsSize && newPosFloored.z + j >= 0 && newPosFloored.z + j < rowsSize)
                            {
                                neighborPos = grid[newPosFloored.x + i, newPosFloored.z + j];
                                if (neighborPos.position != Vector3.zero)
                                {
                                    float distance = Vector3.Distance(newPos.position, neighborPos.position);
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
                        active.Add(newPos);
                        poissonCount += 1;
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
        stopwatchTimerMain.Stop();
        UnityEngine.Debug.Log("Poisson - Placed " + poissonCount.ToString() + " points in " + (stopwatchTimerMain.ElapsedMilliseconds).ToString() + " ms | " + ((float)stopwatchTimerMain.ElapsedMilliseconds / (float)poissonCount).ToString() + "ms / pt");
    }


    public IEnumerator threadedComputePoints()
    {
        UnityEngine.Debug.Log("PoissonSampling::threadedComputePoints - Starting");
        UnityEngine.Random.seed = randomSeed; //FIXME Find a better solution to this 
        threadComputePoints = new Thread(computePoints);
        threadComputePoints.IsBackground = true;
        threadComputePoints.Start();
        while (threadComputePoints.IsAlive)
        {
            yield return null;
        }
        UnityEngine.Debug.Log("PoissonSampling::threadedComputePoints - Finished");
    }



    private float randomRangeFloatThreadSafe(float a, float b)
    {
        float result;
        result = a + (float)randGiver.NextDouble() * (b - a);
        return result;
    }


    private int randomRangeIntThreadSafe(int a, int b)
    {
        int result;
        result = randGiver.Next(a, b);
        return result;
    }


    public void displayGrid()
    {
        if (instanciateEnable)
        {
            for (int i = 0; i < colsSize; i++)
            {
                for (int j = 0; j < rowsSize; j++)
                {
                    if (grid[i, j].position != Vector3.zero)
                    {
                        GameObject resultInstance = Instantiate(objetInstance, grid[i, j].position, Quaternion.identity);
                        resultInstance.transform.parent = gameObject.transform;
                        instanciatedPoints.Add(resultInstance);
                    }
                }
            }
        }
    }
    private void displayPoint(Intersection newPos)
    {
        if (instanciateEnable)
        {
            Vector3 pt = newPos.position;
            GameObject resultInstance = Instantiate(objetInstance, pt, Quaternion.identity);
            resultInstance.transform.parent = gameObject.transform;
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
        threadComputePoints.IsBackground = false;
        for (int i = 0; i < colsSize; i++)
        {
            for (int j = 0; j < rowsSize; j++)
            {
                if (grid[i, j].position != Vector3.zero)
                {
                    Vector3 temp = grid[i, j].position;
                    temp.y = perlinNoiseGeneratePoint(temp.x, temp.z, rangeX, rangeZ, perlinScale) * scaleY;
                    grid[i, j].position = temp;
                }
            }
        }
        threadComputePoints.IsBackground = true;
    }
    public Vector3Int floorVector3(Vector3 vec, float cellSize)
    {
        Vector3Int result;
        result = new Vector3Int((int)Math.Floor(vec.x / cellSize), 0, (int)Math.Floor(vec.z / cellSize));
        return result;
    }

    private float perlinNoiseGeneratePoint(float x, float y, float width, float height, float scale)
    {
        return Mathf.PerlinNoise((float)((x / width) * scale), (float)((y / height) * scale));
    }

    public Intersection[,] poissonGrid
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
