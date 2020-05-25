using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntersectionMaker : MonoBehaviour
{
    private List<Intersection> m_intersections;
    private PoissonSampling m_poissonScript;

    // Calcul les points les plus proches et retourne la liste des points
    private void NearestPoint(int x, int y, int nbPointsSearched, int rows, int cols) //
    {
        int length = 1;

        Intersection intersection = m_poissonScript.poissonGrid[x, y];

        while (intersection.Neighbours.Count <= nbPointsSearched)
        {
            for (int i = -length; i <= length; i++)
            {
                for (int j = -length + Mathf.Abs(i); j <= length - Mathf.Abs(i); j++)
                {
                    if ((0 <= x + i) && (x + i < rows) && (0 <= y + j) && (y + j < cols) && !(i == 0 && j == 0))
                    {
                        Intersection neighbour = m_poissonScript.poissonGrid[x + i, y + j];
                        if (neighbour.position != Vector3.zero)
                        {
                            intersection.AddNeighbour(neighbour);

                            // Vérifie si l'intersection n'est pas déjà dans la liste et le rajoute
                            if (!neighbour.ContainsIntersection(intersection))
                            {
                                neighbour.AddNeighbour(intersection);
                            }
                                
                        }
                    }
                }
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
                if (m_poissonScript.poissonGrid[x, y].position != Vector3.zero)
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
                if(m_poissonScript.poissonGrid[i, j].position != Vector3.zero)
                {
                    m_poissonScript.poissonGrid[i, j].DelTriangle();
                }
            }
        }
    }

    // Génère les routes en fonctions des voisins
    public void ComputeRoad(bool delTriangles)
    {
        // va lancer la génération
        m_poissonScript = gameObject.GetComponent<PoissonSampling>();
        Intersection[,] poissonGrid = m_poissonScript.poissonGrid;
        
        //m_poissonScript.threadedComputePoints();
        ComputeNearestPoint(m_poissonScript.getRowSize, m_poissonScript.getColSize); // TODO get des lignes et colonnes

        if (delTriangles)
            this.DelTriangles();

        for(int i = 0; i < m_poissonScript.getRowSize; i++)
        {
            for(int j = 0; j < m_poissonScript.getColSize; j++)
            {
                if(poissonGrid[i, j].position != Vector3.zero)
                {
                    for(int n = 0; n < poissonGrid[i, j].Neighbours.Count; n++)
                    {
                        
                        // on vérifie si les deux intersections ne sont pas jointes
                        if (!poissonGrid[i, j].Neighbours[n].joined && !poissonGrid[i, j].Neighbours[n].inter.IsJoined(poissonGrid[i, j]))
                        {
                            poissonGrid[i, j].Neighbours[n].SetJoined(true);
                            poissonGrid[i, j].Neighbours[n].inter.SetJoined(poissonGrid[i, j], true);

                            GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                            Road road = plane.AddComponent<Road>();
                            road.Init(m_poissonScript.poissonGrid[i, j].position, poissonGrid[i, j].Neighbours[n].inter.position);
                            road.SetRoad();
                            plane.transform.parent = transform;
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
