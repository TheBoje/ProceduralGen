using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using UnityEngine;

public class IntersectionMaker : MonoBehaviour
{
    private List<Intersection> m_intersections;
    private PoissonSampling m_poissonScript;
    private Intersection[,] m_poissonGrid;
    private List<List<Vector3>> toGenerateRoad = new List<List<Vector3>>();
    private Thread threadComputeRoad;
    [SerializeField] bool delTriangles = false;


    // Créer un tableau à deux dimensions d'intersections
    private void InitPoissonGrid()
    {
        m_poissonGrid = new Intersection[m_poissonScript.getRowSize, m_poissonScript.getColSize];
        toGenerateRoad.Clear();
        for (int i = 0; i < m_poissonScript.getRowSize; i++)
        {
            for (int j = 0; j < m_poissonScript.getColSize; j++)
            {
                if (m_poissonScript.poissonGrid[i, j] != null)
                {
                    m_poissonGrid[i, j] = new Intersection((Vector3)m_poissonScript.poissonGrid[i, j], new Vector2Int(i, j));
                    m_poissonGrid[i, j].GenerateIntersection(gameObject.transform.Find("Intersections").transform); // FIXME Trouver le bon gameObject
                }
                else
                {
                    m_poissonGrid[i, j] = null;
                }
            }
        }
    }

    // Calcul les points les plus proches et retourne la liste des points
    private void NearestPoint(int x, int y, int nbPointsSearched, int rows, int cols) //
    {
        int length = 1;

        while (m_poissonGrid[x, y].Neighbours.Count <= nbPointsSearched)
        {
            for (int i = -length; i <= length; i++)
            {
                for (int j = -length + Mathf.Abs(i); j <= length - Mathf.Abs(i); j++)
                {
                    if ((0 <= x + i) && (x + i < rows) && (0 <= y + j) && (y + j < cols) && !(i == 0 && j == 0))
                    {
                        if (m_poissonGrid[x + i, y + j] != null)
                        {
                            if (!m_poissonGrid[x, y].ContainsIntersection(m_poissonGrid[x + i, y + j]))
                            {
                                m_poissonGrid[x, y].AddNeighbour(x + i, y + j, m_poissonGrid[x + i, y + j].position);
                            }

                            // Vérifie si l'intersection n'est pas déjà dans la liste et le rajoute
                            if (!m_poissonGrid[x + i, y + j].ContainsIntersection(m_poissonGrid[x, y]))
                            {
                                m_poissonGrid[x + i, y + j].AddNeighbour(x, y, m_poissonGrid[x, y].position);
                            }

                        }
                    }

                    if (m_poissonGrid[x, y].Neighbours.Count > nbPointsSearched)
                        break;
                }
                if (m_poissonGrid[x, y].Neighbours.Count > nbPointsSearched)
                    break;
            }
            length++;
        }
    }



    private void ComputeNearestPoint(int rows, int cols)
    {
        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < cols; y++)
            {
                if (m_poissonGrid[x, y] != null)
                {
                    int nbNearPoints = m_poissonScript.randomRangeIntThreadSafe(1, 3);
                    NearestPoint(x, y, nbNearPoints, rows, cols);
                }
            }
        }
    }



    // Nettoie les routes en enlevant les triangles
    public void DelTriangles()
    {
        for (int i = 0; i < m_poissonScript.getRowSize; i++)
        {
            for (int j = 0; j < m_poissonScript.getColSize; j++)
            {
                if (m_poissonGrid[i, j] != null)
                {
                    m_poissonGrid[i, j].DelTriangle(m_poissonGrid);
                }
            }
        }
    }

    // Instancie toutes les routes contenues dans la liste toGenerateRoad
    private IEnumerator GenerateRoad()
    {
        Stopwatch stopwatchGenerateRoad = new Stopwatch();
        int roadCount = 0;
        int internalGenerateRoadCount = 0;
        stopwatchGenerateRoad.Start();

        foreach (List<Vector3> coords in toGenerateRoad)
        {
            GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            Road road = plane.AddComponent<Road>();
            road.Init(coords[0], coords[1]);
            road.SetRoad();
            plane.name = "Road";
            plane.transform.parent = gameObject.transform.Find("Roads").transform;
            roadCount += 1;
            internalGenerateRoadCount += 1;
            if (internalGenerateRoadCount > 100)
            {
                internalGenerateRoadCount = 0;
                yield return null;
            }
        }
        stopwatchGenerateRoad.Stop();
        UnityEngine.Debug.Log("IntersectionMaker::GenerateRoad - Placed " + roadCount + " roads in  " + stopwatchGenerateRoad.ElapsedMilliseconds + " ms | " + (float)stopwatchGenerateRoad.ElapsedMilliseconds / (float)roadCount + "ms / pt");
    }

    // Calcul les routes en fonctions des voisins
    public void ComputeRoad()
    {
        threadComputeRoad.IsBackground = true;

        int roadCount = 0;
        Stopwatch stopwatchComputeRoad = new Stopwatch();
        stopwatchComputeRoad.Start();

        ComputeNearestPoint(m_poissonScript.getRowSize, m_poissonScript.getColSize); // TODO get des lignes et colonnes


        if (delTriangles)
            this.DelTriangles();

        for (int i = 0; i < m_poissonScript.getRowSize; i++)
        {
            for (int j = 0; j < m_poissonScript.getColSize; j++)
            {
                if (m_poissonGrid[i, j] != null)
                {
                    for (int n = 0; n < m_poissonGrid[i, j].Neighbours.Count; n++)
                    {
                        Vector2Int coords = m_poissonGrid[i, j].Neighbours[n].coords;

                        if (!(m_poissonGrid[i, j].Neighbours[n].joined) && !(m_poissonGrid[coords.x, coords.y].IsJoined(m_poissonGrid[i, j])))
                        {
                            m_poissonGrid[i, j].SetJoined(m_poissonGrid[coords.x, coords.y], true);
                            m_poissonGrid[coords.x, coords.y].SetJoined(m_poissonGrid[i, j], true);

                            int index = m_poissonGrid[coords.x, coords.y].IndexOfInter(m_poissonGrid[i, j]);

                            // Relie les routes entre les bordures de l'intersection
                            List<Vector3> newRoad = new List<Vector3>();
                            newRoad.Add((Vector3)m_poissonGrid[i, j].Neighbours[n].positionOnIntersection);
                            newRoad.Add((Vector3)m_poissonGrid[coords.x, coords.y].Neighbours[index].positionOnIntersection);
                            toGenerateRoad.Add(newRoad);
                            roadCount += 1;
                        }
                    }
                }
            }
        }
        stopwatchComputeRoad.Stop();
        UnityEngine.Debug.Log("IntersectionMaker::ComputeRoad - Computed " + roadCount + " roads in " + stopwatchComputeRoad.ElapsedMilliseconds + " ms | " + (float)stopwatchComputeRoad.ElapsedMilliseconds / (float)roadCount + "ms / pt");
    }


    public IEnumerator threadedComputeRoad()
    {
        InitPoissonGrid();
        threadComputeRoad = new Thread(ComputeRoad);
        threadComputeRoad.Start();
        threadComputeRoad.IsBackground = true;
        UnityEngine.Debug.Log("IntersectionMaker::threadedComputeRoad - Starting");

        while (threadComputeRoad.IsAlive)
        {
            yield return null;
        }
        UnityEngine.Debug.Log("IntersectionMaker::threadedComputePoints - Finished");
        StartCoroutine(GenerateRoad());
    }



    public void ClearInstanciated()
    {
        StartCoroutine(ClearIntersection());
    }

    private IEnumerator ClearIntersection()
    {
        int internalClearIntersectionsCount = 0;
        Transform intersectionTransform = transform.Find("Intersections");
        for (int i = intersectionTransform.childCount; i > 0; i--)
        {
            Destroy(intersectionTransform.GetChild(i - 1).gameObject);
            internalClearIntersectionsCount += 1;
            if (internalClearIntersectionsCount > 100)
            {
                internalClearIntersectionsCount = 0;
                yield return null;
            }
        }
        yield return StartCoroutine(ClearRoads());
    }

    private IEnumerator ClearRoads()
    {
        int internalClearRoadsCount = 0;
        Transform roadsTransform = transform.Find("Roads");
        for (int i = roadsTransform.childCount; i > 0; i--)
        {
            Destroy(roadsTransform.GetChild(i - 1).gameObject);
            internalClearRoadsCount += 1;
            if (internalClearRoadsCount > 100)
            {
                internalClearRoadsCount = 0;
                yield return null;
            }
        }
    }

    private void Start()
    {
        m_intersections = new List<Intersection>();
        m_poissonScript = gameObject.GetComponent<PoissonSampling>();
    }
}
