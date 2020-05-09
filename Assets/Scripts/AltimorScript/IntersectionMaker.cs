using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntersectionMaker : MonoBehaviour
{
    private List<Intersection> m_intersections;
    private PoissonSampling m_poissonScript;


    // Calcul les points les plus proches et retourne la liste des points
    private List<Vector3> NearestPoint(int x, int y, int nbPointsSearched, int rows, int cols) //
    {
        int length = 1;
        List<Vector3> nearestPoints = new List<Vector3>();

        while(nearestPoints.Count <= nbPointsSearched)
        {
            for(int i = -length; i <= length; i++)
            {
                for(int j = -length + Mathf.Abs(i); j <= length - Mathf.Abs(i); j++)
                {
                    if( (0 <= x + i) && (x + i < rows) && (0 <= y + j) && (y + j < cols) && !(i == 0 && j == 0))
                    {
                        if(m_poissonScript.poissonGrid[x + i, y + j] != Vector3.zero)
                        {
                            nearestPoints.Add(m_poissonScript.poissonGrid[x + i, y + j]);
                        }
                    }
                    
                }
                
            }
            length++;

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
                if (m_poissonScript.poissonGrid[x, y] != Vector3.zero)
                {
                    int nbNearPoints = UnityEngine.Random.Range(1, 3);
                    m_intersections.Add(new Intersection(m_poissonScript.poissonGrid[x, y], NearestPoint(x, y, nbNearPoints, rows, cols)));
                }
            }
        }
    }

    // Génère les routes en fonctions des voisins
    private void ComputeRoad()
    {
        // va lancer la génération
        m_poissonScript = gameObject.GetComponent<PoissonSampling>();

        m_poissonScript.computePoints();
        ComputeNearestPoint(m_poissonScript.getRowSize, m_poissonScript.getColSize); // TODO get des lignes et colonnes

        
        foreach(Intersection intersection in m_intersections)
        {
            foreach(Vector3 nPos in intersection.neighbours)
            {
                GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                Road road = plane.AddComponent<Road>();
                road.Init(intersection.position, nPos);
                road.SetRoad();
            }
        }
    }

    private void ComputeDistrict()
    {

    }

    private void Start()
    {
        m_intersections = new List<Intersection>();

        ComputeRoad();
    }
}
