
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Security.Cryptography;
using UnityEngine;

public class PoissonSampling : MonoBehaviour
{
    [SerializeField] private float Rayon = 10f;
    [SerializeField] private int Iterations = 100;
    [SerializeField] private int Precision = 10000;
    [SerializeField] private int Dimension = 2;
    [SerializeField] private int Range_x = 500;
    [SerializeField] private int Range_z = 500;

    [SerializeField] private GameObject pointPoisson;

    private List<GameObject> instanciatedPoints;

    private float Cell_size;
    private int Rows_size;
    private int Cols_size;
    [SerializeField] private Vector3[,] grid;
    [SerializeField] public List<Vector3> active;

    private List<List<Vector3>> resultGrid;


    public void initPoisson()
    {
        Cell_size = (float)(Rayon / Math.Sqrt(Dimension));
        Rows_size = (int)Math.Floor(Range_x/Cell_size);
        Cols_size = (int)Math.Floor(Range_z/Cell_size);
        grid = new Vector3[Cols_size, Rows_size];
    }

    public void computePoints()
    {
        Vector3 randomPos;
        Vector3 activePos;
        Vector3 newPos;
        Vector3 neighborPos;
        Vector3Int randomPosFloored;
        Vector3Int newPosFloored;
        Int32 randomIndex;
        float randomMagnitude;
        float distance;
        int debugCount = 0;
        bool isFound;
        bool isCorrectDistance;

        initPoisson();

        randomPos = new Vector3(UnityEngine.Random.Range(0, Range_x), 0f, UnityEngine.Random.Range(0, Range_z));
        randomPosFloored = floorVector3(randomPos);
        grid[randomPosFloored.x, randomPosFloored.z] = randomPos;
        active.Add(randomPos);


        for (int l = 0; l < Precision; l++)
        {
            if (active.Count <= 0 && l != 0) { break; } // Safety check

            // Remember that Random.Range(float a, float b) is [a, b] (inclusive inclusive)
            //               Random.Range(int a, int b) is [a, b[ (inclusive exclusive)
            // That's bullshit!

            randomIndex = UnityEngine.Random.Range(0, active.Count - 1);
            activePos = active[randomIndex];
            isFound = false;

            for (int n = 0; n < Iterations; n++)
            {

                newPos = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f));
                randomMagnitude = UnityEngine.Random.Range(0f, (float)2 * Rayon);
                newPos = newPos * randomMagnitude;
                newPos += activePos;
                newPosFloored = floorVector3(newPos);
                if (0 <= newPosFloored.x && newPosFloored.x < Cols_size && 0 <= newPosFloored.z && newPosFloored.z < Rows_size && grid[newPosFloored.x, newPosFloored.z] == Vector3.zero)
                {
                    isCorrectDistance = true;
                    for (int i = -1; i < 2; i++)
                    {
                        for (int j = -1; j < 2; j++)
                        {
                            if (newPosFloored.x + i >= 0 && newPosFloored.x + i < Cols_size && newPosFloored.z + j >= 0 && newPosFloored.z + j < Rows_size)
                            {
                                neighborPos = grid[newPosFloored.x + i, newPosFloored.z + j];
                                if (neighborPos  != Vector3.zero)
                                {
                                    distance = Vector3.Distance(newPos, neighborPos);
                                    if (distance < 2 * Rayon)
                                    {
                                        isCorrectDistance = false;
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
    Debug.Log("Poisson Terminé");
    Debug.Log(debugCount);
    displayGrid();
    }

    public void displayGrid()
    {
        instanciatedPoints = new List<GameObject>();
        for (int i = 0; i < Cols_size; i++)
        {
            for (int j = 0; j < Rows_size; j++)
            {
                if (grid[i,j] != Vector3.zero)
                {
                    GameObject resultInstance = Instantiate(pointPoisson, grid[i, j], Quaternion.identity);
                    instanciatedPoints.Add(resultInstance);
                }
            }
        }
    }

    public void deleteComputed()
    {
        foreach (GameObject item in instanciatedPoints){
            Destroy(item);
        }
        instanciatedPoints.Clear();
    }
    private void printGrid() //Its not working for some unknown reasons 
    {
        resultGrid = new List<List<Vector3>>();
        for (int i = 0; i < Cols_size; i++)
        {
            List<Vector3> temp = new List<Vector3>();
            for (int j = 0; j < Rows_size; j++)
            {
                temp.Add(grid[i,j]);
            }
            resultGrid.Add(temp);
        }
        Debug.Log(resultGrid);
    }
    public Vector3Int floorVector3(Vector3 vec) {
        Vector3Int result;
        result = new Vector3Int((int)Math.Floor(vec.x / Cell_size), (int)vec.z, (int)Math.Floor(vec.y / Cell_size));
        return result;
    }
    void Start()
    {
        computePoints();
    }
}
