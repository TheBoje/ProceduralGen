using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntersectionMaker : MonoBehaviour
{
    private List<Intersection> m_intersections;
    private GameObject poisson;


    // Calcul les points les plus proches et retourne la liste des points
    private List<Vector3> NearestPoint(int x, int y, int nbPointsSearched, int rows, int cols)
    {
        int length = 1;
        List<Vector3> nearestPoints = new List<Vector3>();

        while(nearestPoints.Count < nbPointsSearched)
        {
            for(int i = -length; i <= length; i++)
            {
                for(int j = -length + Mathf.Abs(i); j <= length - Mathf.Abs(i); j++)
                {
                    if( (0 <= x + i) && (x + i < rows) && (0 <= y + j) && (y + j < cols) && !(i == 0 && j == 0))
                    {
                        if(poisson.GetComponent<PoissonSampling>().poissonGrid[x + i, y + j] != null)
                        {
                            nearestPoints.Add(poisson.GetComponent<PoissonSampling>().poissonGrid[x + i, y + j]);
                        }
                    }
                }
                length++;
            }
        }
        return nearestPoints;
    }

    // Calcul les points les plus proches de chaques points
    private void ComputeNearestPoint(int rows, int cols)
    {
        for(int x = 0; x < rows; x++)
        {
            for(int y = 0; y < cols; y++)
            {
                if(poisson.GetComponent<PoissonSampling>().poissonGrid[x, y] != null)
                {
                    int nbNearPoints = 2; //UnityEngine.Random.Range(1, 3);
                    m_intersections.Add(new Intersection(poisson.GetComponent<PoissonSampling>().poissonGrid[x, y], NearestPoint(x, y, nbNearPoints, rows, cols)));
                }
            }
        }
    }

    // Génère les routes en fonctions des voisins
    private void ComputeRoad()
    {
        // va lancer la génération
        PoissonSampling poissonScript = poisson.GetComponent<PoissonSampling>();

        poissonScript.computePoints();
        ComputeNearestPoint(poissonScript.getRowSize, poissonScript.getColSize); // TODO get des lignes et colonnes
       /* 
        foreach(Intersection intersection in m_intersections)
        {
            foreach(Vector3 nPos in intersection.neighbours)
            {
                GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                Road road = plane.AddComponent<Road>();
                road.Init(intersection.position, nPos);
                road.SetRoad();
            }
        }*/
    }

    private void Start()
    {
        poisson = gameObject;
        ComputeRoad();
    }
}
