using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntersectionMaker : MonoBehaviour
{
    private List<Intersection> m_intersections;
    private PoissonSampling m_poissonScript;
    private Intersection[,] m_poissonGrid;
    private HalfEdgesMap m_halfEdgeMap;


    // Créer un tableau à deux dimensions d'intersections
    private void InitPoissonGrid()
    {
        m_poissonGrid = new Intersection[m_poissonScript.getRowSize, m_poissonScript.getColSize];

        for(int i = 0; i < m_poissonScript.getRowSize; i++)
        {
            for(int j = 0; j < m_poissonScript.getColSize; j++)
            {
                if(m_poissonScript.poissonGrid[i, j] != null)
                {
                    m_poissonGrid[i, j] = new Intersection((Vector3)m_poissonScript.poissonGrid[i, j], new Vector2Int(i, j));
                    //m_poissonGrid[i, j].GenerateIntersection(gameObject.transform);
                    m_halfEdgeMap.AddIsolatedDart((Vector3)m_poissonScript.poissonGrid[i, j]);
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
                            if(!m_poissonGrid[x, y].ContainsIntersection(m_poissonGrid[x + i, y + j]))
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

                    /*if (m_poissonGrid[x, y].Neighbours.Count > nbPointsSearched)
                        break;*/
                }
                /*if (m_poissonGrid[x, y].Neighbours.Count > nbPointsSearched)
                    break;*/
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
                    int nbNearPoints = UnityEngine.Random.Range(1, 3);
                    NearestPoint(x, y, nbNearPoints, rows, cols);
                }
            }
        }
    }



    // Nettoie les routes en enlevant les triangles
    public void DelTriangles()
    {
        for(int i = 0; i < m_poissonScript.getRowSize; i++)
        {
            for(int j = 0; j < m_poissonScript.getColSize; j++)
            {
                if(m_poissonGrid[i, j] != null)
                {
                    m_poissonGrid[i, j].DelTriangle(m_poissonGrid);
                }
            }
        }
    }

    // Instancie une route entre les deux positions
    private void GenerateRoad(Vector3 cr1, Vector3 cr2)
    {
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        Road road = plane.AddComponent<Road>();
        road.Init(cr1, cr2);
        road.SetRoad();
        plane.name = "Road";
        plane.transform.parent = transform;
    }

    // Calcul les routes en fonctions des voisins
    public void ComputeRoad(bool delTriangles)
    {
        m_halfEdgeMap = GameObject.Find("HalfEdge").GetComponent<HalfEdgesMap>();
        m_halfEdgeMap.Init();

        // va lancer la génération
        m_poissonScript = gameObject.GetComponent<PoissonSampling>();
        InitPoissonGrid();
        
        //m_poissonScript.threadedComputePoints();
        ComputeNearestPoint(m_poissonScript.getRowSize, m_poissonScript.getColSize); // TODO get des lignes et colonnes

        if (delTriangles)
            this.DelTriangles();

        for (int i = 0; i < m_poissonScript.getRowSize; i++)
        {
            for(int j = 0; j < m_poissonScript.getColSize; j++)
            {
                if(m_poissonGrid[i, j] != null)
                {
                    for (int n = 0; n < m_poissonGrid[i, j].Neighbours.Count; n++)
                    {
                        Vector2Int coords = m_poissonGrid[i, j].Neighbours[n].coords;

                        if (!(m_poissonGrid[i, j].Neighbours[n].joined) && !(m_poissonGrid[coords.x, coords.y].IsJoined(m_poissonGrid[i, j])))
                        {
                            m_poissonGrid[i, j].SetJoined(m_poissonGrid[coords.x, coords.y], true);
                            m_poissonGrid[coords.x, coords.y].SetJoined(m_poissonGrid[i, j], true);
                            
                            // Pour passer à la grille en 1 dimension -> j * maxI + i

                            int index = m_poissonGrid[coords.x, coords.y].IndexOfInter(m_poissonGrid[i, j]);
                            //Debug.Log("HalfEdges ind : " + m_poissonGrid[i, j].IndexInMap);

                            m_halfEdgeMap.LinkTwoPoints((Vector3)m_poissonScript.poissonGrid[i, j], (Vector3)m_poissonScript.poissonGrid[coords.x, coords.y]);

                            // Relie les routes entre les bordures de l'intersection
                            //GenerateRoad((Vector3)m_poissonGrid[i, j].Neighbours[n].positionOnIntersection, (Vector3)m_poissonGrid[coords.x, coords.y].Neighbours[index].positionOnIntersection);

                        }
                    }
                }
            }
        }
    }






    public void ClearIntersections()
    {
        m_intersections.Clear();
        foreach (Transform children in transform)
        {
            Destroy(children.gameObject);
        }
    }

    private void Start()
    {
        m_intersections = new List<Intersection>();

        //ComputeRoad();
    }
}
