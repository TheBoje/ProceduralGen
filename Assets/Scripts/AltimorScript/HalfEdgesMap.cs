using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder;

// Créer un triangle isolé

public class HalfEdgesMap : MonoBehaviour
{
    private List<HalfEdge> m_halfEdges;
    private List<Vector3> m_positions;

    public void Init()
    {
        m_halfEdges = new List<HalfEdge>();
        m_positions = new List<Vector3>();
    }

    //  
    private void AddIsolatedDart(int indexPosition)
    {
        // TODO Le créer directement opposés
        HalfEdge dart = new HalfEdge(m_halfEdges.Count, m_halfEdges.Count, m_halfEdges.Count + 1, indexPosition);
        m_halfEdges.Add(dart);
        HalfEdge opposite = new HalfEdge(m_halfEdges.Count, m_halfEdges.Count, m_halfEdges.Count - 1, indexPosition);
        m_halfEdges.Add(opposite);
        
    }

    public void AddPoint(Vector3 position)
    {
        AddIsolatedDart(m_positions.Count);
        m_positions.Add(position);
    }

    // Dessine une face à l'aide d'un Line renderer
    private List<int> ComputePointsFace(int firstEdge, List<HalfEdge> printList)
    {
        int currentIndex = firstEdge;
        List<int> indexPoints = new List<int>();

        indexPoints.Add(m_halfEdges[firstEdge].Position);

        do
        {
            currentIndex = m_halfEdges[currentIndex].Next;
            indexPoints.Add(m_halfEdges[currentIndex].Position);
            printList.Remove(m_halfEdges[currentIndex]);
        } while (currentIndex != firstEdge);

        return indexPoints;
    }

    public void DrawMap()
    {
        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.widthMultiplier = 0.2f;
        lineRenderer.positionCount = m_halfEdges.Count;

        List<HalfEdge> copy = new List<HalfEdge>(m_halfEdges);
        List<List<int>> facesList = new List<List<int>>();

        while(copy.Count > 0)
        {
            facesList.Add(ComputePointsFace(m_halfEdges.IndexOf(copy[0]), copy));
        }

        int i = 0;
        int face = 0;

        while (i < lineRenderer.positionCount)
        {
            for (int vertex = 0; vertex < facesList[face].Count; vertex++)
            {
                lineRenderer.SetPosition(i, m_positions[facesList[face][vertex]]);
                i++;
            }
            face++;
        }
        

    }

    public void Demo()
    {
        Init();

        m_positions.Add(new Vector3(0f, 0f, 0f));
        m_positions.Add(new Vector3(0f, 0f, 10f));
        m_positions.Add(new Vector3(10f, 0f, 5f));
        m_positions.Add(new Vector3(20f, 0f, 0f));

        // Face 1
        HalfEdge dart1 = new HalfEdge(1, 2, 3, 0); // 0
        m_halfEdges.Add(dart1);
        HalfEdge dart2 = new HalfEdge(2, 0, 4, 1); // 1
        m_halfEdges.Add(dart2);
        HalfEdge dart3 = new HalfEdge(0, 1, 5, 2); // 2
        m_halfEdges.Add(dart3);

        HalfEdge opp1 = new HalfEdge(9, 4, 0, 1);  // 3
        m_halfEdges.Add(opp1);
        HalfEdge opp2 = new HalfEdge(3, 8, 1, 2);  // 4
        m_halfEdges.Add(opp2);
        HalfEdge opp3 = new HalfEdge(6, 7, 2, 0);  // 5
        m_halfEdges.Add(opp3);

        // Face 2
        HalfEdge dart12 = new HalfEdge(7, 5, 8, 2); // 6
        m_halfEdges.Add(dart12);
        HalfEdge dart22 = new HalfEdge(5, 6, 9, 3); // 7
        m_halfEdges.Add(dart22);

        HalfEdge opp12 = new HalfEdge(4, 9, 6, 3);  // 8
        m_halfEdges.Add(opp12);
        HalfEdge opp22 = new HalfEdge(8, 3, 7, 0);  // 9
        m_halfEdges.Add(opp22);


        DrawMap();
    }

    private void Start()
    {
        Demo();
    }

    /*
    // Cas précis où on ne relie que deux brins -- Faire une fonction plus générale pour l'ajout de brins
    private void LinkTwoStrands(int indexStrand1, int indexStrand2) // Couture -> sew
    {
        m_halfEdges[indexStrand1].LinkStrandToNext(indexStrand2);
        m_halfEdges[indexStrand2].LinkStrandToPrevious(indexStrand1);

        // On insère les brins opposés dans les bons brins
        int oppositeStrandIndex1 = m_halfEdges[indexStrand1].Opposite;
        int oppositeStrandIndex2 = m_halfEdges[indexStrand2].Opposite;

        //                                              l'opposé du brin suivant du brin opposé             l'opposé du brin précédent du brin opposé                   le brin opposé
        m_halfEdges[oppositeStrandIndex1].SetHalfEdge(oppositeStrandIndex2, oppositeStrandIndex2, indexStrand1);
        m_halfEdges[oppositeStrandIndex2].SetHalfEdge(oppositeStrandIndex1, oppositeStrandIndex1, indexStrand2);
    }

    private void LinkThreeStrands(int indexMotherStrand, int nextStrand, int previousStrand)
    {
        m_halfEdges[indexMotherStrand].LinkStrandToNext(nextStrand);
        m_halfEdges[indexMotherStrand].LinkStrandToPrevious(previousStrand);

        m_halfEdges[nextStrand].LinkStrandToPrevious(indexMotherStrand);
        m_halfEdges[previousStrand].LinkStrandToNext(indexMotherStrand);

        int oppositeNextStrandIndex = m_halfEdges.Count;
        HalfEdge oppositeNextStrand = new HalfEdge(oppositeNextStrandIndex, 
            m_halfEdges[previousStrand].Opposite, 
            m_halfEdges[nextStrand].Opposite,
            nextStrand, 
            m_halfEdges[nextStrand].Position);

        m_halfEdges.Add(oppositeNextStrand);

        m_halfEdges[m_halfEdges[nextStrand].Opposite].Next = oppositeNextStrandIndex;
        m_halfEdges[m_halfEdges[previousStrand].Opposite].Previous = oppositeNextStrandIndex;
    }

    // lie trois brins les uns aux autres
    public void linkStrand(int indexStrandToLink, int nextStrand, int previousStrand)
    {
        if (nextStrand == previousStrand)
            LinkTwoStrands(indexStrandToLink, nextStrand);
        else
            LinkThreeStrands(indexStrandToLink, nextStrand, previousStrand);
    }
    */
}